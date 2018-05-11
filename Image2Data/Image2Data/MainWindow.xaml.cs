using Image2Data.Classes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

namespace Image2Data
{
    public partial class MainWindow : Window
    {
        /*
         * Attributs
         */
        private Project project;

        public MainWindow()
        {
            // Création d'un nouveau projet
            project = new Project();

            InitializeComponent();
        }

        /*
         * Gestion des évènements
         */

        private void DropImage(object sender, DragEventArgs e)
        {
            // Pas de données = pas de chocolat
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            string fileDropped = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];

            // Extension non supporté = pas de chocolat
            if (!fileDropped.EndsWith(".png") && !fileDropped.EndsWith(".tiff") && !fileDropped.EndsWith(".bpm"))
                return;

            ModelImage.Source = new BitmapImage(new Uri(fileDropped));

            // Force la mise à jour de la taille du composant image et calcule les ratio de rétrécissent de l'image
            ModelImage.UpdateLayout();
            project.Ratio = new Vector(ModelImage.ActualWidth / ModelImage.Source.Width, ModelImage.ActualHeight / ModelImage.Source.Height);
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
            project = new Project();

            if (!ModelImage.AllowDrop)
                ModelImage.AllowDrop = true;
        }

        private void CanExecuteOpenProject(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ExecutedOpenProject(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Fichiers Image2Data (*.i2d)|*.i2d";

            if (openFileDialog.ShowDialog() == true)
            {
                project = Project.open(openFileDialog.FileName);

                if (!ModelImage.AllowDrop)
                    ModelImage.AllowDrop = true;
            }
        }

        private void CanExecuteCloseProject(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = project != null;
        }

        private void ExecutedCloseProject(object sender, ExecutedRoutedEventArgs e)
        {
            project = null;
            
            if (ModelImage.AllowDrop)
                ModelImage.AllowDrop = false;
        }

        private void CanExecuteSaveProject(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = project != null;
        }

        private void ExecutedSaveProject(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Fichiers Image2Data (*.i2d)|*.i2d";
            saveFileDialog.AddExtension = true;

            if (saveFileDialog.ShowDialog() == true)
            {
                project.save(saveFileDialog.FileName);
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
            // TODO
            e.CanExecute = false;
        }

        private void ExecutedCopyDetector(object sender, ExecutedRoutedEventArgs e)
        {
            // TODO
        }

        private void CanExecutePasteDetector(object sender, CanExecuteRoutedEventArgs e)
        { 
            // TODO
            e.CanExecute = false;
        }

        private void ExecutedPasteDetector(object sender, ExecutedRoutedEventArgs e)
        {
            // TODO
        }

        private void CanExecuteNewTextDetector(object sender, CanExecuteRoutedEventArgs e)
        {
            // TODO
            e.CanExecute = false;
        }

        private void ExecutedNewTextDetector(object sender, ExecutedRoutedEventArgs e)
        {
            // TODO
        }

        private void CanExecuteNewImageDetector(object sender, CanExecuteRoutedEventArgs e)
        {
            // TODO
            e.CanExecute = false;
        }

        private void ExecutedNewImageDetector(object sender, ExecutedRoutedEventArgs e)
        {
            // TODO
        }

        private void CanExecuteNewColorDetector(object sender, CanExecuteRoutedEventArgs e)
        {
            // TODO
            e.CanExecute = false;
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
