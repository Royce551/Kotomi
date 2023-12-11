using Avalonia.Controls;
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

        public Control CurrentPage => new Image()
        {
            Source = CurrentChapter.GetPage(page)
        };

        public IChapter CurrentChapter => series.Chapters[SelectedChapterIndex];

        public void SwitchToLibraryView() => MainView.NavigateTo(new LibraryViewModel());

        public void PageLeft()
        {
            Page--;
        }

        public void PageRight()
        {
            Page++;
        }

        [ObservableProperty]
        private bool isMenuBarShown = true;

        public void ShowMenuBar() => IsMenuBarShown = true;
    }
}
