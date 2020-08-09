using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Assets.Models.Dtos;

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
    }
}