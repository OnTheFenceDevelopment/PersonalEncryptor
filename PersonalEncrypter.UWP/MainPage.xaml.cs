using PersonalEncrypter.UWP.Views;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PersonalEncrypter.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var itemContent = args.InvokedItem as string;
            if (itemContent != null)
            {
                if (itemContent == Constants.GenerateKeys)
                {
                    contentFrame.Navigate(typeof(GenerateKeys));
                }
                else if (itemContent == Constants.EncryptText)
                {
                    contentFrame.Navigate(typeof(EncryptText));
                }
            }
        }
    }
}
