using System;
using System.Drawing;
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

        public override void ComputeOutput(Bitmap imageToProcess, Vector ratio, bool grayScale = false)
        {
            TesseractEngine tesseract = (TesseractEngine) App.Current.Properties["Tesseract"];
            Bitmap preparedBitmap = GetGrayScaleBitmap(GetX3Bitmap(GetCroppedBitmap(imageToProcess, ratio)));

            Page result = tesseract.Process(preparedBitmap, PageSegMode.Auto);
            value = result.GetText().Trim();
            result.Dispose();

            NotifyPropertyChanged("Value");
        }
    }
}
