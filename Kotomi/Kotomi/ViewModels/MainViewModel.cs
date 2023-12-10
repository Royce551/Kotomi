using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Kotomi.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        [ObservableProperty]
        private PageViewModelBase? selectedView;

        [ObservableProperty]
        private Thickness safeArea = new(0);

        public MainViewModel()
        {
            SelectedView = new LibraryViewModel(this);
        }
    }
}