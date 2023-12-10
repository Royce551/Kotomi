using Avalonia;
using Avalonia.Controls;
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

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            var insetsManager = TopLevel.GetTopLevel(this)!.InsetsManager;
            if (insetsManager != null)
            {
                (DataContext as MainViewModel)!.SafeArea = insetsManager.SafeAreaPadding;
            }
        }
    }
}