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
        [ObservableProperty]
        private ObservableCollection<SeriesViewModel> allSeries = new();

        public LibraryViewModel()
        {

        }

        public void UpdateLibrary()
        {
            AllSeries = new(MainView.Library.GetAllSeries().Select(x => new SeriesViewModel(this, x)));
        }

        public void Import(string url)
        {
            MainView.Library.Import($"folder://{url}");
            UpdateLibrary();
        }
    }

    public class SeriesViewModel : ISeries
    {
        public string? Title { get; }
        public byte[]? Cover { get; }
        public string? URL { get; set; }
        public IChapter[]? Chapters { get; }

        private readonly LibraryViewModel library;
        public SeriesViewModel(LibraryViewModel library, ISeries series)
        {
            this.library = library;
            Title = series.Title;
            Cover = series.Cover;
            URL = series.URL;
            Chapters = series.Chapters;
        }

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
