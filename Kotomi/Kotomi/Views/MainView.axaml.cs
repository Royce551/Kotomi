using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Platform.Storage;
using Kotomi.ViewModels;
using System;
using System.Threading.Tasks;

namespace Kotomi.Views
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            DataContext = new MainViewModel();
            Unloaded += MainView_Unloaded;
            InitializeComponent();
        }

        private void MainView_Unloaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            (DataContext as MainViewModel)!.HandleAppClosing();
        }

        private IInsetsManager? insetsManager;

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            insetsManager = TopLevel.GetTopLevel(this)!.InsetsManager;
            if (insetsManager != null)
            {
                insetsManager.DisplayEdgeToEdge = true;
                insetsManager.SafeAreaChanged += InsetsManager_SafeAreaChanged;
                (DataContext as MainViewModel)!.SafeArea = insetsManager.SafeAreaPadding;
            }
        }

        private void InsetsManager_SafeAreaChanged(object? sender, Avalonia.Controls.Platform.SafeAreaChangedArgs e)
        {
            if (insetsManager is not null)
            {
                (DataContext as MainViewModel)!.SafeArea = insetsManager.SafeAreaPadding;
            }
        }

        public async Task AnimateSidePaneInAsync(double width)
        {
            var animation = (Animation)Resources["RightSidePaneIn450"];
            await animation.RunAsync(SidePaneControl);
        }

        public async Task AnimateSidePaneOutAsync()
        {
            var animation = (Animation)Resources["RightSidePaneOut450"];
            await animation.RunAsync(SidePaneControl);
        }
    }
}