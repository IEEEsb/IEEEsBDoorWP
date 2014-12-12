using IEEEsbWPDoor.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers.Provider;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento del cuadro de diálogo de contenido está documentada en http://go.microsoft.com/fwlink/?LinkID=390556

namespace IEEEsbWPDoor.View
{
    public sealed partial class DataStorageView : ContentDialog
    {

        public DataStorageView()
        {
            this.InitializeComponent();
            InputPane inputPane = InputPane.GetForCurrentView();
            inputPane.Showing += inputPane_Showing;
            inputPane.Hiding += inputPane_Hiding;
        }

        void inputPane_Hiding(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            Body.Height = Double.NaN;
        }

        private async void inputPane_Showing(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Body.Height = 130;
                 });
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

        }

    }
}
