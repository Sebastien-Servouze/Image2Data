using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace Image2Data.Classes
{
    public class Project
    {
        public string Path { get; set; }
        public string ImageModelPath { get; set; }
        public Vector Ratio { get; set; }
        public ObservableCollection<Detector> Detectors { get; set; }

        // Méthode de sauvegarde
        public void save(string path = null)
        {
            if (path != null)
                Path = path;

            File.WriteAllText(Path, JsonConvert.SerializeObject(this));
        }

        // Méthode d'ouverture
        public static Project open(string path)
        {
            return JsonConvert.DeserializeObject<Project>(File.ReadAllText(path));
        }
    }
}
