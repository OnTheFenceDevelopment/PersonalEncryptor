using Newtonsoft.Json;
using PersonalEncrypter.Shared;
using PersonalEncrypter.SodiumEncypter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PersonalEncrypter.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EncryptText : Page
    {
        private StorageFile privateKey;
        private StorageFile publicKey;
        private StorageFolder outputPath;
        private Encrypter encrypter;

        public EncryptText()
        {
            this.InitializeComponent();

            btnPrivateKeyBrowse.Click += BtnPrivateKeyBrowse_Click;
            btnPublicKeyBrowse.Click += BtnPublicKeyBrowse_Click;
            btnEncryptText.Click += BtnGenerate_Click;
            btnOutputBrowse.Click += BtnOutputBrowse_Click;
            txtPlainText.KeyUp += txtPlainText_KeyUp;
            txtFilename.KeyUp += txtFilename_KeyUp;
        }

        private void txtFilename_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            SetGenerateButtonState();
        }

        private void txtPlainText_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            SetGenerateButtonState();
        }

        public Encrypter Encrypter
        {
            get
            {
                if (encrypter == null)
                    encrypter = ((App)Application.Current).Encrypter;

                return encrypter;
            }
        }

        private async void BtnOutputBrowse_Click(object sender, RoutedEventArgs e)
        {
            var outputFolderPicker = new FolderPicker();
            outputFolderPicker.ViewMode = PickerViewMode.List;
            outputFolderPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            outputFolderPicker.FileTypeFilter.Add("*");

            outputPath = await outputFolderPicker.PickSingleFolderAsync();

            txtOutputPath.Text = outputPath.Path;

            SetGenerateButtonState();
        }

        private async void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            var packet = string.Empty;

            try
            {
                var privateKeyBytes = await GetKeyBytes(privateKey);

                var publicKeyBytes = await GetKeyBytes(publicKey);

                var nonce = ((App)Application.Current).Encrypter.GenerateNonce();

                var encodedText = Encoding.ASCII.GetBytes(txtPlainText.Text);

                var encryptedData = Encrypter.Encrypt(encodedText, nonce, publicKeyBytes, privateKeyBytes);

                var encryptedFilename = Encrypter.EncryptText(Path.GetFileName(txtFilename.Text), nonce, publicKeyBytes, privateKeyBytes);

                packet = JsonConvert.SerializeObject(new EncryptedPacket
                {
                    Filename = encryptedFilename,
                    EncryptedData = encryptedData,
                    Nonce = nonce
                });
            }
            catch (Exception ex)
            {
                var messageDialog = new MessageDialog($"Exception: {ex.Message}");
                messageDialog.Commands.Add(new UICommand("Close"));

                // Set the command that will be invoked by default
                messageDialog.DefaultCommandIndex = 0;

                // Show the message dialog
                await messageDialog.ShowAsync();

                return;
            }

            try
            {
                var fullOuputFilepath = Path.Combine(outputPath.Path, txtFilename.Text);

                var encryptedPacket = await outputPath.CreateFileAsync(txtFilename.Text, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(encryptedPacket, packet);
            }
            catch (Exception ex)
            {
                var messageDialog = new MessageDialog($"Exception: {ex.Message}");
                messageDialog.Commands.Add(new UICommand("Close"));

                // Set the command that will be invoked by default
                messageDialog.DefaultCommandIndex = 0;

                // Show the message dialog
                await messageDialog.ShowAsync();
            }
        }

        private async void BtnPublicKeyBrowse_Click(object sender, RoutedEventArgs e)
        {
            var publicKeyPicker = new FileOpenPicker();
            publicKeyPicker.ViewMode = PickerViewMode.List;
            publicKeyPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            publicKeyPicker.FileTypeFilter.Add(".txt");

            publicKey = await publicKeyPicker.PickSingleFileAsync();

            txtPublicKey.Text = publicKey.Path;

            SetGenerateButtonState();
        }

        private async void BtnPrivateKeyBrowse_Click(object sender, RoutedEventArgs e)
        {
            var privateKeyPicker = new FileOpenPicker();
            privateKeyPicker.ViewMode = PickerViewMode.List;
            privateKeyPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            privateKeyPicker.FileTypeFilter.Add(".txt");

            privateKey = await privateKeyPicker.PickSingleFileAsync();

            txtPrivateKey.Text = privateKey.Path;

            SetGenerateButtonState();
        }
        private async Task<byte[]> GetKeyBytes(StorageFile keyFile)
        {
            try
            {
                var text = await FileIO.ReadTextAsync(keyFile);
                return Convert.FromBase64String(text);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void SetGenerateButtonState()
        {
            if (privateKey != null && publicKey != null && outputPath != null
                && string.IsNullOrWhiteSpace(txtPlainText.Text) == false && string.IsNullOrWhiteSpace(txtFilename.Text) == false)
            {
                btnEncryptText.IsEnabled = true;
            }
            else
            {
                btnEncryptText.IsEnabled = false;
            }
        }
    }
}
