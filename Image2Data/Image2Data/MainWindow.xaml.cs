using Image2Data.Classes;
using Image2Data.Vues;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
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
        private System.Windows.Point StartPoint;
        private int DraggedIndex = -1;

        /*
         * Constructeur
         */
        public MainWindow()
        {
            // Création d'un nouveau projet
            Project = new Project();

            // Debug 
            //Project = Project.Open("C:/Users/sservouze/Documents/test.i2d");   

            // Création d'une liste de propriétés
            DetectorProperties = new ObservableCollection<PropertyPresentation>(); 
            
            InitializeComponent();

            // Debug
            //ModelImage.Source = new BitmapImage(new Uri(Project.ImageModelPath));
            ModelImage.AllowDrop = true;

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
            Bitmap img = new Bitmap(fileDropped);

            // Ajoute le chemin de l'image au projet
            Project.ImageModelPath = fileDropped;

            // Force la mise à jour de la taille du composant image et calcule les ratio de rétrécissent de l'image
            ModelImage.UpdateLayout();
            Project.Ratio = new Classes.Vector();
            Project.Ratio.X = (float) (ModelImage.ActualWidth / img.Width);
            Project.Ratio.Y = (float) (ModelImage.ActualHeight / img.Height);
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

        private bool RectangleOfRegionDetectorContain(Detector d, System.Windows.Point p)
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
                Project.Detectors[DetectorList.SelectedIndex].ComputeOutput(new Bitmap(Project.ImageModelPath), Project.Ratio);
            }

        }

        private void OnWindowMouseMove(object sender, MouseEventArgs e)
        {
            // Si le flag drag est activé
            if (DraggedIndex != -1)
            {
                // Récupère la position de la souris par rapport au canvas
                System.Windows.Point mousePos = e.GetPosition(DetectorControlCanvas);

                // Si la position de la souris est contenu dans le rectangle
                Project.Detectors[DraggedIndex].X += (float) (mousePos.X - StartPoint.X);
                Project.Detectors[DraggedIndex].Y += (float) (mousePos.Y - StartPoint.Y);

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
                Project = Project.Open(openFileDialog.FileName);

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
            saveFileDialog.Filter = "Fichiers Image2Data|*.i2d";
            saveFileDialog.AddExtension = true;

            // Sur une fermeture de l'invite par enregistrer
            if (saveFileDialog.ShowDialog() == true)
            {
                // On sauvegarde le projet
                Project.Save(saveFileDialog.FileName);
            }
        }

        private void CanExecuteExtractData(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Project != null;
        }

        private void ExecutedExtractData(object sender, ExecutedRoutedEventArgs e)
        {
            // Création de l'invite d'ouverture de fichier
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Fichiers image|*.png;*.bmp";

            if (openFileDialog.ShowDialog() == true)
            {
                
                Bitmap imageToProcess = new Bitmap(openFileDialog.FileName);

                // Mis à jour de l'image
                ModelImage.Source = new BitmapImage(new Uri(openFileDialog.FileName));

                // On extrait les données
                Project.ExtractData(imageToProcess, new Uri(openFileDialog.FileName.Replace(".png", ".json").Replace(".bmp",".json")).AbsolutePath);
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
            if (Project.Detectors.Count > 0)
                e.CanExecute = Clipboard.GetDataObject().GetDataPresent(Project.Detectors[0].GetType());
            else
                e.CanExecute = false;
        }

        private void ExecutedPasteDetector(object sender, ExecutedRoutedEventArgs e)
        {
            Detector pastedDetector = (Detector)Clipboard.GetDataObject().GetData(Project.Detectors[0].GetType());

            // Déplacement du détecteur copié de 10 pixel en X et en Y
            pastedDetector.X += 10;
            pastedDetector.Y += 10;

            Project.Detectors.Add(pastedDetector);
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

        private void CanExecuteNewColorDetector(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Project != null;
        }

        private void ExecutedNewColorDetector(object sender, ExecutedRoutedEventArgs e)
        {
            AddColorDetector addColor = new AddColorDetector("Detector" + Project.Detectors.Count);
            addColor.Owner = this;
            addColor.ShowDialog();

            if (addColor.ColorDetector != null)
            {
                Project.Detectors.Add(addColor.ColorDetector);
                DetectorList.SelectedIndex = Project.Detectors.Count - 1;
            }
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

        public static readonly RoutedUICommand ExtractData = new RoutedUICommand
        (
            "Extraire les données",
            "Extraire les données",
            typeof(Commands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.E, ModifierKeys.Control)
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
