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
using System.Threading.Tasks;

namespace Kotomi.ViewModels
{
    public partial class ReaderViewModel : PageViewModelBase, IRecipient<PropertyChangedMessage<double>>, IRecipient<PropertyChangedMessage<bool>>
    {
        [ObservableProperty]
        private ISeries series;

        private SeriesCachingContext? cache;

        public ReaderViewModel(ISeries series, int initialChapterIndex = 0, SeriesCachingContext? cache = null)
        {
            this.series = series;
            this.cache = cache;
            SelectedChapterIndex = initialChapterIndex;    
        }

        public override void AfterPageLoaded()
        {
            if (cache is null) cache = new SeriesCachingContext(MainView.Config);
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

        private Thickness ReadingModeLongMarginAsThickness => new Thickness(MainView.Config.ReadingModeLongMargin, 0);

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CurrentPageOpacity))]
        private bool currentPageIsReady = false;

        public double CurrentPageOpacity => CurrentPageIsReady ? 1 : 0;

        // TODO: yikes! UI code in the VM layer? not ideal at all
        private async Task<Control> GetNextPageAsync()
        {
            CurrentPageIsReady = false;
            if (MainView.Config.ReadingModeSingle)
            {
                var page = await CurrentChapter.GetPageAsControlAsync(Page, cache);

                page.Bind(Layoutable.MarginProperty, new Binding { Source = MainView, Path = IsMenuBarShown ? nameof(MainView.SafeAreaLeftBottomRight) : nameof(MainView.SafeArea) });

                CurrentPageIsReady = true;
                return page;
            }

            if (MainView.Config.ReadingModeTwo)
            {
                if (Page % 2 == 0) Page--; // If the page is not odd, go back to the previous page so that the 2nd page will be even
                SecondPage = Page + 1;

                var grid = new Grid() { ColumnDefinitions = new("*,*") };
                var leftPage = await CurrentChapter.GetPageAsControlAsync(Page, cache);

                leftPage.Bind(Layoutable.MarginProperty, new Binding
                {
                    Source = MainView,
                    Path = nameof(MainView.SafeAreaLeft)
                });

                if (MainView.Config.ReadingDirectionLeftToRight) Grid.SetColumn(leftPage, 0);
                else Grid.SetColumn(leftPage, 1);
                grid.Children.Add(leftPage);

                if (SecondPage <= CurrentChapter.TotalPages)
                {
                    var rightPage = await CurrentChapter.GetPageAsControlAsync(SecondPage, cache);
                    if (MainView.Config.ReadingDirectionLeftToRight) Grid.SetColumn(rightPage, 1);
                    else Grid.SetColumn(rightPage, 0);
                    grid.Children.Add(rightPage);
                }

                grid.Bind(Layoutable.MarginProperty, new Binding { Source = MainView, Path = IsMenuBarShown ? nameof(MainView.SafeAreaLeftBottomRight) : nameof(MainView.SafeArea) });

                CurrentPageIsReady = true;
                return grid;
            }
            if (MainView.Config.ReadingModeLong)
            {
                var scrollViewer = new ScrollViewer();
                var stackPanel = new StackPanel { Spacing = 5 };
                for (int i = 1; i < CurrentChapter.TotalPages; i++)
                {
                    var page = await CurrentChapter.GetPageAsControlAsync(i, cache);

                    if (i == 1)
                    {
                        page.Bind(Layoutable.MarginProperty, new MultiBinding()
                        {
                            Converter = new CombineMarginsConverter(),
                            Bindings = [new Binding { Source = MainView, Path = nameof(MainView.SafeAreaLeftTopRight) },
                                new Binding{ Source = this, Path = nameof(ReadingModeLongMarginAsThickness)}]
                        });
                    }
                    else if (i == CurrentChapter.TotalPages)
                    {
                        page.Bind(Layoutable.MarginProperty, new MultiBinding()
                        {
                            Converter = new CombineMarginsConverter(),
                            Bindings = [new Binding { Source = MainView, Path = nameof(MainView.SafeAreaLeftBottomRight) },
                                new Binding{ Source = this, Path = nameof(ReadingModeLongMarginAsThickness)}]
                        });
                    }
                    else
                    {
                        page.Bind(Layoutable.MarginProperty, new MultiBinding()
                        {
                            Converter = new CombineMarginsConverter(),
                            Bindings = [new Binding { Source = MainView, Path = nameof(MainView.SafeAreaLeftRight) },
                                new Binding{ Source = this, Path = nameof(ReadingModeLongMarginAsThickness)}]
                        });
                    }

                    stackPanel.Children.Add(page);

                }

                scrollViewer.Content = stackPanel;
                CurrentPageIsReady = true;
                return scrollViewer;
            }
            else throw new Exception();
        }

        public Task<Control> CurrentPage => GetNextPageAsync();

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
