using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Kotomi.Models.Library;
using LiteDB;
using System;
using System.IO;

namespace Kotomi.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        [ObservableProperty]
        private PageViewModelBase? selectedView;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SafeAreaLeft))]
        [NotifyPropertyChangedFor(nameof(SafeAreaTop))]
        [NotifyPropertyChangedFor(nameof(SafeAreaRight))]
        [NotifyPropertyChangedFor(nameof(SafeAreaBottom))]
        [NotifyPropertyChangedFor(nameof(SafeAreaLeftTopRight))]
        [NotifyPropertyChangedFor(nameof(SafeAreaLeftBottomRight))]
        [NotifyPropertyChangedFor(nameof(SafeAreaTopRight))]
        [NotifyPropertyChangedFor(nameof(SafeAreaLeftRight))]
        private Thickness safeArea = new(0);

        // TODO: this is really not elegant, need to figure out a better way to do this
        public Thickness SafeAreaTop => new(0, SafeArea.Top, 0, 0);
        public Thickness SafeAreaLeft => new(SafeArea.Left, 0, 0, 0);
        public Thickness SafeAreaBottom => new(0, 0, 0, SafeArea.Bottom);
        public Thickness SafeAreaRight => new(0, 0, SafeArea.Right, 0);
        public Thickness SafeAreaLeftTopRight => new(SafeArea.Left, SafeArea.Top, SafeArea.Right, 0);
        public Thickness SafeAreaLeftBottomRight => new(SafeArea.Left, 0, SafeArea.Right, SafeArea.Bottom);
        public Thickness SafeAreaTopRight => new(0, SafeArea.Top, SafeArea.Right, 0);
        public Thickness SafeAreaLeftRight => new(SafeArea.Left, 0, SafeArea.Right, 0);

        public Library Library { get; private set; }

        public MainViewModel()
        {
            if (!Design.IsDesignMode)
            {
                var dataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Squidhouse Software", "Kotomi");
                Directory.CreateDirectory(dataFolderPath);

                var database = new LiteDatabase($"Filename=\"{Path.Combine(dataFolderPath, "database.kdb1")}\";Connection=shared"); ;
                Library = new Library(database);
            }
            

            NavigateTo(new LibraryViewModel());
        }

        public void NavigateTo(PageViewModelBase page)
        {
            page.MainView = this;
            SelectedView = page;
        }
    }
}