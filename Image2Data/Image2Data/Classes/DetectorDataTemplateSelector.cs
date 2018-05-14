using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Image2Data.Classes
{
    class DetectorDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TextDetectorTemplate { get; set; }

        public DataTemplate ColorDetectorTemplate { get; set; }


        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item.GetType() == typeof(TextDetector))
            {
                return TextDetectorTemplate;
            }

            if (item.GetType() == typeof(ColorDetector))
            {
                return ColorDetectorTemplate;
            }

            return TextDetectorTemplate;
        }
    }
}
