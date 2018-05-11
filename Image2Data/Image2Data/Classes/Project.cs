using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Image2Data.Classes
{
    public class Project : INotifyPropertyChanged
    {
        private string path;
        public string Path
        {
            get { return path; }
            set { path = value; NotifyPropertyChanged("Path"); }
        }

        private string imageModelPath;
        public string ImageModelPath
        {
            get { return imageModelPath; }
            set { imageModelPath = value; NotifyPropertyChanged("ImageModelPath"); }
        }

        private Vector ratio;
        public Vector Ratio
        {
            get { return ratio; }
            set { ratio = value; NotifyPropertyChanged("Ratio"); }
        }

        private ObservableCollection<Detector> detectors;
        public ObservableCollection<Detector> Detectors
        {
            get { return detectors; }
            set { detectors = value; NotifyPropertyChanged("Detectors"); }
        }

        public Project()
        {
            Detectors = new ObservableCollection<Detector>();
        }

        // Méthode de sauvegarde
        public void save(string path = null)
        {
            if (path != null)
                Path = path;

            File.WriteAllText(Path, JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            }));
        }

        // Méthode d'ouverture
        public static Project open(string path)
        {
            Project openedProject = JsonConvert.DeserializeObject<Project>(File.ReadAllText(path), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            });

            openedProject.NotifyPropertyChanged("Path");
            openedProject.NotifyPropertyChanged("ImageModelPath");
            openedProject.NotifyPropertyChanged("Ratio");
            openedProject.NotifyPropertyChanged("Detectors");

            BitmapImage imageToProcess = new BitmapImage(new Uri(openedProject.ImageModelPath));
            foreach (Detector d in openedProject.Detectors)
                d.ComputeOutput(imageToProcess, openedProject.Ratio, true);

            return openedProject;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
