using Image2Data.Classes;
using Image2Data.Vues;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tesseract;

namespace Image2Data
{
    public partial class MainWindow : Window
    {
        /*
         * Attributs
         */
        private Project Project;
        private ObservableCollection<PropertyPresentation> DetectorProperties { get; set; }

        // Drag de rectangles
        private Point StartPoint;
        private int DraggedIndex = -1;

        /*
         * Constructeur
         */
        public MainWindow()
        {
            // Initialisation de Tesseract
            if (App.Current.Properties["Tesseract"] == null)
                App.Current.Properties["Tesseract"] = new TesseractEngine("C:/Users/sservouze/Documents/Tesseract", "eng");

            // Création d'un nouveau projet
            Project = new Project();

            // Création d'une liste de propriétés
            DetectorProperties = new ObservableCollection<PropertyPresentation>(); 
            
            InitializeComponent();

            InitBinding();
        }

        /*
         * Gestion des évènements
         */

        private void DropImage(object sender, DragEventArgs e)
        {
            // Pas de données = pas de chocolat
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            // Récupère le premier fichier parmis les fichiers déposés
            string fileDropped = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];

            // Extension non supporté = pas de chocolat
            if (!fileDropped.EndsWith(".png") && !fileDropped.EndsWith(".tiff") && !fileDropped.EndsWith(".bpm"))
                return;

            // Change la source de l'image
            ModelImage.Source = new BitmapImage(new Uri(fileDropped));

            // Ajoute le chemin de l'image au projet
            Project.ImageModelPath = fileDropped;

            // Force la mise à jour de la taille du composant image et calcule les ratio de rétrécissent de l'image
            ModelImage.UpdateLayout();
            Project.Ratio = new Vector(ModelImage.ActualWidth / ModelImage.Source.Width, ModelImage.ActualHeight / ModelImage.Source.Height);
        }

        /*
         * Méthodes privées
         */
        
        private void InitBinding()
        {
            // Liaison des détecteurs à la liste de détecteurs
            DetectorList.ItemsSource = Project.Detectors;

            // Liaison des détecteurs au canvas
            DetectorControlCanvas.ItemsSource = Project.Detectors;

            // Liaison des propriétés à la liste de propriétés
            PropertyList.ItemsSource = DetectorProperties;
        }

        private bool RectangleOfRegionDetectorContain(Detector d, Point p)
        {
            return p.X >= d.X && p.X <= d.X + d.W && p.Y >= d.Y && p.Y <= d.Y + d.H;
        }

        /*
         * Evènements
         */

        private void OnDetectorSelected(object sender, SelectionChangedEventArgs e)
        {
            DetectorProperties.Clear();

            if (DetectorList.SelectedIndex == -1)
                return;

            PropertyInfo[] propertyInfos = Project.Detectors[DetectorList.SelectedIndex].GetType().GetProperties();
            foreach (PropertyInfo property in propertyInfos)
                DetectorProperties.Add(new PropertyPresentation(property, Project.Detectors[DetectorList.SelectedIndex]));
        }

