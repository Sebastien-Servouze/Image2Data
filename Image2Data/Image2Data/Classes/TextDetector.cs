using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Tesseract;

namespace Image2Data.Classes
{
    [Serializable]
    public class TextDetector : Detector
    {
        public TextDetector()
        {
            W = 100;
            H = 24;
        }

        public override void ComputeOutput(BitmapImage imageToProcess, Vector ratio, bool grayScale = false)
        {
            TesseractEngine tesseract = (TesseractEngine) App.Current.Properties["Tesseract"];
            Bitmap preparedBitmap = GetPreparedBitmap(imageToProcess, ratio, true);

            Page result = tesseract.Process(preparedBitmap, PageSegMode.Auto);
            value = result.GetText().Trim();
            result.Dispose();

            NotifyPropertyChanged("Value");
        }
    }
}
