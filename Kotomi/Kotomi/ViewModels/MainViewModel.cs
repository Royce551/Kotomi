using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Kotomi.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string greeting = "FRESHMusicPlayer, Kotomi";

        [ObservableProperty]
        private Thickness safeArea = new(0);
    }
}