using System;
using System.Windows;
using Assets.Helpers;
using Assets.Models;
using Assets.Models.DataModels;

namespace Assets.Views
{
    /// <summary>
    ///     Interaction logic for RepairAddingWindow.xaml
    /// </summary>
    public partial class RepairAddingWindow : Window
    {
        public RepairAddingWindow(int assetId, string callerWindow)
        {
            InitializeComponent();
            AssetId = assetId;
            CallerWindow = callerWindow;
        }

        public int AssetId { get; set; }
        public string CallerWindow { get; set; }

        private void SaveBTN_OnClick(object sender, RoutedEventArgs e)
        {
            var pendingRepair = new Repair() { AssetId = AssetId };

            if (!string.IsNullOrWhiteSpace(RepairLocationBox.Text))
            {
                pendingRepair.Location = RepairLocationBox.Text.ToLower();
            }
            else
            {
                MessageBox.Show("Repair Location can't be empty");
                return;
            }

            if (!string.IsNullOrWhiteSpace(LaborCostBox.Text))
            {
                pendingRepair.LaborAmount = NumberHelpers.StringToDouble(LaborCostBox.Text);
            }
            else
            {
                MessageBox.Show("Labor Cost can't be empty");
                return;
            }

            if (!string.IsNullOrWhiteSpace(SparePartsCostBox.Text))
            {
                pendingRepair.SparePartsAmount = NumberHelpers.StringToDouble(SparePartsCostBox.Text);
            }
            else
            {
                MessageBox.Show("Spare Parts Cost can't be empty");
                return;
            }


            if (RepairDatePicker.SelectedDate.HasValue)
                pendingRepair.RepairDate = RepairDatePicker.SelectedDate.Value;
            else
            {
                MessageBox.Show("Repair Dates can't be empty");
                return;
            }

            using (var dbContext = new DatabaseContext())
            {
                try
                {
                    pendingRepair.AddedDate = DateTime.Now;
                    dbContext.Add(pendingRepair);
                    dbContext.SaveChanges();
                    if (pendingRepair.Id < 0)
                    {
                        MessageBox.Show("Error in Addition");
                    }
                    else
                    {
                        MessageBox.Show("Repair Added");
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
            RepairLocationBox.Text = string.Empty;
            LaborCostBox.Text = string.Empty;
            SparePartsCostBox.Text = string.Empty;
            RepairDatePicker.SelectedDate = null;
        }
    }
}