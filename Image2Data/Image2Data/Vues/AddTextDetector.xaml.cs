using Image2Data.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace Image2Data.Vues
{
    /// <summary>
    /// Logique d'interaction pour AddTextDetector.xaml
    /// </summary>
    public partial class AddTextDetector : Window
    {
        public TextDetector TextDetector;
        private bool cancelled = true;

        public AddTextDetector(string defaultDetectorName)
        {
            // Gestion de la fermeture de la fenêtre
            cancelled = true;
            this.Closing += new CancelEventHandler(OnWindowClosed);

            TextDetector = new TextDetector();
            TextDetector.Name = defaultDetectorName;

            InitializeComponent();

            DataContext = TextDetector;
        }

        private void OnWindowClosed(object sender, CancelEventArgs e)
        {
            if (cancelled)
                TextDetector = null;
        }

        private void OnAdd(object sender, RoutedEventArgs e)
        {
            cancelled = false;
            this.Close();
        }
    }
}
