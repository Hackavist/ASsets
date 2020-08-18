using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Assets.Helpers;
using Assets.Models;
using Assets.Models.DataModels;
using Assets.Models.Enums;
using Microsoft.Win32;

namespace Assets.Views
{
    /// <summary>
    ///     Interaction logic for AssetAddingWindow.xaml
    /// </summary>
    public partial class AssetAddingWindow : Window
    {
        private string AssetImageBase64 { get; set; }
        private string AssetImageFormat { get; set; }
        private string CalibrationCertificateBase64 { get; set; }
        private string CalibrationCertificateFormat { get; set; }
        private List<string> Errors { get; }
        private int InsertedAssetId { get; set; }

        public AssetAddingWindow()
        {
            InitializeComponent();
            Errors = new List<string>();
            RepairBTN.IsEnabled = false;
            HistoryBTN.IsEnabled = false;
        }

        private void ImageNameLabel_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            string documentBase64 = "";
            string documentName = "";
            DocumentSelectionDialog(out documentBase64, out documentName);
            AssetImageBase64 = documentBase64;
            var splits = documentName.Split('.');
            AssetImageFormat = "." + splits[splits.Length - 1];
            ImageNameLabel.Content = documentName;
        }

        private void CalibrationCertificateImageName_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DocumentSelectionDialog(out var documentBase64, out var documentName);
            CalibrationCertificateBase64 = documentBase64;
            var splits = documentName.Split('.');
            CalibrationCertificateFormat = "." + splits[splits.Length - 1];
            CalibrationCertificateImageName.Content = documentName;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Errors.Clear();
            var pendingAsset = ExtractAndValidateAsset();
            if (Errors.Count > 0)
            {
                var builder = new StringBuilder();
                Errors.ForEach(x =>
                {
                    builder.Append(x);
                    builder.Append("\n");
                });
                var aggregatedErrors = builder.ToString();
                MessageBox.Show(aggregatedErrors, "Please Check The Data");
                return;
            }

            using (var dbContext = new DatabaseContext())
            {
                try
                {
                    pendingAsset.AddedDate = DateTime.Now;
                    dbContext.Add(pendingAsset);
                    dbContext.SaveChanges();
                    if (pendingAsset.Id < 0)
                    {
                        MessageBox.Show("Error in Addition");
                    }
                    else
                    {
                        MessageBox.Show("Asset Added");
                        InsertedAssetId = pendingAsset.Id;
                        RepairBTN.IsEnabled = true;
                        HistoryBTN.IsEnabled = true;
                        ClearBoxes();
                        Application.Current.Properties[Constants.ShouldMainWindowRefresh] = true;
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    MessageBox.Show("Error in Addition");
                }
            }
        }

