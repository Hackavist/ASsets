using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Assets.Helpers;
using Assets.Models;
using Assets.Models.DataModels;
using Assets.Models.Dtos;
using Assets.Models.Enums;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Win32;

namespace Assets.Views
{
    /// <summary>
    ///     Interaction logic for AssetDetailsWindow.xaml
    /// </summary>
    public partial class AssetDetailsWindow : Window
    {
        private string AssetImageBase64 { get; set; } = string.Empty;
        private string AssetImageFormat { get; set; } = string.Empty;
        private string CalibrationCertificateBase64 { get; set; } = string.Empty;
        private string CalibrationCertificateFormat { get; set; } = string.Empty;
        private AssetDto SelectedAssetDto { get; }
        public Asset SelectedAsset { get; set; }
        public ObservableCollection<RepairDto> RepairsList { get; set; }
        public ObservableCollection<RepositionDto> RepositionsList { get; set; }
        private List<string> Errors { get; }
        public string[] ComboboxSource = { "Ready", "Needs Service", "In Service" };

        public AssetDetailsWindow(AssetDto selectedAssetDto)
        {
            InitializeComponent();
            SelectedAssetDto = selectedAssetDto;
            RepairsList = new ObservableCollection<RepairDto>();
            RepositionsList = new ObservableCollection<RepositionDto>();
            StatusPicker.ItemsSource = ComboboxSource;
            Errors = new List<string>();
            Application.Current.Properties[Constants.ShouldAssetDetailsRefresh] = true;
        }

        #region Populating The Ui

        private void AssetDetailsWindow_OnActivated(object sender, EventArgs e)
        {
            if (!(bool)Application.Current.Properties[Constants.ShouldAssetDetailsRefresh]) return;
            FillTheUi();
        }

        private void FillTheUi()
        {
            RepairsList.Clear();
            RepositionsList.Clear();
            using (var dbContext = new DatabaseContext())
            {
                try
                {
                    SelectedAsset = dbContext.Assets.Where(x =>
                        x.AssetId == SelectedAssetDto.AssetId && x.AssetNumber == SelectedAssetDto.AssetNumber).First();
                    foreach (var rep in dbContext.Repair.Where(x => x.AssetId == SelectedAsset.Id)
                        .OrderByDescending(x => x.RepairDate).ToList()) RepairsList.Add(new RepairDto(rep));
                    RepairGrid.ItemsSource = RepairsList;

                    foreach (var rep in dbContext.Repositions.Where(x => x.AssetId == SelectedAsset.Id)
                        .OrderByDescending(x => x.AddedDate).ToList()) RepositionsList.Add(new RepositionDto(rep));
                    RepositionHistory.ItemsSource = RepositionsList;

                    if (SelectedAsset == null)
                    {
                        MessageBox.Show("Invalid selected Asset", "Error");
                        return;
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    MessageBox.Show("Error Loading Assets ");
                }
            }

            if (SelectedAsset == null) return;
            AssetIdBox.Text = SelectedAsset.AssetId;
            PMVCodeBox.Text = SelectedAsset.PMVCode;
            AssetNumberBox.Text = SelectedAsset.AssetNumber;
            PoNumberBox.Text = SelectedAsset.PoNumber;
            AssetNameBox.Text = SelectedAsset.AssetName;
            PlateSerialNumberBox.Text = SelectedAsset.PlateSerialNumber;
            DateOfPurchasePicker.SelectedDate = SelectedAsset.DateOfPurchase;
            CostOfAssetBox.Text = SelectedAsset.PurchaseCostOfAsset.ToString(CultureInfo.InvariantCulture);
            ToolTypeBox.Text = SelectedAsset.ToolType;
            MonthsToDepreciationBox.Text = SelectedAsset.MonthsToDepreciation.ToString();
            StatusPicker.SelectedIndex = ComboboxSource.IndexOf(SelectedAsset.AssetStatus.ToString());
            CalibrationCertificationDatePicker.SelectedDate = SelectedAsset.CalibrationCertificationDate;
            CalibrationCertificationNumberBox.Text = SelectedAsset.CalibrationCertificationNumber;
            var netBookValue = SelectedAsset.PurchaseCostOfAsset -
                               SelectedAsset.PurchaseCostOfAsset / SelectedAsset.MonthsToDepreciation *
                               (DateTime.Today.Month - SelectedAsset.DateOfPurchase.Month + 1);
            netBookValue = Math.Round(netBookValue, 3);
            NetBookValueBox.Text = netBookValue.ToString(CultureInfo.InvariantCulture);
            if (netBookValue > 0.0)
            {
                var mon = SelectedAsset.PurchaseCostOfAsset / SelectedAsset.MonthsToDepreciation;
                mon = Math.Round(mon, 3);
                MonthlyDepreciationDueDateBox.Text = mon.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                MonthlyDepreciationDueDateBox.Text = 1.ToString();
            }

            Application.Current.Properties[Constants.ShouldAssetDetailsRefresh] = false;
        }

        #endregion

        #region Controlling Ui Elements

        private void LockUiElements()
        {
            AssetIdBox.IsReadOnly = true;
            PMVCodeBox.IsReadOnly = true;
            AssetNumberBox.IsReadOnly = true;
            PoNumberBox.IsReadOnly = true;
            AssetNameBox.IsReadOnly = true;
            PlateSerialNumberBox.IsReadOnly = true;
            DateOfPurchasePicker.IsManipulationEnabled = true;
            CostOfAssetBox.IsReadOnly = true;
            ToolTypeBox.IsReadOnly = true;
            MonthsToDepreciationBox.IsReadOnly = true;
            StatusPicker.IsReadOnly = true;
            StatusPicker.IsEditable = true;
            CalibrationCertificationDatePicker.IsManipulationEnabled = true;
            CalibrationCertificationNumberBox.IsReadOnly = true;


            SaveAssetBTN.IsEnabled = false;
            ChangeAssetImageLabel.IsEnabled = false;
            ChangeCertificateLabel.IsEnabled = false;
        }

        private void UnLockUiElements()
        {
            AssetIdBox.IsReadOnly = false;
            PMVCodeBox.IsReadOnly = false;
            AssetNumberBox.IsReadOnly = false;
            PoNumberBox.IsReadOnly = false;
            AssetNameBox.IsReadOnly = false;
            PlateSerialNumberBox.IsReadOnly = false;
            DateOfPurchasePicker.IsManipulationEnabled = true;
            CostOfAssetBox.IsReadOnly = false;
            ToolTypeBox.IsReadOnly = false;
            MonthsToDepreciationBox.IsReadOnly = false;
            StatusPicker.IsReadOnly = false;
            StatusPicker.IsEditable = true;
            CalibrationCertificationDatePicker.IsManipulationEnabled = true;
            CalibrationCertificationNumberBox.IsReadOnly = false;


            SaveAssetBTN.IsEnabled = true;
            ChangeAssetImageLabel.IsEnabled = true;
            ChangeCertificateLabel.IsEnabled = true;
            ChangeAssetImageLabel.Foreground = Brushes.Blue;
            ChangeCertificateLabel.Foreground = Brushes.Blue;
        }

        #endregion

        #region External Files

        private void ShowAssetImageBTN_OnClick(object sender, RoutedEventArgs e)
        {
            var filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + SelectedAsset.AssetName +
                           SelectedAsset.AssetPictureFormat;
            File.WriteAllBytes(filePath, Convert.FromBase64String(SelectedAsset.AssetPictureBase64));
            Process.Start(filePath);
        }

        private void ShowAssetCertificateBTN_OnClick(object sender, RoutedEventArgs e)
        {
            var filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + SelectedAsset.AssetName +
                           " Certificate " + SelectedAsset.CalibrationCertificationPictureFormat;
            File.WriteAllBytes(filePath, Convert.FromBase64String(SelectedAsset.CalibrationCertificationPictureBase64));
            Process.Start(filePath);
        }

        private void ChangeAssetImageLabel_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DocumentSelectionDialog(out var documentBase64, out var documentName);
            AssetImageBase64 = documentBase64;
            var splits = documentName.Split('.');
            AssetImageFormat = "." + splits[splits.Length - 1];
            ChangeAssetImageLabel.Content = documentName;
        }

