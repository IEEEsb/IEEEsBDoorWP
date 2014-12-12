using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEEEsbWPDoor.Model
{
    public class User:INotifyPropertyChanged
    {
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }
        private string id;
        public string ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                NotifyPropertyChanged("ID");
            }
        }
        private string token;
        public string Token
        {
            get
            {
                return token;
            }
            set
            {
                token = value;
                NotifyPropertyChanged("Token");
                NotifyPropertyChanged("WelcomeMessage");
            }
        }
        private string regID;
        public string RegistrationID
        {
            get
            {
                return regID;
            }
            set
            {
                regID = value;
                NotifyPropertyChanged("Registration ID");
            }
        }
        private string credit;
        public string Credit
        {
            get
            {
                return credit;
            }
            set
            {
                credit = value;
                NotifyPropertyChanged("Credit");
            }
        }
        private string pairingToken;
        public string PairingToken
        {
            get
            {
                return pairingToken;
            }
            set
            {
                pairingToken = value;
                NotifyPropertyChanged("PairingToken");
            }
        }
        private bool validCredit;
        public bool ValidCredit
        {
            get
            {
                return validCredit;
            }
            set
            {
                validCredit = value;
                NotifyPropertyChanged("ValidCredit");
            }
        }
        private bool updatedCredit;
        public bool UpdatedCredit
        {
            get
            {
                return updatedCredit;
            }
            set
            {
                updatedCredit = value;
                NotifyPropertyChanged("UpdatedCredit");
            }
        }
        private bool validIEEENumber;
        public bool ValidIEEENumber
        {
            get
            {
                return validIEEENumber;
            }
            set
            {
                validIEEENumber = value;
                NotifyPropertyChanged("ValidIEEENumber");
            }
        }
        private string welcomeMessage;
        public string WelcomeMessage
        {
            get
            {
                return welcomeMessage;
            }
            set
            {
                welcomeMessage = value;
                NotifyPropertyChanged("WelcomeMessage");
            }
        }
        private string ieeeNumber;
        public string IEEENumber
        {
            get
            {
                return ieeeNumber;
            }
            set
            {
                ieeeNumber = value;
                NotifyPropertyChanged("IEEENumber");
            }
        }
        private string email;
        public string Email
        {
            get
            {
                return email;
            }
            set
            {
                email = value;
                NotifyPropertyChanged("Email");
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
