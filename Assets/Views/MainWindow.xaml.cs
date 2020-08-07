using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
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
        public ObservableCollection<AssetDto> AssetGridDataSource { get; set; }
        public bool ShouldRefresh { get; set; }
        public DateTime LastRefreshed { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            AssetGridDataSource = new ObservableCollection<AssetDto>();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void MainWindow_OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (!ShouldRefresh) return;
            Refresh();
        }

        private void AddAssetBTN_OnClick(object sender, RoutedEventArgs e)
        {
            AssetAddingWindow addingWindow = new AssetAddingWindow();
            addingWindow.Show();
        }

        private void NotifyBTN_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SearchBTN_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
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
                        MessageBox.Show("There are no Assets in the database");
                        return;
                    }

                    AssetGridDataSource.Clear();
                    foreach (Asset asset in db) AssetGridDataSource.Add(new AssetDto(asset));
                    AssetsDataGrid.ItemsSource = AssetGridDataSource;
                    LastRefreshed = DateTime.Now;
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