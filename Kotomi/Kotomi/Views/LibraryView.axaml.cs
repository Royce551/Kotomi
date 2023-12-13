using Avalonia.Controls;
using Kotomi.ViewModels;
using System;

namespace Kotomi.Views
{
    public partial class LibraryView : UserControl
    {
        public LibraryView()
        {
            InitializeComponent();
        }

        protected override void OnDataContextChanged(EventArgs e)
        {
            (DataContext as LibraryViewModel)?.UpdateLibrary();
            base.OnDataContextChanged(e);
        }

        private async void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var storage = TopLevel.GetTopLevel(this)!.StorageProvider;
            var folder = await storage.OpenFolderPickerAsync(new Avalonia.Platform.Storage.FolderPickerOpenOptions { AllowMultiple = false });
            (DataContext as LibraryViewModel)?.Import(folder[0].Path.LocalPath);
        }
    }
}
