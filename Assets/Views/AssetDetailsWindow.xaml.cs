using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
using Assets.Models;
using Assets.Models.DataModels;
using Assets.Models.Dtos;
using Microsoft.EntityFrameworkCore.Internal;

namespace Assets.Views
{
    /// <summary>
    /// Interaction logic for AssetDetailsWindow.xaml
    /// </summary>
    public partial class AssetDetailsWindow : Window
    {
        private AssetDto SelectedAssetDto { get; set; }
        public Asset SelectedAsset { get; set; }
        public string[] ComboboxSource = { "Ready", "Needs Service", "In Service" };

        public AssetDetailsWindow(AssetDto selectedAssetDto)
        {
            InitializeComponent();
            SelectedAssetDto = selectedAssetDto;
            StatusPicker.ItemsSource = ComboboxSource;
            FillTheUI();
        }

        private void FillTheUI()
        {
            using (DatabaseContext dbContext = new DatabaseContext())
            {
                try
                {
                    SelectedAsset = dbContext.Assets.Where(x => x.AssetId == SelectedAssetDto.AssetId && x.AssetNumber == SelectedAssetDto.AssetNumber).First();
                    //History = new ObservableCollection<Repositions>(SelectedAsset.Repositions);
                    //foreach (var rep in SelectedAsset.Repairs) Repairs.Add(new RepairDto(rep));
                    if (SelectedAsset == null)
                    {
                        MessageBox.Show("Invalid selected Asset", "Error");
                        return;
                    }
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
                    double netBookValue = SelectedAsset.PurchaseCostOfAsset - (SelectedAsset.PurchaseCostOfAsset / SelectedAsset.MonthsToDepreciation * (DateTime.Today.Month - SelectedAsset.DateOfPurchase.Month + 1));
                    netBookValue = Math.Round(netBookValue, 3);
                    NetBookValueBox.Text = netBookValue.ToString(CultureInfo.InvariantCulture);
                    if (netBookValue > 0.0)
                    {
                        double mon = SelectedAsset.PurchaseCostOfAsset / SelectedAsset.MonthsToDepreciation;
                        mon = Math.Round(mon, 3);
                        MonthlyDepreciationDueDateBox.Text = mon.ToString(CultureInfo.InvariantCulture);
                    }
                    else
                        MonthlyDepreciationDueDateBox.Text = 1.ToString();

                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    MessageBox.Show("Error Loading Assets ");
                }
            }
        }
    }
}
