using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEEEsbWPDoor.Model
{
    public class PrinterStatus:INotifyPropertyChanged
    {

        private string fileName;
        public string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                fileName = value;
                NotifyPropertyChanged("FileName");
            }
        }
        private string timeLeft;
        public string TimeLeft
        {
            get
            {
                return timeLeft;
            }
            set
            {
                timeLeft = value;
                NotifyPropertyChanged("TimeLeft");
            }
        }
        private string status;
        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                NotifyPropertyChanged("Status");
            }
        }
        private string completion;
        public string Completion
        {
            get
            {
                return completion;
            }
            set
            {
                completion = value;
                NotifyPropertyChanged("Completion");
            }
        }


        private bool validData;
        public bool ValidData
        {
            get
            {
                return validData;
            }
            set
            {
                validData = value;
                NotifyPropertyChanged("ValidData");
            }
        }

        public PrinterStatus()
        {

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
