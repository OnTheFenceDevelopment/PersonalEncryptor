using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PersonalEncrypter.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private StorageFolder keyFolder;

        public MainPage()
        {
            this.InitializeComponent();

            btnGenerate.Click += new RoutedEventHandler(btnGenerate_Click);
            btnBrowse.Click += new RoutedEventHandler(btnBrowse_Click);
        }

        private async void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var foo = new FolderPicker();
            foo.ViewMode = PickerViewMode.List;
            foo.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            foo.FileTypeFilter.Add("*");

            keyFolder = await foo.PickSingleFolderAsync();
        }
        private async void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var encrypter = new SodiumEncypter.Encrypter();
                var keyPair = encrypter.GenerateKeyPair();

                var prKey = await keyFolder.CreateFileAsync("DavePrivateKey.txt", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(prKey, Convert.ToBase64String(keyPair.PrivateKey));

                //File.WriteAllText(Path.Combine(keyFolder.Path, "DavePrivateKey.txt"), Convert.ToBase64String(keyPair.PrivateKey));
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
