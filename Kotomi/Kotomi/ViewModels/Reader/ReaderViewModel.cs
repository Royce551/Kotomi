using Avalonia;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Kotomi.Models.Configuration;
using Kotomi.Models.Series;
using Kotomi.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kotomi.ViewModels.Reader
{
    public partial class ReaderViewModel : PageViewModelBase, IRecipient<PropertyChangedMessage<double>>, IRecipient<PropertyChangedMessage<bool>>
    {
        [ObservableProperty]
        private ISeries series;

        public SeriesCachingContext Cache { get; private set; } = default!;

        public ReaderViewModel(ISeries series, int initialChapterIndex = 0, SeriesCachingContext? cache = null)
        {
            this.series = series;
            if (cache != null) Cache = cache;
            SelectedChapterIndex = initialChapterIndex;    
        }

        public override void AfterPageLoaded()
        {
            if (Cache is null) Cache = new SeriesCachingContext(MainView.Config);
            MainView.WindowTitleOverride = $"{Series.Title} - Kotomi";
            base.AfterPageLoaded();
        }

        public override void OnNavigatingAway()
        {
            base.OnNavigatingAway();
        }

        public decimal? Volume => CurrentChapter.VolumeNumber;
        public bool ShowVolume => Volume != null;

        public decimal Chapter => CurrentChapter.ChapterNumber;

        public int SelectedChapterIndex
        {
            get => selectedChapterIndex;
            set
            {
                if (SetProperty(ref selectedChapterIndex, value))
                {
                    Page = 1;

                    OnPropertyChanged(nameof(CurrentPage));
                    OnPropertyChanged(nameof(CurrentChapter));
                    OnPropertyChanged(nameof(Chapter));
                    OnPropertyChanged(nameof(Volume));
                    OnPropertyChanged(nameof(ShowVolume));    
                }
            }
        }
        private int selectedChapterIndex = 0;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CurrentPage))]
        private int page = 1;

        [ObservableProperty]
        private int secondPage; // the 2nd page when reading mode is set to Two Pages

        public void Receive(PropertyChangedMessage<double> message)
        {
            if (message is { Sender: ConfigurationFile, PropertyName: nameof(ConfigurationFile.ReadingModeLongMargin) })
                OnPropertyChanged(nameof(ReadingModeLongMarginAsThickness));
        }

        public Thickness ReadingModeLongMarginAsThickness => new Thickness(MainView.Config.ReadingModeLongMargin, 0);

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CurrentPageOpacity))]
        private bool currentPageIsReady = false;

        public double CurrentPageOpacity => CurrentPageIsReady ? 1 : 0;

        private CancellationTokenSource cancellationTokenSource;

        public ViewModelBase CurrentPage
        {
            get
            {
                CurrentPageIsReady = true;

                cancellationTokenSource?.Cancel();
                cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = cancellationTokenSource.Token;

                if (MainView.Config.ReadingModeSingle)
                {
                    var page = new SinglePageViewModel(this);
                    page.LoadImages(cancellationToken);
                    return page;
                }
                else if (MainView.Config.ReadingModeTwo)
                {
                    if (Page % 2 == 0) Page--; // If the page is not odd, go back to the previous page so that the 2nd page will be even
                    SecondPage = Page + 1;

                    var page = new TwoPageViewModel(this);
                    page.LoadImages(cancellationToken);
                    return page;
                }
                else
                {
                    var page = new LongPageViewModel(this);
                    page.LoadImages(cancellationToken);
                    return page;
                }
            }
        }

        public IChapter CurrentChapter => Series.Chapters[SelectedChapterIndex];

        public void SwitchToLibraryView() => MainView.NavigateTo(new LibraryViewModel());

        public bool ShowAlternativeControls => MainView.Config.ReadingModeLong || Series.IsInteractive;

        public void Receive(PropertyChangedMessage<bool> message)
        {
            if (message is { Sender: ConfigurationFile, PropertyName: nameof(ConfigurationFile.ReadingModeSingle) }
                        or { Sender: ConfigurationFile, PropertyName: nameof(ConfigurationFile.ReadingModeTwo)}
                        or { Sender: ConfigurationFile, PropertyName: nameof(ConfigurationFile.ReadingModeLong)}
                        or { Sender: ConfigurationFile, PropertyName: nameof(ConfigurationFile.ReadingDirectionLeftToRight)}
                        or { Sender: ConfigurationFile, PropertyName: nameof(ConfigurationFile.ReadingDirectionRightToLeft) })
                OnPropertyChanged(nameof(CurrentPage));
            if (message is { Sender: ConfigurationFile, PropertyName: nameof(ConfigurationFile.ReadingModeLong) })
                OnPropertyChanged(nameof(ShowAlternativeControls));
        }

        public void PageLeft()
        {
            var pagesToTurn = MainView.Config.ReadingModeSingle ? 1 : 2;

            if (MainView.Config.ReadingDirectionLeftToRight)
            {
                if (MainView.Config.ReadingModeLong) PreviousChapter();
                else if (Page - pagesToTurn >= 1)
                    Page -= pagesToTurn;
                else PreviousChapter();
            }
            else
            {
                if (MainView.Config.ReadingModeLong) NextChapter();
                else if (Page + pagesToTurn <= CurrentChapter.TotalPages)
                    Page += pagesToTurn;
                else NextChapter();
            }
        }
        public void PageRight()
        {
            var pagesToTurn = MainView.Config.ReadingModeSingle ? 1 : 2;

            if (MainView.Config.ReadingDirectionLeftToRight)
            {
                if (MainView.Config.ReadingModeLong) NextChapter();
                else if (Page + pagesToTurn <= CurrentChapter.TotalPages)
                    Page += pagesToTurn;
                else NextChapter();
            }
            else
            {
                if (MainView.Config.ReadingModeLong) PreviousChapter();
                else if (Page - pagesToTurn >= 1)
                    Page -= pagesToTurn;
                else PreviousChapter();
            }
        }

        public void NextChapter()
        {
            if (SelectedChapterIndex + 2 <= Series.Chapters.Length)
                SelectedChapterIndex++;
        }
        public void PreviousChapter()
        {
            if (SelectedChapterIndex >= 1)
            {
                SelectedChapterIndex--;
                Page = CurrentChapter.TotalPages; // Flip to last page of chapter after chapter has been switched
            }
        }

        [ObservableProperty]
        private bool isMenuBarShown = true;

        public void ShowHideMenuBar() => IsMenuBarShown = !IsMenuBarShown;
    }
}
