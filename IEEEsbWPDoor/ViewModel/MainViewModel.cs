using IEEEsbWPDoor.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace IEEEsbWPDoor.ViewModel
{
    public class MainViewModel
    {
        public static MainViewModel Current { get; set; }
        public HttpRequestClient HttpRequestClient { get; set; }
        public PrinterStatus PrinterStatus { get; set; }
        public User User { get; set; }
        public STLPicker STLPicker { get; set; }

        public ObservableCollection<PrinterProfile> Profiles { get; set; }
        public PrinterProfile Selected { get; set; }
        
       

        public MainViewModel()
        {
            Current = this;
            HttpRequestClient = new HttpRequestClient();
            PrinterStatus = new PrinterStatus();
            User = new User();
            STLPicker = new STLPicker();
            CheckStoredData();
            Profiles = new ObservableCollection<PrinterProfile>();
        }

        public async Task<bool> TryPair()
        {
            string response = await HttpRequestClient.RequestPair("http://rgnu.ieeesb.etsit.upm.es/latchPair", User.Name, User.ID, User.PairingToken, User.Token);
            return !response.Equals("Nope");
        }

        public async Task<bool> TryUnpair()
        {
            string response = await HttpRequestClient.RequestUnpair("http://rgnu.ieeesb.etsit.upm.es/latchUnpair", User.Name, User.ID, User.Token);
            return !response.Equals("Nope");
        }

        public async Task<bool> TryRequestToken()
        {
            string token = await HttpRequestClient.RequestToken("http://rgnu.ieeesb.etsit.upm.es/door", User.Name, User.ID, User.RegistrationID);
            if (!token.Equals("Nope"))
            {
                User.Token = token;
                MemoryDataToStorage();
                UpdateWelcomeMessage();
                return true;
            }
            else return false;
              
        }

        public async Task<bool> TryOpenToken()
        {
            string result = await HttpRequestClient.RequestOpen("http://rgnu.ieeesb.etsit.upm.es/door", User.Name, User.ID, User.Token);
            return result.Equals("Acceso autorizado");
        }

        public async Task<bool> TryCheckCredit()
        {
            string result = await HttpRequestClient.RequestCredit("http://rgnu.ieeesb.etsit.upm.es/fridge", User.Name, User.ID);
            double credit = 0;
            CultureInfo culture = CultureInfo.InvariantCulture;
            NumberStyles style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.Number;
            if (Double.TryParse(result, style, culture, out credit))
            {
                credit = Math.Round(credit, 2);
                User.Credit = credit.ToString("C");
                MemoryDataToStorage();
                User.ValidCredit = true;
                User.UpdatedCredit = true;
                return true;
            }
            return false;
        }

        public async Task<bool> TryCheckIEEENumber()
        {
            string result = await HttpRequestClient.RequestIEEENumber("http://rgnu.ieeesb.etsit.upm.es/ieeeNumber", User.Name, User.ID, User.Token);
            if (!result.Equals("Nope"))
            {
                User.ValidIEEENumber = true;
                User.IEEENumber = result;
                MemoryDataToStorage();
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateProfiles()
        {
            string result = await HttpRequestClient.RequestPrinterProfiles("http://rgnu.ieeesb.etsit.upm.es/slic3rProfiles");
            if (result.Equals("Error")) return false;
            string[] profiles = result.Split(',');
            Profiles.Clear();
            foreach(string profile in profiles)
            {
                String trimmedProfile = profile.Trim();
                if(trimmedProfile != string.Empty)
                    Profiles.Add(new PrinterProfile(trimmedProfile));
            }
            return true;
        }

        public async Task<bool> SendSTL()
        {
            Stream stream = await STLPicker.STL.OpenStreamForReadAsync();
            string result = await HttpRequestClient.RequestSlic3r("http://rgnu.ieeesb.etsit.upm.es/slic3r", User.Name, User.ID, User.Token, User.Email, Selected.Name, STLPicker.Filename, stream);
            if (!result.Equals("Error"))
            {
                STLPicker.STL = null;
                return true;
            }
            else
            {
                STLPicker.STL = null;
                return false;
            }
        }


        public async Task<bool> UpdatePrinterStatus()
        {
            double completion = 0;
            double timeFile = 0;
            double timeLeftOcto = 0;
            double elapsedTime = 0;
            TimeSpan t; 
            CultureInfo culture = CultureInfo.InvariantCulture;
            try
            {
                string jsonString = await HttpRequestClient.RequestPrinterStatus("http://rgnu.ieeesb.etsit.upm.es/3dprinter");
                if (jsonString.Equals("Error")) return false;
                dynamic json = JValue.Parse(jsonString);
                var job = json.job;
                var file = job.file;
                var progress = json.progress;
                string completionStr = progress.completion;
                Double.TryParse(completionStr, NumberStyles.AllowDecimalPoint, culture, out completion);
                completion = completion / 100;
                string timeLeftOctoStr = progress.printTimeLeft;
                string timeFileStr = job.estimatedPrintTime;
                string elapsedTimeStr = progress.printTime;
                Double.TryParse(timeLeftOctoStr, NumberStyles.AllowDecimalPoint, culture, out timeLeftOcto);
                Double.TryParse(timeFileStr, NumberStyles.AllowDecimalPoint, culture, out timeFile);
                Double.TryParse(elapsedTimeStr, NumberStyles.AllowDecimalPoint, culture, out elapsedTime);
                double timeLeft = (timeFile - elapsedTime);
                if (timeLeft > 0)
                    t = TimeSpan.FromSeconds(timeLeft);
                else
                    t = TimeSpan.FromSeconds(timeLeftOcto);
                PrinterStatus.FileName = "File: " + file.name;
                PrinterStatus.Completion = "Progress: " + completion.ToString("P2");
                PrinterStatus.TimeLeft = "Time left: " + string.Format("{0:D2}h:{1:D2}m:{2:D2}s", t.Hours, t.Minutes, t.Seconds);
                PrinterStatus.Status = "Status: " + json.state;

                return true;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                return false;
            }
            
        }


        public void CheckStoredData()
        {
            object storedName = string.Empty;
            bool thereIsName = Windows.Storage.ApplicationData.Current.LocalSettings.Values.TryGetValue("Name", out storedName);
            if (thereIsName) User.Name = (string)storedName;
            object storedID = string.Empty;
            bool thereIsID = Windows.Storage.ApplicationData.Current.LocalSettings.Values.TryGetValue("ID", out storedID);
            if (thereIsID) User.ID = (string)storedID;
            object storedCredit = string.Empty;
            bool thereIsCredit = Windows.Storage.ApplicationData.Current.LocalSettings.Values.TryGetValue("Credit", out storedCredit);
            if (thereIsCredit)
            {
                User.Credit = (string)storedCredit;
                User.ValidCredit = true;
                User.UpdatedCredit = false;
            }
            object storedIEEENumber = string.Empty;
            bool thereIsIEEENumber = Windows.Storage.ApplicationData.Current.LocalSettings.Values.TryGetValue("IEEENumber", out storedIEEENumber);
            if (thereIsIEEENumber)
            {
                User.IEEENumber = (string)storedIEEENumber;
                User.ValidIEEENumber = true;
            }
            object storedRegID = string.Empty;
            bool thereIsRegID = Windows.Storage.ApplicationData.Current.LocalSettings.Values.TryGetValue("RegID", out storedRegID);
            if (thereIsRegID) User.RegistrationID = (string)storedRegID;
            object storedToken = string.Empty;
            bool thereIsToken = Windows.Storage.ApplicationData.Current.LocalSettings.Values.TryGetValue("Token", out storedToken);
            if (thereIsToken) User.Token = (string)storedToken;
            object storedEmail = string.Empty;
            bool thereIsEmail = Windows.Storage.ApplicationData.Current.LocalSettings.Values.TryGetValue("Email", out storedEmail);
            if (thereIsEmail) User.Email = (string)storedEmail;
            UpdateWelcomeMessage();
        }



        public void MemoryDataToStorage()
        {
            string name = User.Name;
            string id = User.ID;
            string credit = User.Credit;
            string regID = User.RegistrationID;
            string token = User.Token;
            string ieeeNumber = User.IEEENumber;
            string email = User.Email;
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Clear();
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Add(new KeyValuePair<string, object>("Name", name));
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Add(new KeyValuePair<string, object>("ID", id));
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Add(new KeyValuePair<string, object>("Credit", credit));
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Add(new KeyValuePair<string, object>("IEEENumber", ieeeNumber));
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Add(new KeyValuePair<string, object>("RegID", regID));
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Add(new KeyValuePair<string, object>("Token", token));
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Add(new KeyValuePair<string, object>("Email", email));
        }

        public void UpdateWelcomeMessage()
        {
            if (User.Token == null || User.Token == string.Empty)
            {
                User.WelcomeMessage = "You have no access token yet. Please request one using your registration ID.";
            }
            else
            {
                User.WelcomeMessage = "Welcome again, " + MainViewModel.Current.User.Name;
            }
        }
    }
}
