using Avalonia.Controls;
using Kotomi.ViewModels;

namespace Kotomi.Views
{
    public partial class LibraryView : UserControl
    {
        public LibraryView()
        {
            InitializeComponent();
        }

        private async void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var storage = TopLevel.GetTopLevel(this)!.StorageProvider;
            var folder = await storage.OpenFolderPickerAsync(new Avalonia.Platform.Storage.FolderPickerOpenOptions { AllowMultiple = false });
            (DataContext as LibraryViewModel)?.OpenReader(folder[0].Path.LocalPath);
        }
    }
}
