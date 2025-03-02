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

        public string? Description => Series.Description;
        public bool ShowDescription => Description != null;

        public string? Genres => Series.Genres != null ? string.Join(", ", Series.Genres) : string.Empty;
        public bool ShowGenres => !string.IsNullOrEmpty(Genres);

        public string? Tags => Series.Tags != null ? string.Join(", ", Series.Tags) : string.Empty;
        public bool ShowTags => !string.IsNullOrEmpty(Tags);

        public string? Demographic => Series.Demographic;
        public bool ShowDemographic => Demographic != null;

        public string Source => Series.Source;

        [ObservableProperty]
        private ObservableCollection<ChapterViewModel> allChapters = new();

        private PageViewModelBase previousPage;

        public SeriesCachingContext Cache { get; }

        public BookInfoViewModel(ISeries series, PageViewModelBase previousPage)
        {
            Series = series;
            this.previousPage = previousPage;
            Cache = new SeriesCachingContext();

            foreach (var chapter in series.Chapters)
            {
                allChapters.Add(new ChapterViewModel(this, series, chapter));
            }
        }

        public override void AfterPageLoaded()
        {
            MainView.WindowTitleOverride = $"{Series.Title} - Kotomi";
            base.AfterPageLoaded();
        }

        public void Back() => MainView.NavigateTo(previousPage);

        public void StartReading() => MainView.NavigateTo(new ReaderViewModel(Series));

        public void AddRemoveLibrary()
        {
            MainView.Config.ReadingModeLongMargin = 100;
        }
    }

    public class ChapterViewModel
    {
        public string? Title { get; }
        public int? TotalPages { get; }
        public decimal? VolumeNumber { get; }
        public decimal? ChapterNumber { get; }

        private BookInfoViewModel bookInfo;
        private ISeries series;
        private IChapter chapter;
        public ChapterViewModel(BookInfoViewModel bookInfo, ISeries series, IChapter chapter)
        {
            Title = chapter.Title;
            TotalPages = chapter.TotalPages;
            VolumeNumber = chapter.VolumeNumber;
            ChapterNumber = chapter.ChapterNumber;
            this.series = series;
            this.bookInfo = bookInfo;
            this.chapter = chapter;
        }

        public void Open() => bookInfo.MainView.NavigateTo(new ReaderViewModel(series, series.Chapters.ToList().FindIndex(x => x == chapter), bookInfo.Cache));
    }
}
