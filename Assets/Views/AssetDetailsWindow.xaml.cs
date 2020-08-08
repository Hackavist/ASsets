﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using Assets.Helpers;
using Assets.Models;
using Assets.Models.DataModels;
using Assets.Models.Dtos;
using Microsoft.EntityFrameworkCore.Internal;

namespace Assets.Views
{
    /// <summary>
    ///     Interaction logic for AssetDetailsWindow.xaml
    /// </summary>
    public partial class AssetDetailsWindow : Window
    {
        private AssetDto SelectedAssetDto { get; }
        public Asset SelectedAsset { get; set; }
        public ObservableCollection<RepairDto> RepairsList { get; set; }
        public string[] ComboboxSource = {"Ready", "Needs Service", "In Service"};

        public AssetDetailsWindow(AssetDto selectedAssetDto)
        {
            InitializeComponent();
            SelectedAssetDto = selectedAssetDto;
            RepairsList = new ObservableCollection<RepairDto>();
            StatusPicker.ItemsSource = ComboboxSource;
            Application.Current.Properties[Constants.ShouldAssetDetailsRefresh] = true;
        }

        private void FillTheUi()
        {
            using (var dbContext = new DatabaseContext())
            {
                try
                {
                    SelectedAsset = dbContext.Assets.Where(x =>
                        x.AssetId == SelectedAssetDto.AssetId && x.AssetNumber == SelectedAssetDto.AssetNumber).First();
                    foreach (var rep in dbContext.Repair.Where(x => x.AssetId == SelectedAsset.Id)
                        .OrderByDescending(x => x.RepairDate).ToList()) RepairsList.Add(new RepairDto(rep));
                    RepairGrid.ItemsSource = RepairsList;
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
                    RepairsList.RemoveAt(RepairsList.Count-1);
                    //History = new ObservableCollection<Repositions>(SelectedAsset.Repositions);
                    //foreach (var rep in SelectedAsset.Repairs) Repairs.Add(new RepairDto(rep));
                    MessageBox.Show("Asset Deleted", "Done");
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    MessageBox.Show("Error Deleting Asset");
                }
            }
        }

        private void AssetDetailsWindow_OnActivated(object sender, EventArgs e)
        {
            if (!(bool) Application.Current.Properties[Constants.ShouldAssetDetailsRefresh]) return;
            FillTheUi();
        }
    }
}