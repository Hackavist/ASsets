﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using Assets.Helpers;
using Assets.Models;
using Assets.Models.DataModels;
using Assets.Models.Dtos;

namespace Assets.Views
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string[] ComboboxStrings =
        {
            "Asset Id",
            "Asset Number",
            "Asset Name",
            "PMV Code",
            "Current Location",
            "Po Number",
            "Plate Serial Number",
            "Purchase Cost Of Asset",
            "Asset Status",
            "Tool Type",
            "Calibration Certification Number"
        };

        public ObservableCollection<AssetDto> AssetGridDataSource { get; set; }
        public ObservableCollection<AssetDto> ExpiredAssets { get; set; }
        public ObservableCollection<AssetDto> SearchResults { get; set; }
        public DateTime LastRefreshed { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            AssetGridDataSource = new ObservableCollection<AssetDto>();
            ExpiredAssets = new ObservableCollection<AssetDto>();
            SearchResults = new ObservableCollection<AssetDto>();
            PropertyPicker.ItemsSource = ComboboxStrings;
            Application.Current.Properties[Constants.ShouldMainWindowRefresh] = false;
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void MainWindow_OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (!(bool)Application.Current.Properties[Constants.ShouldMainWindowRefresh]) return;
            Refresh();
        }

        private void AddAssetBTN_OnClick(object sender, RoutedEventArgs e)
        {
            AssetAddingWindow addingWindow = new AssetAddingWindow();
            addingWindow.Show();
        }

        private void NotifyBTN_OnClick(object sender, RoutedEventArgs e)
        {
            using (DatabaseContext dbContext = new DatabaseContext())
            {
                try
                {
                    var db = dbContext.Assets.Where(x =>
                        x.CalibrationCertificationDate.Date >= DateTime.Today.Date &&
                        x.CalibrationCertificationDate.Date < DateTime.Today.Date.AddDays(10.0)).ToList();
                    ExpiredAssets.Clear();
                    foreach (Asset asset in db) ExpiredAssets.Add(new AssetDto(asset));
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    MessageBox.Show("Error Loading Assets ");
                }
            }

            if (ExpiredAssets.Count > 0)
            {
                ShowExpiryNotificationWindow window = new ShowExpiryNotificationWindow(ExpiredAssets);
                window.Show();
            }
            else
            {
                MessageBox.Show("There are no assets to expire this week");
            }
        }

        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
        {
            AssetDetailsWindow window = new AssetDetailsWindow(AssetGridDataSource[AssetsDataGrid.SelectedIndex]);
            window.Show();
        }

        private void SearchBTN_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(QueryTXT.Text))
            {
                MessageBox.Show("There is no text to search for");
                return;
            }

            string query = QueryTXT.Text;
            DateTime? toDate = DateTime.Today;
            DateTime? fromDate = DateTime.Today;
            if (FromDatePicker.SelectedDate == null)
                using (DatabaseContext dbContext = new DatabaseContext())
                {
                    try
                    {
                        Asset db = dbContext.Assets.OrderBy(x => x.DateOfPurchase).First();
                        fromDate = db.DateOfPurchase;
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                        MessageBox.Show("Error Loading Assets for date");
                    }
                }
            else
                fromDate = FromDatePicker.SelectedDate;

            if (ToDatePicker.SelectedDate != null) toDate = ToDatePicker.SelectedDate;
            //DateTime date2Compare = new DateTime(2017, 1, 20);
            //list.Where(x => myDateColumn >= date2Compare && x.myTextColumn.Contains('abc'));
            using (DatabaseContext dbContext = new DatabaseContext())
            {
                var res = new List<AssetDto>();
                try
                {
                    switch (PropertyPicker.SelectedIndex)
                    {
                        case 0:
                            res = AssetGridDataSource.Where(x =>
                                x.DateOfPurchase >= fromDate && x.DateOfPurchase <= toDate &&
                                x.AssetId.Contains(query)).ToList();
                            break;
                        case 1:
                            res = AssetGridDataSource.Where(x =>
                                x.DateOfPurchase >= fromDate && x.DateOfPurchase <= toDate &&
                                x.AssetNumber.Contains(query)).ToList();
                            break;
                        case 2:
                            res = AssetGridDataSource.Where(x =>
                                x.DateOfPurchase >= fromDate && x.DateOfPurchase <= toDate &&
                                x.AssetName.Contains(query)).ToList();
                            break;
                        case 3:
                            res = AssetGridDataSource.Where(x =>
                                x.DateOfPurchase >= fromDate && x.DateOfPurchase <= toDate &&
                                x.PMVCode.Contains(query)).ToList();
                            break;
                        case 4:
                            res = AssetGridDataSource.Where(x =>
                                x.DateOfPurchase >= fromDate && x.DateOfPurchase <= toDate &&
                                x.CurrentLocation.Contains(query)).ToList();
                            break;

                        case 5:
                            res = AssetGridDataSource.Where(x =>
                                x.DateOfPurchase >= fromDate && x.DateOfPurchase <= toDate &&
                                x.PoNumber.Contains(query)).ToList();
                            break;
                        case 6:
                            res = AssetGridDataSource.Where(x =>
                                x.DateOfPurchase >= fromDate && x.DateOfPurchase <= toDate &&
                                x.PlateSerialNumber.Contains(query)).ToList();
                            break;
                        case 7:
                            res = AssetGridDataSource.Where(x =>
                                x.DateOfPurchase >= fromDate && x.DateOfPurchase <= toDate &&
                                Math.Abs(x.PurchaseCostOfAsset - NumberHelpers.StringToDouble(query)) < 0.05).ToList();
                            break;

                        case 8:
                            res = AssetGridDataSource.Where(x =>
                                x.DateOfPurchase >= fromDate && x.DateOfPurchase <= toDate &&
                                x.AssetStatus.ToString().Contains(query)).ToList();
                            break;

                        case 9:
                            res = AssetGridDataSource.Where(x =>
                                x.DateOfPurchase >= fromDate && x.DateOfPurchase <= toDate &&
                                x.ToolType.Contains(query)).ToList();
                            break;
                        case 10:
                            res = AssetGridDataSource.Where(x =>
                                x.DateOfPurchase >= fromDate && x.DateOfPurchase <= toDate &&
                                x.CalibrationCertificationNumber.Contains(query)).ToList();
                            break;
                    }

                    if (res.Count > 0)
                    {
                        ShowExpiryNotificationWindow window =
                            new ShowExpiryNotificationWindow(new ObservableCollection<AssetDto>(res));
                        window.Show();
                    }
                    else
                    {
                        MessageBox.Show("No results were found");
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    MessageBox.Show("Error Loading Assets for date");
                }

                QueryTXT.Text = string.Empty;
            }
        }

        private void Refresh()
        {
            using (DatabaseContext dbContext = new DatabaseContext())
            {
                try
                {
                    var db = dbContext.Assets.Where(x => x.Id > 0).ToList();
                    if (db.Count <= 0)
                    {
                        AssetGridDataSource.Clear();
                        MessageBox.Show("There are no Assets in the database");
                        return;
                    }

                    AssetGridDataSource.Clear();
                    foreach (Asset asset in db)
                    {
                        if (asset.CurrentLocation == null)
                        {
                            string location = dbContext.Repositions.Where(x => x.AssetId == asset.Id)
                                .OrderByDescending(x => x.AddedDate).FirstOrDefault()
                                ?.NewPosition;

                            if (string.IsNullOrEmpty(location)) asset.CurrentLocation = location;
                        }

                        AssetGridDataSource.Add(new AssetDto(asset));
                    }

                    AssetsDataGrid.ItemsSource = AssetGridDataSource;
                    LastRefreshed = DateTime.Now;
                    Application.Current.Properties[Constants.ShouldMainWindowRefresh] = false;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    MessageBox.Show("Error Loading Assets ");
                }
            }
        }

        private void MainWindow_OnActivated(object sender, EventArgs e)
        {
            if (!(bool)Application.Current.Properties[Constants.ShouldMainWindowRefresh]) return;
            Refresh();
        }

        private void RefreshBTN_OnClick(object sender, RoutedEventArgs e)
        {
            Refresh();
        }
    }
}