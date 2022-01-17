using System.Windows;
using ModernWpf.Controls;

namespace VRCTower
{
    /// <summary>
    ///     InviteDialog.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InviteDialog : ContentDialog
    {
        public bool UseCurrentLocation = true;

        public InviteDialog()
        {
            InitializeComponent();
            _worldId.IsEnabled = false;
            UseCurrentLocation = true;
        }

        private void _useCurrentLocation_OnCheckedChanged(object sender, RoutedEventArgs e)
        {
            UseCurrentLocation = !UseCurrentLocation;
            _worldId.IsEnabled = !_worldId.IsEnabled;
        }
    }
}