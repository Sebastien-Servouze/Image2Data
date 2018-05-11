using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Image2Data
{
    public class PropertyPresentation : INotifyPropertyChanged
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; NotifyPropertyChanged("Name"); }
        }

        private object value;
        public object Value
        {
            get { return value; }
            set { this.value = value; NotifyPropertyChanged("Value"); }
        }

        public PropertyPresentation(PropertyInfo propertyInfo, object src)
        {
            Name = propertyInfo.Name;
            Value = propertyInfo.GetValue(src);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
