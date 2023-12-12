using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Kotomi.Models.Series;
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

        public ReaderViewModel(ISeries series)
        {
            this.series = series;
        }

        public int Chapter => selectedChapterIndex + 1;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CurrentPage))]
        [NotifyPropertyChangedFor(nameof(CurrentChapter))]
        [NotifyPropertyChangedFor(nameof(Chapter))]
        private int selectedChapterIndex = 0;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CurrentPage))]
        private int page = 1;

        // TODO: yikes! UI code in the VM layer? not ideal at all
        public Control? CurrentPage
        {
            get
            {
                if (ReadingModeSingle) return new Image() { Source = CurrentChapter.GetPage(page) };
                if (ReadingModeTwo)
                {
                    var grid = new Grid() { ColumnDefinitions = new("*,*") };
                    var leftImage = new Image()
                    {
                        Source = CurrentChapter.GetPage(page),
                    };
                    if (ReadingDirectionLeftToRight) Grid.SetColumn(leftImage, 0);
                    else Grid.SetColumn(leftImage, 1);
                    grid.Children.Add(leftImage);

                    var rightImage = new Image()
                    {
                        Source = CurrentChapter.GetPage(page + 1),
                    };
                    if (ReadingDirectionLeftToRight) Grid.SetColumn(rightImage, 1);
                    else Grid.SetColumn(rightImage, 0);
                    grid.Children.Add(rightImage);
                    return grid;
                }
                if (ReadingModeLong)
                {
                    var scrollViewer = new ScrollViewer();
                    var stackPanel = new StackPanel();
                    for (int i = 1; i < CurrentChapter.TotalPages; i++)
                    {
                        var imageSource = CurrentChapter.GetPage(i);
                        stackPanel.Children.Add(new Image() { Source = imageSource, MaxHeight = imageSource.Size.Height, Margin = new(0, 5) });
                    }
                    scrollViewer.Content = stackPanel;
                    return scrollViewer;
                }
                else return null;
            }
        }

        public IChapter CurrentChapter => series.Chapters[SelectedChapterIndex];

        public void SwitchToLibraryView() => MainView.NavigateTo(new LibraryViewModel());

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CurrentPage))]
        private bool readingModeSingle = true;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CurrentPage))]
        private bool readingModeTwo;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CurrentPage))]
        private bool readingModeLong;

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
                
                if (Page - pagesToTurn >= 1)
                    Page -= pagesToTurn;
            }
            else
            {
                if (Page + pagesToTurn <= CurrentChapter.TotalPages)
                    Page += pagesToTurn;
            }
        }

        public void PageRight()
        {
            var pagesToTurn = ReadingModeSingle ? 1 : 2;

            if (ReadingDirectionLeftToRight)
            {

                if (Page + pagesToTurn <= CurrentChapter.TotalPages)
                    Page += pagesToTurn;
            }
            else
            {
                if (Page - pagesToTurn >= 1)
                    Page -= pagesToTurn;
            }
        }

        [ObservableProperty]
        private bool isMenuBarShown = true;

        public void ShowHideMenuBar() => IsMenuBarShown = !IsMenuBarShown;
    }
}
