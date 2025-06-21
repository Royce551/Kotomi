using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Converters;
using CommunityToolkit.Mvvm.ComponentModel;
using Kotomi.Models.Configuration;
using Kotomi.Models.Library;
using Kotomi.ViewModels.Reader;
using Realms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Kotomi.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNavbarVisible))]
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

        public bool IsNavbarVisible => SelectedView is not ReaderViewModel;

        private string? windowTitleOverride;
        public string? WindowTitleOverride
        {
            get => windowTitleOverride;
            set
            {
                windowTitleOverride = value;
                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime && desktopLifetime.MainWindow != null)
                    if (string.IsNullOrEmpty(windowTitleOverride)) desktopLifetime.MainWindow.Title = "Kotomi";
                    else desktopLifetime.MainWindow.Title = windowTitleOverride;
            }
        }

        public Realm Realm { get; private set; } = default!;

        public ConfigurationFile Config { get; private set; } = default!; // will not be null when app is actually running

        public MainViewModel()
        {
            if (!Design.IsDesignMode)
            {
                var dataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Squidhouse Software", "Kotomi");
                Directory.CreateDirectory(dataFolderPath);

                var realmConfig = new RealmConfiguration(Path.Combine(dataFolderPath, "library.realm"));
                Realm = Realm.GetInstance(realmConfig);

                Config = ConfigurationFile.Read(dataFolderPath);
            }  

            NavigateTo(new LibraryViewModel());
        }

        public void HandleAppClosing()
        {
            var dataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Squidhouse Software", "Kotomi");

            Realm.Dispose();
            Config.Save(dataFolderPath);
        }

        public void NavigateTo(PageViewModelBase page)
        {
            SelectedView?.OnNavigatingAway();

            page.MainView = this;
            SelectedView = page;
            page.AfterPageLoaded();
        }
    }

    public class CombineMarginsConverter : IMultiValueConverter
    {
        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (!values.All(x => x is Thickness)) throw new NotSupportedException();

            var valuesAsThickness = values.OfType<Thickness>();

            var y = new Thickness(valuesAsThickness.Sum(x => x.Left),
                                 valuesAsThickness.Sum(x => x.Top),
                                 valuesAsThickness.Sum(x => x.Right),
                                 valuesAsThickness.Sum(x => x.Bottom));
            return y;
        }
    }
}