﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Assets.Helpers;
using Assets.Models;
using Assets.Models.DataModels;

namespace Assets.Views
{
    /// <summary>
    /// Interaction logic for HistoryAddingWindow.xaml
    /// </summary>
    public partial class HistoryAddingWindow : Window
    {
        public int AssetId { get; set; }
        public string CallerWindow { get; set; }
        public HistoryAddingWindow(int assetId, string callerWindow)
        {
            InitializeComponent();
            AssetId = assetId;
            CallerWindow = callerWindow;
        }

        private void SaveBTN_OnClick(object sender, RoutedEventArgs e)
        {
            var pendingHistory = new Repositions { AssetId = AssetId };

            if (!string.IsNullOrWhiteSpace(OldLocationBox.Text))
                pendingHistory.OldPosition = OldLocationBox.Text;
            else
            {
                MessageBox.Show("Old Location can't be empty");
                return;
            }
            if (!string.IsNullOrWhiteSpace(NewLocationBox.Text))
                pendingHistory.NewPosition = NewLocationBox.Text;
            else
            {
                MessageBox.Show("New Location can't be empty");
                return;
            }

            using (DatabaseContext dbContext = new DatabaseContext())
            {
                try
                {
                    pendingHistory.AddedDate = DateTime.Now;
                    dbContext.Add(pendingHistory);
                    dbContext.SaveChanges();
                    if (pendingHistory.Id < 0)
                    {
                        MessageBox.Show("Error in Addition");
                    }
                    else
                    {
                        var asset = dbContext.Assets.Where(x => x.Id == AssetId).First();
                        asset.CurrentLocation = pendingHistory.NewPosition;
                        dbContext.Update(pendingHistory);
                        dbContext.SaveChanges();
                        MessageBox.Show("History Added");
                        ClearBoxes();
                        SetCallerWindowRefresh();
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    MessageBox.Show("Error in Addition");
                }
            }
        }

        private void SetCallerWindowRefresh()
        {
            switch (CallerWindow)
            {
                case Constants.AssetDetailsWindow:
                    Application.Current.Properties[Constants.ShouldAssetDetailsRefresh] = true;
                    break;
            }
        }

        private void ClearBoxes()
        {
            OldLocationBox.Text = string.Empty;
            NewLocationBox.Text = string.Empty;
        }
    }
}
