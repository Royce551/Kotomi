using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Kotomi.ViewModels;

namespace Kotomi.Views
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            DataContext = new MainViewModel();
            InitializeComponent();
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
    }
}