        private void ChangeCertificateLabel_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DocumentSelectionDialog(out var documentBase64, out var documentName);
            CalibrationCertificateBase64 = documentBase64;
            var splits = documentName.Split('.');
            CalibrationCertificateFormat = "." + splits[splits.Length - 1];
            ChangeCertificateLabel.Content = documentName;
        }

        private static void DocumentSelectionDialog(out string selectedPicBase64, out string selectedPicName)
        {
            var op = new OpenFileDialog
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

            var path = new Uri(op.FileName).LocalPath;
            selectedPicBase64 = Convert.ToBase64String(File.ReadAllBytes(path));
            var splits = path.Split('\\');
            selectedPicName = splits[splits.Length - 1];
        }

        #endregion

        #region Repairs

        private void AddRepairHistory_OnClick(object sender, RoutedEventArgs e)
        {
            var window = new RepairAddingWindow(SelectedAsset.Id, Constants.AssetDetailsWindow);
            window.Show();
        }

        private void RemoveRepairHistory_OnClick(object sender, RoutedEventArgs e)
        {
            using (var dbContext = new DatabaseContext())
            {
                try
                {
                    var lastRepair = dbContext.Repair.Where(x => x.AssetId == SelectedAsset.Id)
                        .OrderByDescending(x => x.AddedDate).First();
                    dbContext.Remove(lastRepair);
                    dbContext.SaveChanges();
                    RepairsList.RemoveAt(RepairsList.Count - 1);

                    MessageBox.Show("Asset Repair Deleted", "Done");
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    MessageBox.Show("Error Deleting Asset repair");
                }
            }
        }

        #endregion

        #region Repositions

