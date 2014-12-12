using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace IEEEsbWPDoor.Model
{
    public class STLPicker :INotifyPropertyChanged
    {
        private StorageFile stl;
        public StorageFile STL
        {
            get
            {
                return stl;
            }
            set
            {
                stl = value;
                NotifyPropertyChanged("STL");
                NotifyPropertyChanged("ValidFile");
                NotifyPropertyChanged("Filename");
            }
        }
        private string filename;
        public string Filename
        {
            get
            {
                if (STL != null)
                    return STL.Name;
                else return string.Empty;
            }
            set
            {
                if(STL != null)
                    filename = STL.Name;
                NotifyPropertyChanged("Filename");
            }
        }
        private bool validFile;
        public bool ValidFile
        {
            get
            {
                return STL != null;
            }
            set
            {
                validFile = STL != null;
                NotifyPropertyChanged("ValidFile");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string property)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
