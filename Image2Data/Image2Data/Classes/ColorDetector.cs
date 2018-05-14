using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Image2Data.Classes
{
    public class ColorDetector : Detector
    { 
        public ColorDetector()
        {
            W = 20;
            H = 20;
        }

        public override void ComputeOutput(Bitmap imageToProcess, Vector ratio, bool grayScale = false)
        {
            Bitmap preparedBitmap = GetCroppedBitmap(imageToProcess, ratio);
            Dictionary<Color, int> colorDictionnary = new Dictionary<Color, int>();

            Color pixel;
            Console.WriteLine(preparedBitmap.Width + "x" + preparedBitmap.Height);
            Console.WriteLine(preparedBitmap.Width * preparedBitmap.Height);
            for (int i = 0; i < preparedBitmap.Width * preparedBitmap.Height; i++)
            {
                pixel = preparedBitmap.GetPixel(i % preparedBitmap.Width, i / preparedBitmap.Width);

                if (colorDictionnary.ContainsKey(pixel))
                    colorDictionnary[pixel]++;
                else
                    colorDictionnary[pixel] = 1;
            }

            value = ColorTranslator.ToHtml(colorDictionnary.Aggregate((color, val) => color.Value > val.Value ? color : val).Key);
            NotifyPropertyChanged("Value");
        }
    }
}
