using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Input;
using Assets.Helpers;
using Assets.Models.Dtos;
using Microsoft.Office.Interop.Excel;
using Application = Microsoft.Office.Interop.Excel.Application;
using DataTable = System.Data.DataTable;
using Window = System.Windows.Window;

namespace Assets.Views
{
    /// <summary>
    ///     Interaction logic for ShowExpiryNotificationWindow.xaml
    /// </summary>
    public partial class ShowExpiryNotificationWindow : Window
    {
        public ObservableCollection<AssetDto> AssetGridDataSource { get; set; }

        public ShowExpiryNotificationWindow(ObservableCollection<AssetDto> expAssetDtos)
        {
            InitializeComponent();
            AssetGridDataSource = expAssetDtos;
            AssetsDataGrid.ItemsSource = AssetGridDataSource;
        }

        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
        {
            var window = new AssetDetailsWindow(AssetGridDataSource[AssetsDataGrid.SelectedIndex]);
            window.Show();
        }

        private void ExcelBTN_OnClick(object sender, RoutedEventArgs e)
        {
            string customExcelSavingPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" +
                                           "AssetsReport.xlsx";
            GenerateExcel(ExcelHelpers.ConvertToDataTable(new List<AssetDto>(AssetGridDataSource)),
                customExcelSavingPath);
        }

        public static void GenerateExcel(DataTable dataTable, string path)
        {
            var dataSet = new DataSet();
            dataSet.Tables.Add(dataTable);

            // create a excel app along side with workbook and worksheet and give a name to it  
            var excelApp = new Application();
            Workbook excelWorkBook = excelApp.Workbooks.Add();
            _Worksheet xlWorksheet = excelWorkBook.Sheets[1];
            Range xlRange = xlWorksheet.UsedRange;
            foreach (DataTable table in dataSet.Tables)
            {
                //Add a new worksheet to workbook with the Datatable name  
                Worksheet excelWorkSheet = excelWorkBook.Sheets.Add();
                excelWorkSheet.Name = table.TableName;

                // add all the columns  
                for (int i = 1; i < table.Columns.Count + 1; i++)
                    excelWorkSheet.Cells[1, i] = table.Columns[i - 1].ColumnName;

                // add all the rows  
                for (int j = 0; j < table.Rows.Count; j++)
                for (int k = 0; k < table.Columns.Count; k++)
                    excelWorkSheet.Cells[j + 2, k + 1] = table.Rows[j].ItemArray[k].ToString();
            }

            // excelWorkBook.Save(); -> this is save to its default location  
            excelWorkBook.SaveAs(path); // -> this will do the custom  
            excelWorkBook.Close();
            excelApp.Quit();
        }
    }
}