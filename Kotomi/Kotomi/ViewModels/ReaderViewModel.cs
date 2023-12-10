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
        private Series series;

        public ReaderViewModel(Series series)
        {
            this.series = series;
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CurrentPage))]
        private int chapter = 0;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CurrentPage))]
        private int page = 0;

        public Control CurrentPage => new Image()
        {
            Source = series.Chapters[chapter].Pages[page]
        };

        public void SwitchToLibraryView() => MainView.NavigateTo(new LibraryViewModel());

        public void PageLeft()
        {
            Page--;
        }

        public void PageRight()
        {
            Page++;
        }
    }
}
