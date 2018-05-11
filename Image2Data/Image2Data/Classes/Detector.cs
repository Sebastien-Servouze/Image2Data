using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;


namespace Image2Data.Classes
{
    [Serializable]
    public abstract class Detector : INotifyPropertyChanged
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; NotifyPropertyChanged("Name"); }
        }

        private double x;
        public double X
        {
            get { return x; }
            set { x = value; NotifyPropertyChanged("X"); }
        }

        private double y;
        public double Y
        {
            get { return y; }
            set { y = value; NotifyPropertyChanged("Y"); }
        }

        private double w;
        public double W
        {
            get { return w; }
            set { w = value; NotifyPropertyChanged("W"); }
        }

        private double h;
        public double H
        {
            get { return h; }
            set { h = value; NotifyPropertyChanged("H"); }
        }

        protected string value;
        public string Value
        {
            get { return value; }
        }

        public abstract void ComputeOutput(BitmapImage imageToProcess, Vector ratio, bool grayScale = false);

        protected Bitmap GetPreparedBitmap(BitmapImage imageToProcess, Vector ratio, bool grayScale = false)
        {
            if (grayScale)
                return GetGrayScaleBitmap(GetCroppedBitmap(imageToProcess, ratio));
            else
                return GetCroppedBitmap(imageToProcess, ratio);
        }

        protected Bitmap GetCroppedBitmap(BitmapImage imageToProcess, Vector ratio)
        {
            var bitmap = BitmapImage2Bitmap(imageToProcess);
            RectangleF area = new RectangleF();
            area.X = (float)(X / ratio.X);
            area.Y = (float)(Y / ratio.Y);
            area.Width = (float)(W / ratio.X);
            area.Height = (float)(H / ratio.Y);
            var smallBitmap = bitmap.Clone(area, bitmap.PixelFormat);

            var croppedBitmap = new Bitmap(Convert.ToInt32(smallBitmap.Width / ratio.X) * 3, Convert.ToInt32(smallBitmap.Height / ratio.Y) * 3);
            var graph = Graphics.FromImage(croppedBitmap);
            var brush = new SolidBrush(Color.Black);

            graph.FillRectangle(brush, new RectangleF(0, 0, croppedBitmap.Width, croppedBitmap.Height));
            graph.DrawImage(smallBitmap, 0, 0, croppedBitmap.Width, croppedBitmap.Height);

            return croppedBitmap;
        }

        protected static Bitmap GetGrayScaleBitmap(Bitmap original)
        {
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            Graphics g = Graphics.FromImage(newBitmap);

            ColorMatrix colorMatrix = new ColorMatrix(
               new float[][]
               {
                   new float[] {.3f, .3f, .3f, 0, 0},
                   new float[] {.59f, .59f, .59f, 0, 0},
                   new float[] {.11f, .11f, .11f, 0, 0},
                   new float[] {0, 0, 0, 1, 0},
                   new float[] {0, 0, 0, 0, 1}
               });

            ImageAttributes attributes = new ImageAttributes();

            attributes.SetColorMatrix(colorMatrix);

            g.DrawImage(original, new System.Drawing.Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            g.Dispose();
            return newBitmap;
        }

        protected Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        public void updateFromProperties(PropertyPresentation[] properties)
        {
            properties.ToList().ForEach(updateFromProperty);
        }

        public void updateFromProperty(PropertyPresentation property)
        {
            if (property.Name == "X" || property.Name == "Y" || property.Name == "W" || property.Name == "H")
                this.GetType().GetProperty(property.Name).SetValue(this, Double.Parse(property.Value.ToString()));
            else if (property.Name != "Value")
                this.GetType().GetProperty(property.Name).SetValue(this, property.Value);
        }

        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
