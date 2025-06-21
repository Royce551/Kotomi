using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using Kotomi.Models.Library;
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
    public partial class LibraryViewModel : PageViewModelBase
    {
        //[ObservableProperty]
        //private ObservableCollection<SeriesViewModel> allSeries = new();
        public IEnumerable<SeriesViewModel> AllSeries => MainView.Realm.All<DatabaseSeries>().ToList().Select(x => new SeriesViewModel(this, x.ConcreteSeries));
        public LibraryViewModel()
        {
            
        }

        public override void AfterPageLoaded()
        {
            MainView.WindowTitleOverride = null;
        }

        public void Import(string url)
        {
            //MainView.Library.Import($"folder://{url}");
            MainView.Realm.Write(() => MainView.Realm.Add(new DatabaseSeries(SeriesLocator.GetSeriesForPrefixedURL($"folder://{url}"))));
            OnPropertyChanged(nameof(AllSeries));
        }
    }

    public class SeriesViewModel(LibraryViewModel _library, ISeries series) : ISeries
    {
        public string Source => series.Source;
        public string? Title => series.Title;
        public string? Author => series.Author;
        public byte[]? Cover => series.Cover;
        public string? URL => series.URL;
        public string? PrefixedURL => series.PrefixedURL;
        public IChapter[] Chapters => series.Chapters;
        public string? Description => series.Description;
        public string[]? Genres => series.Genres;
        public string[]? Tags => series.Tags;
        public string? Demographic => series.Demographic;
        public bool IsInteractive => series.IsInteractive;

        private LibraryViewModel library => _library;

        public void Open()
        {
            library.MainView.NavigateTo(new BookInfoViewModel(this, library));
        }
    }

    public class ByteArrayToBitmapConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is byte[] image)
            {
                return new Bitmap(new MemoryStream(image));
            }

            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
