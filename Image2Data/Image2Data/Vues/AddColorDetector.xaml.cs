using Image2Data.Classes;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace Image2Data.Vues
{
    /// <summary>
    /// Logique d'interaction pour AddColorDetector.xaml
    /// </summary>
    public partial class AddColorDetector : Window
    {
        public ColorDetector ColorDetector;
        private bool cancelled = true;

        public AddColorDetector(string defaultDetectorName)
        {
            // Gestion de la fermeture de la fenêtre
            cancelled = true;
            this.Closing += new CancelEventHandler(OnWindowClosed);

            ColorDetector = new ColorDetector();
            ColorDetector.Name = defaultDetectorName;

            InitializeComponent();

            DataContext = ColorDetector;
        }

        private void OnWindowClosed(object sender, CancelEventArgs e)
        {
            if (cancelled)
                ColorDetector = null;
        }

        private void OnAdd(object sender, RoutedEventArgs e)
        {
            cancelled = false;
            this.Close();
        }
    }
}
