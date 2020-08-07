using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

using Microsoft.Win32;

namespace Assets.Views
{
    /// <summary>
    ///     Interaction logic for AssetAddingWindow.xaml
    /// </summary>
    public partial class AssetAddingWindow : Window
    {
        private string AssetImage { get; set; }
        private string CalibrationCertificate { get; set; }
        public AssetAddingWindow()
        {
            InitializeComponent();
        }

        private void ImageNameLabel_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DocumentSelectionDialog(out string documentBase64, out string documentName);
            AssetImage = documentBase64;
            ImageNameLabel.Content = documentName;
        }

        private void CalibrationCertificateImageName_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DocumentSelectionDialog(out string documentBase64, out string documentName);
            CalibrationCertificate = documentBase64;
            CalibrationCertificateImageName.Content = documentName;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void DocumentSelectionDialog(out string selectedPicBase64, out string selectedPicName)
        {
            OpenFileDialog op = new OpenFileDialog
            {
                Title = "Select a picture",
                Filter = "All supported graphics|*.jpg;*.jpeg;*.png;*.pdf|" +
                         "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                         "Portable Network Graphic (*.png)|*.png|" +
                         "Portable Document Format (*.pdf)|*.pdf"
            };

            if (op.ShowDialog() != true)
            {
                selectedPicName = "";
                selectedPicBase64 = "";
            }
            string path = new Uri(op.FileName).LocalPath;
            selectedPicBase64 = Convert.ToBase64String(File.ReadAllBytes(path));
            var splits = path.Split('\\');
            selectedPicName = splits[splits.Length - 1];
        }
    }
}