using Image2Data.Classes;
using System.ComponentModel;
using System.Windows;
using Tesseract;

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

            TextDetector = new TextDetector((TesseractEngine) App.Current.Properties["Tesseract"]);
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
