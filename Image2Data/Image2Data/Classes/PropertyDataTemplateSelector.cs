using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Image2Data
{
    public class PropertyDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate StringPropertyTemplate { get; set; }

        public DataTemplate NumberPropertyTemplate { get; set; }

        public DataTemplate BooleanPropertyTemplate { get; set; }

        public DataTemplate ValuePropertyTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            PropertyPresentation property = item as PropertyPresentation;

            if (property.Value == null || property.Name == "Value")
            {
                return ValuePropertyTemplate;
            }

            if (property.Value.GetType() == typeof(double) || property.Value.GetType() == typeof(int))
            {
                return NumberPropertyTemplate;
            }

            if (property.Value.GetType() == typeof(bool))
            {
                return BooleanPropertyTemplate;
            }

            return StringPropertyTemplate;
        }
    }
}
