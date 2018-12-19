using System;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PersonalEncrypter.UWP.Views
{
    public sealed partial class GenerateKeys : Page
    {
        private StorageFolder outputPath;

        public GenerateKeys()
        {
            this.InitializeComponent();

            btnBrowse.Click += btnBrowse_Click;

            chkSaveToFile.Click += ChkSaveToFile_Checked;
            chkDisplayOnScreen.Click += ChkDisplayOnScreen_Checked;

            txtOutputPath.Text = "Please Select an Output Folder";

            btnGenerate.Click += BtnGenerate_Click;
        }

        private async void btnBrowse_Click(object sender, RoutedEventArgs e)
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
            var keyPair = ((App)Application.Current).Encrypter.GenerateKeyPair();

            if (chkDisplayOnScreen.IsChecked == true)
            {
                txtPublicKey.Text = Convert.ToBase64String(keyPair.PublicKey);
                txtPrivateKey.Text = Convert.ToBase64String(keyPair.PrivateKey);
            }

            if (chkSaveToFile.IsChecked == true)
            {
                var privateKeyFileExists = await outputPath.TryGetItemAsync($"{txtKeyName.Text}PrivateKey.txt");
                var publicKeyFileExists = await outputPath.TryGetItemAsync($"{txtKeyName.Text}PublicKey.txt");

                if (privateKeyFileExists != null || publicKeyFileExists != null)
                {
                    var overwriteExistingFilesDialog = new ContentDialog
                    {
                        Title = "Key(s) Exist in Output Location",
                        Content = "Overwrite key files with same name(s) in the specified output folder?",
                        PrimaryButtonText = "Yes",
                        SecondaryButtonText = "No"
                    };

                    var shouldOverwrite = await overwriteExistingFilesDialog.ShowAsync();

                    if (shouldOverwrite == ContentDialogResult.Secondary)
                        return;
                }

                var privateKey = await outputPath.CreateFileAsync($"{txtKeyName.Text}PrivateKey.txt", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(privateKey, Convert.ToBase64String(keyPair.PrivateKey));

                var publicKey = await outputPath.CreateFileAsync($"{txtKeyName.Text}PublicKey.txt", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(publicKey, Convert.ToBase64String(keyPair.PublicKey));
            }
        }

        private void ChkDisplayOnScreen_Checked(object sender, RoutedEventArgs e)
        {
            txtPrivateKey.IsEnabled = txtPublicKey.IsEnabled = chkDisplayOnScreen.IsChecked ?? false;

            if (chkDisplayOnScreen.IsChecked == false)
                txtPublicKey.Text = txtPrivateKey.Text = string.Empty;

            SetGenerateButtonState();
        }

        private void ChkSaveToFile_Checked(object sender, RoutedEventArgs e)
        {
            btnBrowse.IsEnabled = chkSaveToFile.IsChecked ?? false;
            txtKeyName.IsEnabled = chkSaveToFile.IsChecked ?? false;

            SetGenerateButtonState();
        }

        private void SetGenerateButtonState()
        {
            if (chkDisplayOnScreen.IsChecked == true || (chkSaveToFile.IsChecked == true && outputPath != null))
                btnGenerate.IsEnabled = true;
            else
                btnGenerate.IsEnabled = false;
        }
    }
}
