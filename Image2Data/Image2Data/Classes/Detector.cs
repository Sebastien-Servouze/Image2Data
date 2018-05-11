using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image2Data.Classes
{
    public abstract class Detector : INotifyPropertyChanged
    {
        protected string name;
        public string Name
        {
            get { return name; }
            set { name = value; NotifyPropertyChanged("Name"); }
        }

        protected double x;
        public double X
        {
            get { return x; }
            set { x = value; NotifyPropertyChanged("X"); }
        }

        protected double y;
        public double Y
        {
            get { return y; }
            set { y = value; NotifyPropertyChanged("Y"); }
        }

        protected double w;
        public double W
        {
            get { return w; }
            set { w = value; NotifyPropertyChanged("W"); }
        }

        protected double h;
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