        private void OnWindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DraggedIndex == -1)
            {
                // Sélection de la région liée au rectangle
                for (int i = 0; i < Project.Detectors.Count; i++)
                {
                    if (RectangleOfRegionDetectorContain(Project.Detectors[i], e.GetPosition(DetectorControlCanvas)))
                    {
                        DetectorList.SelectedIndex = i;
                        StartPoint = e.GetPosition(DetectorControlCanvas);
                        DraggedIndex = i;

                        break;
                    }
                }
            }
        }

        private void OnWindowMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (DraggedIndex != -1)
            {
                DraggedIndex = -1;

                // Lecture du texte
                Project.Detectors[DetectorList.SelectedIndex].ComputeOutput(new BitmapImage(new Uri(Project.ImageModelPath)), Project.Ratio);
            }

        }

        private void OnWindowMouseMove(object sender, MouseEventArgs e)
        {
            // Si le flag drag est activé
            if (DraggedIndex != -1)
            {
                // Récupère la position de la souris par rapport au canvas
                Point mousePos = e.GetPosition(DetectorControlCanvas);

                // Si la position de la souris est contenu dans le rectangle
                Project.Detectors[DraggedIndex].X += mousePos.X - StartPoint.X;
                Project.Detectors[DraggedIndex].Y += mousePos.Y - StartPoint.Y;

                StartPoint.X = mousePos.X;
                StartPoint.Y = mousePos.Y;

                // Mis à jour des propriétés
                DetectorProperties.Clear();
                PropertyInfo[] propertyInfos = Project.Detectors[DraggedIndex].GetType().GetProperties();
                foreach (PropertyInfo property in propertyInfos)
                    DetectorProperties.Add(new PropertyPresentation(property, Project.Detectors[DraggedIndex]));
            }
        }

        private void OnPropertyChange(object sender, TextChangedEventArgs e)
        {
            Project.Detectors[DetectorList.SelectedIndex].updateFromProperties(DetectorProperties.ToArray());
        }

        private void OnPropertyChange(object sender, SelectionChangedEventArgs e)
        {
            Project.Detectors[DetectorList.SelectedIndex].updateFromProperties(DetectorProperties.ToArray());
        }

        private void OnPropertyChange(object sender, RoutedEventArgs e)
        {
            Project.Detectors[DetectorList.SelectedIndex].updateFromProperties(DetectorProperties.ToArray());
        }

        /*
         * Gestion des commandes
         */

        private void CanExecuteNewProject(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ExecutedNewProject(object sender, ExecutedRoutedEventArgs e)
        {
            Project = new Project();

            if (!ModelImage.AllowDrop)
                ModelImage.AllowDrop = true;
        }

        private void CanExecuteOpenProject(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ExecutedOpenProject(object sender, ExecutedRoutedEventArgs e)
        {
            // Création de l'invite d'ouverture de fichier
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Filtre les fichiers ouvrables
            openFileDialog.Filter = "Fichiers Image2Data (*.i2d)|*.i2d";

            // Sur une fermeture par "ouvrir"
            if (openFileDialog.ShowDialog() == true)
            {
                // Chargement du projet
                Project = Project.open(openFileDialog.FileName);

                // Mis à jour de l'image
                ModelImage.Source = new BitmapImage(new Uri(Project.ImageModelPath));

                // Binding
                InitBinding();

                // Autorisation du drop 
                if (!ModelImage.AllowDrop)
                    ModelImage.AllowDrop = true;
            }
        }

        private void CanExecuteCloseProject(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Project != null;
        }

        private void ExecutedCloseProject(object sender, ExecutedRoutedEventArgs e)
        {
            Project = null;
            
            if (ModelImage.AllowDrop)
                ModelImage.AllowDrop = false;
        }

        private void CanExecuteSaveProject(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Project != null;
        }

        private void ExecutedSaveProject(object sender, ExecutedRoutedEventArgs e)
        {
            // Création de l'invite de sauvegarde de fichier
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            // Force l'extension .i2d
            saveFileDialog.Filter = "Fichiers Image2Data (*.i2d)|*.i2d";
            saveFileDialog.AddExtension = true;

            // Sur une fermeture de l'invite par enregistrer
            if (saveFileDialog.ShowDialog() == true)
            {
                // On sauvegarde le projet
                Project.save(saveFileDialog.FileName);
            }
        }

        private void CanExecuteCloseApplication(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ExecutedCloseApplication(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void CanExecuteCopyDetector(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DetectorList.SelectedIndex != -1;
        }

        private void ExecutedCopyDetector(object sender, ExecutedRoutedEventArgs e)
        {
            Clipboard.SetDataObject(Project.Detectors[DetectorList.SelectedIndex]);
        }

        private void CanExecutePasteDetector(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Clipboard.GetDataObject().GetDataPresent(Project.Detectors[0].GetType());
        }

        private void ExecutedPasteDetector(object sender, ExecutedRoutedEventArgs e)
        {
            Project.Detectors.Add((Detector)Clipboard.GetDataObject().GetData(Project.Detectors[0].GetType()));
        }

        private void CanExecuteNewTextDetector(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Project != null;
        }

        private void ExecutedNewTextDetector(object sender, ExecutedRoutedEventArgs e)
        {
            AddTextDetector addText = new AddTextDetector("Detector" + Project.Detectors.Count);
            addText.Owner = this;
            addText.ShowDialog();

            if (addText.TextDetector != null)
            {
                Project.Detectors.Add(addText.TextDetector);
                DetectorList.SelectedIndex = Project.Detectors.Count - 1;
            }
        }

        private void CanExecuteNewImageDetector(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Project != null;
        }

        private void ExecutedNewImageDetector(object sender, ExecutedRoutedEventArgs e)
        {
            // TODO
        }

        private void CanExecuteNewColorDetector(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Project != null;
        }

        private void ExecutedNewColorDetector(object sender, ExecutedRoutedEventArgs e)
        {
            // TODO
        }    
    }

    public static class Commands
    {
        public static readonly RoutedUICommand NewProject = new RoutedUICommand
        (
            "Nouveau Projet",
            "Nouveau Projet",
            typeof(Commands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.N, ModifierKeys.Control)
            }
        );

        public static readonly RoutedUICommand OpenProject = new RoutedUICommand
        (
            "Ouvrir Projet",
            "Ouvrir Projet",
            typeof(Commands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.O, ModifierKeys.Control)
            }
        );

        public static readonly RoutedUICommand CloseProject = new RoutedUICommand
        (
            "Fermer Projet",
            "Fermer Projet",
            typeof(Commands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.X, ModifierKeys.Control)
            }
        );

        public static readonly RoutedUICommand SaveProject = new RoutedUICommand
        (
            "Sauvegarder Projet",
            "Sauvegarder Projet",
            typeof(Commands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.S, ModifierKeys.Control)
            }
        );

        public static readonly RoutedUICommand CloseApplication = new RoutedUICommand
        (
            "Quitter",
            "Quitter",
            typeof(Commands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.F4, ModifierKeys.Alt)
            }
        );

        public static readonly RoutedUICommand CopyDetector = new RoutedUICommand
        (
            "Copier détecteur",
            "Copier détecteur",
            typeof(Commands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.C, ModifierKeys.Control)
            }
        );

        public static readonly RoutedUICommand PasteDetector = new RoutedUICommand
        (
            "Coller détecteur",
            "Coller détecteur",
            typeof(Commands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.V, ModifierKeys.Control)
            }
        );

        public static readonly RoutedUICommand NewTextDetector = new RoutedUICommand
        (
            "Nouveau détecteur de Texte",
            "Nouveau détecteur de Texte",
            typeof(Commands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.T, ModifierKeys.Alt)
            }
        );

        public static readonly RoutedUICommand NewImageDetector = new RoutedUICommand
        (
            "Nouveau détecteur d'Image",
            "Nouveau détecteur d'Image",
            typeof(Commands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.I, ModifierKeys.Alt)
            }
        );

        public static readonly RoutedUICommand NewColorDetector = new RoutedUICommand
        (
            "Nouveau détecteur de Couleur",
            "Nouveau détecteur de Couleur",
            typeof(Commands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.C, ModifierKeys.Alt)
            }
        );
    }
}
