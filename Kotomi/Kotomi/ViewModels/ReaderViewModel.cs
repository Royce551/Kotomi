using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Kotomi.Models.Series;
using Kotomi.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kotomi.ViewModels
{
    public partial class ReaderViewModel : PageViewModelBase
    {
        [ObservableProperty]
        private ISeries series;

        public ReaderViewModel(ISeries series, int initialChapterIndex = 0)
        {
            this.series = series;
            SelectedChapterIndex = initialChapterIndex;
        }

        public override void AfterPageLoaded()
        {
            MainView.WindowTitleOverride = $"{Series.Title} - Kotomi";
            base.AfterPageLoaded();
        }

        public decimal? Volume => CurrentChapter.VolumeNumber;
        public bool ShowVolume => Volume != null;

        public decimal Chapter => CurrentChapter.ChapterNumber is null ? SelectedChapterIndex + 1 : (decimal)CurrentChapter.ChapterNumber;

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

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(readingModeLongMarginAsThickness))]
        private double readingModeLongMargin = 0;

        private Thickness readingModeLongMarginAsThickness => new Thickness(ReadingModeLongMargin, 0);

        // TODO: yikes! UI code in the VM layer? not ideal at all
        public Control? CurrentPage
        {
            get
            {
                if (ReadingModeSingle)
                {
                    var page = CurrentChapter.GetPageAsControl(Page);

                    page.Bind(Layoutable.MarginProperty, new Binding { Source = MainView, Path = IsMenuBarShown ? nameof(MainView.SafeAreaLeftBottomRight) : nameof(MainView.SafeArea) });

                    return page;
                }
                
                if (ReadingModeTwo)
                {
                    if (Page % 2 == 0) Page--; // If the page is not odd, go back to the previous page so that the 2nd page will be even
                    SecondPage = Page + 1;

                    var grid = new Grid() { ColumnDefinitions = new("*,*") };
                    var leftPage = CurrentChapter.GetPageAsControl(Page);

                    leftPage.Bind(Layoutable.MarginProperty, new Binding
                    {
                        Source = MainView,
                        Path = nameof(MainView.SafeAreaLeft)
                    });

                    if (ReadingDirectionLeftToRight) Grid.SetColumn(leftPage, 0);
                    else Grid.SetColumn(leftPage, 1);
                    grid.Children.Add(leftPage);

                    if (SecondPage <= CurrentChapter.TotalPages)
                    {
                        var rightPage = CurrentChapter.GetPageAsControl(SecondPage);
                        if (ReadingDirectionLeftToRight) Grid.SetColumn(rightPage, 1);
                        else Grid.SetColumn(rightPage, 0);
                        grid.Children.Add(rightPage);
                    }
                   
                    grid.Bind(Layoutable.MarginProperty, new Binding { Source = MainView, Path = IsMenuBarShown ? nameof(MainView.SafeAreaLeftBottomRight) : nameof(MainView.SafeArea) });

                    return grid;
                }
                if (ReadingModeLong)
                {
                    var scrollViewer = new ScrollViewer();
                    var stackPanel = new StackPanel { Spacing = 5 };
                    for (int i = 1; i < CurrentChapter.TotalPages; i++)
                    {
                        var page = CurrentChapter.GetPageAsControl(i);

                        if (i == 1)
                        {
                            page.Bind(Layoutable.MarginProperty, new MultiBinding()
                            {
                                Converter = new CombineMarginsConverter(),
                                Bindings = [new Binding { Source = MainView, Path = nameof(MainView.SafeAreaLeftTopRight) },
                                new Binding{ Source = this, Path = nameof(readingModeLongMarginAsThickness)}]
                            });
                        }
                        else if (i == CurrentChapter.TotalPages)
                        {
                            page.Bind(Layoutable.MarginProperty, new MultiBinding()
                            {
                                Converter = new CombineMarginsConverter(),
                                Bindings = [new Binding { Source = MainView, Path = nameof(MainView.SafeAreaLeftBottomRight) },
                                new Binding{ Source = this, Path = nameof(readingModeLongMarginAsThickness)}]
                            });
                        }
                        else
                        {
                            page.Bind(Layoutable.MarginProperty, new MultiBinding()
                            {
                                Converter = new CombineMarginsConverter(),
                                Bindings = [new Binding { Source = MainView, Path = nameof(MainView.SafeAreaLeftRight) },
                                new Binding{ Source = this, Path = nameof(readingModeLongMarginAsThickness)}]
                            });
                        }

                        stackPanel.Children.Add(page);
                        
                    }
                    scrollViewer.Content = stackPanel;
                    return scrollViewer;
                }
                else return null;
            }
        }

        public IChapter CurrentChapter => Series.Chapters[SelectedChapterIndex];

        public void SwitchToLibraryView() => MainView.NavigateTo(new LibraryViewModel());

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CurrentPage))]
        private bool readingModeSingle = true;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CurrentPage))]
        private bool readingModeTwo;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CurrentPage))]
        [NotifyPropertyChangedFor(nameof(ShowAlternativeControls))]
        private bool readingModeLong;

        public bool ShowAlternativeControls => ReadingModeLong || Series.IsInteractive;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CurrentPage))]
        private bool readingDirectionLeftToRight;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CurrentPage))]
        private bool readingDirectionRightToLeft = true;
        
        public void PageLeft()
        {
            var pagesToTurn = ReadingModeSingle ? 1 : 2;

            if (ReadingDirectionLeftToRight)
            {
                if (ReadingModeLong) PreviousChapter();
                else if (Page - pagesToTurn >= 1)
                    Page -= pagesToTurn;
                else PreviousChapter();
            }
            else
            {
                if (ReadingModeLong) NextChapter();
                else if (Page + pagesToTurn <= CurrentChapter.TotalPages)
                    Page += pagesToTurn;
                else NextChapter();
            }
        }
        public void PageRight()
        {
            var pagesToTurn = ReadingModeSingle ? 1 : 2;

            if (ReadingDirectionLeftToRight)
            {
                if (ReadingModeLong) NextChapter();
                else if (Page + pagesToTurn <= CurrentChapter.TotalPages)
                    Page += pagesToTurn;
                else NextChapter();
            }
            else
            {
                if (ReadingModeLong) PreviousChapter();
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