        private void RemoveRepositionHistory_OnClick(object sender, RoutedEventArgs e)
        {
            using (var dbContext = new DatabaseContext())
            {
                try
                {
                    var lastReposition = dbContext.Repositions.Where(x => x.AssetId == SelectedAsset.Id)
                        .OrderByDescending(x => x.AddedDate).First();
                    dbContext.Remove(lastReposition);
                    dbContext.SaveChanges();
                    RepositionsList.RemoveAt(0);
                    SelectedAsset = dbContext.Assets.Where(x => x.Id == SelectedAsset.Id).First();
                    SelectedAsset.CurrentLocation = RepositionsList.Count >= 1
                        ? RepositionsList[0].NewPosition
                        : "-";
                    dbContext.Update(SelectedAsset);
                    dbContext.SaveChanges();
                    Application.Current.Properties[Constants.ShouldMainWindowRefresh] = true;
                    MessageBox.Show("Asset History Deleted", "Done");
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    MessageBox.Show("Error Deleting Asset History");
                }
            }
        }

        private void AddRepositionHistory_OnClick(object sender, RoutedEventArgs e)
        {
            var window = new HistoryAddingWindow(SelectedAsset.Id, Constants.AssetDetailsWindow);
            window.Show();
        }

        #endregion

        #region Asset Operations

        private void DeleteAssetBTN_OnClick(object sender, RoutedEventArgs e)
        {
            using (var dbContext = new DatabaseContext())
            {
                try
                {
                    dbContext.Remove(SelectedAsset);
                    dbContext.SaveChanges();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    MessageBox.Show("Error Loading Assets ");
                }
            }

            MessageBox.Show("Asset Deleted");
            Close();
        }

        private void EditAssetBTN_OnClick(object sender, RoutedEventArgs e)
        {
            UnLockUiElements();
        }

        private void SaveAssetBTN_OnClick(object sender, RoutedEventArgs e)
        {
            LockUiElements();
            var editedAsset = ExtractAndValidateAsset();
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
                    editedAsset.Id = SelectedAsset.Id;
                    editedAsset.ModifiedDate = DateTime.Now;
                    dbContext.Update(editedAsset);
                    dbContext.SaveChanges();
                    if (editedAsset.Id < 0)
                    {
                        MessageBox.Show("Error in Addition");
                    }
                    else
                    {
                        MessageBox.Show("Asset Edited");
                        Application.Current.Properties[Constants.ShouldMainWindowRefresh] = true;
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    MessageBox.Show("Error in editing");
                    UnLockUiElements();
                }
            }
        }

        private Asset ExtractAndValidateAsset()
        {
            var temp = new Asset();

            if (!string.IsNullOrWhiteSpace(AssetIdBox.Text))
                temp.AssetId = AssetIdBox.Text;
            else
                Errors.Add("Asset Id Can't be Empty");

            if (!string.IsNullOrWhiteSpace(AssetNameBox.Text))
                temp.AssetName = AssetNameBox.Text;
            else
                Errors.Add("Asset Name Can't be Empty");

            if (!string.IsNullOrWhiteSpace(AssetNumberBox.Text))
                temp.AssetNumber = AssetNumberBox.Text;
            else
                Errors.Add("Asset Number Can't be Empty");

            temp.DateOfPurchase = DateOfPurchasePicker.SelectedDate ?? SelectedAsset.DateOfPurchase;

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
                temp.ToolType = ToolTypeBox.Text;
            else
                Errors.Add("Tool Type Can't be Empty");

            if (!string.IsNullOrWhiteSpace(PMVCodeBox.Text))
                temp.PMVCode = PMVCodeBox.Text;

            if (!string.IsNullOrWhiteSpace(PoNumberBox.Text))
                temp.PoNumber = PoNumberBox.Text;

            if (!string.IsNullOrWhiteSpace(PlateSerialNumberBox.Text))
                temp.PlateSerialNumber = PlateSerialNumberBox.Text;

            if (!string.IsNullOrWhiteSpace(CalibrationCertificationNumberBox.Text))
                temp.CalibrationCertificationNumber = CalibrationCertificationNumberBox.Text;

            temp.CalibrationCertificationDate = CalibrationCertificationDatePicker.SelectedDate ??
                                                SelectedAsset.CalibrationCertificationDate;

            temp.AssetPictureBase64 = !string.IsNullOrWhiteSpace(AssetImageBase64)
                ? AssetImageBase64
                : SelectedAsset.AssetPictureBase64;

            temp.CalibrationCertificationPictureBase64 = !string.IsNullOrWhiteSpace(CalibrationCertificateBase64)
                ? CalibrationCertificateBase64
                : SelectedAsset.CalibrationCertificationPictureBase64;

            temp.AssetPictureFormat = !string.IsNullOrWhiteSpace(AssetImageFormat)
                ? AssetImageFormat
                : SelectedAsset.AssetPictureFormat;


            temp.CalibrationCertificationPictureFormat = !string.IsNullOrWhiteSpace(CalibrationCertificateFormat)
                ? CalibrationCertificateFormat
                : SelectedAsset.CalibrationCertificationPictureFormat;
            return temp;
        }

        #endregion
    }
}