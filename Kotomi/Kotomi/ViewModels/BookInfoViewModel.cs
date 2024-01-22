using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using Kotomi.Models.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kotomi.ViewModels
{
    public partial class BookInfoViewModel : PageViewModelBase
    {
        public ISeries Series { get; }

        [ObservableProperty]
        private ObservableCollection<ChapterViewModel> allChapters = new();

        private PageViewModelBase previousPage;

        public BookInfoViewModel(ISeries series, PageViewModelBase previousPage)
        {
            Series = series;
            this.previousPage = previousPage;

            foreach (var chapter in series.Chapters)
            {
                allChapters.Add(new ChapterViewModel(this, series, chapter));
            }
        }

        public void Back() => MainView.NavigateTo(previousPage);

        public void StartReading() => MainView.NavigateTo(new ReaderViewModel(Series));

        public void AddRemoveLibrary()
        {

        }
    }

    public class ChapterViewModel
    {
        public string? Title { get; }
        public int? TotalPages { get; }

        private BookInfoViewModel bookInfo;
        private ISeries series;
        private IChapter chapter;
        public ChapterViewModel(BookInfoViewModel bookInfo, ISeries series, IChapter chapter)
        {
            Title = chapter.Title;
            TotalPages = chapter.TotalPages;
            this.series = series;
            this.bookInfo = bookInfo;
            this.chapter = chapter;
        }

        public void Open() => bookInfo.MainView.NavigateTo(new ReaderViewModel(series, series.Chapters.ToList().FindIndex(x => x == chapter)));
    }
}