        private Asset ExtractAndValidateAsset()
        {
            var temp = new Asset();

            if (!string.IsNullOrWhiteSpace(AssetIdBox.Text))
                temp.AssetId = AssetIdBox.Text.ToLower();
            else
                Errors.Add("Asset Id Can't be Empty");

            if (!string.IsNullOrWhiteSpace(AssetNameBox.Text))
                temp.AssetName = AssetNameBox.Text.ToLower();
            else
                Errors.Add("Asset Name Can't be Empty");

            if (!string.IsNullOrWhiteSpace(AssetNumberBox.Text))
                temp.AssetNumber = AssetNumberBox.Text.ToLower();
            else
                Errors.Add("Asset Number Can't be Empty");

            if (DateOfPurchasePicker.SelectedDate.HasValue)
                temp.DateOfPurchase = DateOfPurchasePicker.SelectedDate.Value;
            else
                Errors.Add("Purchase Date Can't be empty");

            if (!string.IsNullOrWhiteSpace(CostOfAssetBox.Text))
            {
                temp.PurchaseCostOfAsset = NumberHelpers.StringToDouble(CostOfAssetBox.Text);
            }
            else
            {
                temp.PurchaseCostOfAsset = -55555.55555;
                Errors.Add("Cost Of Assets Can't be Empty");
            }

            if (!string.IsNullOrWhiteSpace(MonthsToDepreciationBox.Text) &&
                NumberHelpers.IsAllDigits(MonthsToDepreciationBox.Text))
                temp.MonthsToDepreciation = Convert.ToInt32(MonthsToDepreciationBox.Text);
            else
                Errors.Add("Months To Depreciation is Empty or Not correct");

            if (StatusPicker.SelectedItem != null)
                switch (StatusPicker.SelectedIndex)
                {
                    case 0:
                        temp.AssetStatus = Status.Ready;
                        break;
                    case 1:
                        temp.AssetStatus = Status.NeedService;
                        break;
                    case 2:
                        temp.AssetStatus = Status.InService;
                        break;
                }
            else
                Errors.Add("Please select the current status of the Asset");

            if (!string.IsNullOrWhiteSpace(ToolTypeBox.Text))
                temp.ToolType = ToolTypeBox.Text.ToLower();
            else
                Errors.Add("Tool Type Can't be Empty");

            if (!string.IsNullOrWhiteSpace(PMVCodeBox.Text))
                temp.PMVCode = PMVCodeBox.Text.ToLower();

            if (!string.IsNullOrWhiteSpace(PoNumberBox.Text))
                temp.PoNumber = PoNumberBox.Text.ToLower();

            if (!string.IsNullOrWhiteSpace(PlateSerialNumberBox.Text))
                temp.PlateSerialNumber = PlateSerialNumberBox.Text.ToLower();

            if (!string.IsNullOrWhiteSpace(CalibrationCertificationNumberBox.Text))
                temp.CalibrationCertificationNumber = CalibrationCertificationNumberBox.Text.ToLower();

            if (CalibrationCertificationDatePicker.SelectedDate.HasValue)
                temp.CalibrationCertificationDate = CalibrationCertificationDatePicker.SelectedDate.Value;

            if (!string.IsNullOrWhiteSpace(AssetImageBase64))
                temp.AssetPictureBase64 = AssetImageBase64;

            if (!string.IsNullOrWhiteSpace(CalibrationCertificateBase64))
                temp.CalibrationCertificationPictureBase64 = CalibrationCertificateBase64;

            temp.AssetPictureFormat = !string.IsNullOrWhiteSpace(AssetImageFormat) ? AssetImageFormat : "-";

            temp.CalibrationCertificationPictureFormat = !string.IsNullOrWhiteSpace(CalibrationCertificateFormat)
                ? CalibrationCertificateFormat
                : "-";
            return temp;
        }

        private void ClearBoxes()
        {
            AssetIdBox.Text = "";
            AssetNameBox.Text = "";
            AssetNumberBox.Text = "";
            CostOfAssetBox.Text = "";
            MonthsToDepreciationBox.Text = "";
            ToolTypeBox.Text = "";
            PMVCodeBox.Text = "";
            PoNumberBox.Text = "";
            PlateSerialNumberBox.Text = "";
            CalibrationCertificationNumberBox.Text = "";
            CalibrationCertificateImageName.Content = "Click Here to add an Image";
            ImageNameLabel.Content = "Click Here to add an Image or pdf";
            DateOfPurchasePicker.SelectedDate = null;
            CalibrationCertificationDatePicker.SelectedDate = null;
            StatusPicker.SelectedItem = null;
            AssetImageBase64 = "";
            CalibrationCertificateBase64 = "";
        }

        private static void DocumentSelectionDialog(out string selectedPicBase64, out string selectedPicName)
        {
            selectedPicName = "";
            selectedPicBase64 = "";
            var op = new OpenFileDialog
            {
                Title = "Select a picture",
                Filter = "All supported graphics|*.jpg;*.jpeg;*.png;*.pdf|" +
                         "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                         "Portable Network Graphic (*.png)|*.png|" +
                         "Portable Document Format (*.pdf)|*.pdf"
            };

            bool? result = op.ShowDialog();

            if (result == true)
            {
                var path = new Uri(op.FileName).LocalPath;
                selectedPicBase64 = Convert.ToBase64String(File.ReadAllBytes(path));
                var splits = path.Split('\\');
                selectedPicName = splits[splits.Length - 1];
            }
        }

        private void HistoryBTN_OnClick(object sender, RoutedEventArgs e)
        {
            var window = new HistoryAddingWindow(InsertedAssetId, Constants.AssetAdditionWindow);
            window.Show();
        }

        private void RepairBTN_OnClick(object sender, RoutedEventArgs e)
        {
            var window = new RepairAddingWindow(InsertedAssetId, Constants.AssetAdditionWindow);
            window.Show();
        }
    }
}