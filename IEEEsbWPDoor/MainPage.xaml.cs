using IEEEsbWPDoor.Model;
using IEEEsbWPDoor.View;
using IEEEsbWPDoor.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.PushNotifications;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en http://go.microsoft.com/fwlink/?LinkId=391641

namespace IEEEsbWPDoor
{
    /// <summary>
    /// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IFileOpenPickerContinuable
    {
        public static MainPage Current;

        public MainPage()
        {
            this.InitializeComponent();
            Current = this;
            this.NavigationCacheMode = NavigationCacheMode.Required;
            MainViewModel.Current.CheckStoredData();
            //InitializePush(); TODO
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SuspensionManager.RegisterFrame(this.Frame, "MainFrame");
        }


        private async void InitializePush()
        {
            PushNotificationChannel channel = null;

            try
            {
                channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
                // Success. The channel URI is found in channel.Uri.
            }

            catch (Exception ex)
            {
                // Could not create a channel. 
            }
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ToastNotificationManager.History.Remove("IEEEsbApp");
        }


        private void Open_Click(object sender, RoutedEventArgs e)
        {
            open();
        }

        private async void open()
        {
            bool success = await MainViewModel.Current.TryOpenToken();
            if (success)
            {
                ShowToastNotification("Opening door");
            }

            else
            {
                ShowToastNotification("An error occurred");
            }
        }


        private void CreditButton_Click(object sender, RoutedEventArgs e)
        {
            checkCredit();
        }

        private async void checkCredit()
        {
            bool success = await MainViewModel.Current.TryCheckCredit();
            if (success)
            {
                ShowToastNotification("Credit updated");
            }

            else
                ShowToastNotification("Credit cannot be checked");
        }

        private void PrinterButton_Click(object sender, RoutedEventArgs e)
        {
            updatePrinterStatus();
        }

        private async void updatePrinterStatus()
        {
            bool updated = await MainViewModel.Current.UpdatePrinterStatus();
            if (updated)
            {
                ShowToastNotification("3D Printer status updated");
                MainViewModel.Current.PrinterStatus.ValidData = true;
            }
            else
            {
                ShowToastNotification("Status not available");
            }
        }

        private void CheckProfiles_Click(object sender, RoutedEventArgs e)
        {
            updatePrinterProfiles();
        }

        private async void updatePrinterProfiles()
        {
            bool updated = await MainViewModel.Current.UpdateProfiles();
            if(updated)
            {
                ShowToastNotification("3D Printer profiles updated");
            }
            else
            {
                ShowToastNotification("Profiles not available");
            }
        }

        private void PairButton_Click(object sender, RoutedEventArgs e)
        {
            pair();
        }

        private async void pair()
        {
            bool paired = await MainViewModel.Current.TryPair();
            if(!paired)
            {
                ShowToastNotification("An error occured");
            }
        }

        private void UnpairButton_Click(object sender, RoutedEventArgs e)
        {
            unpair();
        }

        private async void unpair()
        {
            bool unpaired = await MainViewModel.Current.TryUnpair();
            if (!unpaired)
            {
                ShowToastNotification("An error occured");
            }
        }




        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            DataStorageView dataStorageView = new DataStorageView();
            var result = await dataStorageView.ShowAsync();
            if(result == ContentDialogResult.Secondary)
            {
                bool success = await MainViewModel.Current.TryRequestToken();
                if(success)
                {
                    ShowToastNotification("Token received!");
                }
                else 
                {
                    ShowToastNotification("Couldn't get token");
                }
                success = await MainViewModel.Current.TryCheckIEEENumber();
                if (success)
                {
                    ShowToastNotification("IEEE Number retrieved");
                }
                else
                {
                    ShowToastNotification("Could not check IEEE number");
                }
            }
            else if(result == ContentDialogResult.Primary)
            {
                MainViewModel.Current.MemoryDataToStorage();
            }
        }


        private void ShowToastNotification(String message)
        {
            ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText01;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

            // Set Text
            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode(message));

            // Set image
            // Images must be less than 200 KB in size and smaller than 1024 x 1024 pixels.
            XmlNodeList toastImageAttributes = toastXml.GetElementsByTagName("image");
            ((XmlElement)toastImageAttributes[0]).SetAttribute("src", "ms-appx:///Assets/Logo.scale-100.png");
            ((XmlElement)toastImageAttributes[0]).SetAttribute("alt", "logo");

            // toast duration
            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            ((XmlElement)toastNode).SetAttribute("duration", "short");


            // Create the toast notification based on the XML content you've specified.
            ToastNotification toast = new ToastNotification(toastXml);
            toast.Tag = "IEEEsbApp";
            // Send your toast notification.
            ToastNotificationManager.CreateToastNotifier().Show(toast);
            toast.Dismissed += toast_Dismissed;
        }

        public static void toast_Dismissed(ToastNotification sender, ToastDismissedEventArgs args)
        {
            ToastNotificationManager.History.Remove("IEEEsbApp");
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainViewModel.Current.Selected = (PrinterProfile)e.AddedItems.ElementAt(0);
        }


        private void SendSTL_Click(object sender, RoutedEventArgs e)
        {
            sendSTL();
        }



        public void PickFile()
        {
            FileOpenPicker opener = new FileOpenPicker();
            opener.FileTypeFilter.Clear();
            opener.FileTypeFilter.Add(".stl");
            opener.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            opener.ViewMode = PickerViewMode.Thumbnail;
            opener.PickSingleFileAndContinue();
        }

        private void PickSTL_Click(object sender, RoutedEventArgs e)
        {
            PickFile();
        }

        public void ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs args)
        {
            if (args.Files.Count > 0)
            {
                MainViewModel.Current.STLPicker.STL = args.Files[0];
            }
        }

        private async void sendSTL()
        {
            if(MainViewModel.Current.Profiles == null)
            {
                ShowToastNotification("Please select a profile first");
                return;
            }
            bool sent = await MainViewModel.Current.SendSTL();
            if (sent)
            {
                ShowToastNotification("STL sent");
            }
            else
            {
                ShowToastNotification("An error occurred");
            }
        }
    }
}
