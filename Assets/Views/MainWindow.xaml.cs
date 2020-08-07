using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Assets.Helpers;
using Assets.Models;
using Assets.Models.Dtos;

namespace Assets.Views
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AssetGridDataSource = new ObservableCollection<AssetDto>();
            Application.Current.Properties[Constants.ShouldMainWindowRefresh] = false;
        }

        public ObservableCollection<AssetDto> AssetGridDataSource { get; set; }
        public DateTime LastRefreshed { get; set; }

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
            var addingWindow = new AssetAddingWindow();
            addingWindow.Show();
        }

        private void NotifyBTN_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
        {
            var window = new AssetDetailsWindow(AssetGridDataSource[AssetsDataGrid.SelectedIndex]);
            window.Show();
        }

        private void SearchBTN_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Refresh()
        {
            using (var dbContext = new DatabaseContext())
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
                    foreach (var asset in db) AssetGridDataSource.Add(new AssetDto(asset));
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
    }
}