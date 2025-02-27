using Avalonia.Controls;
using Avalonia.Input;
using Kotomi.ViewModels;

namespace Kotomi.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            AddHandler(KeyDownEvent, OnPreviewKeyDown, Avalonia.Interactivity.RoutingStrategies.Tunnel);
        }

        void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Avalonia.Input.Key.Tab)
                ((Content as MainView).DataContext as MainViewModel).Config.ReadingModeLongMargin = 200;
        }
    }
}