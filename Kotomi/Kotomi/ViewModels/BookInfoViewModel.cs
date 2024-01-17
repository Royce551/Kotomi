using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Kotomi.Models.Series;
using System;
using System.Collections.Generic;
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

        private PageViewModelBase previousPage;

        public BookInfoViewModel(ISeries series, PageViewModelBase previousPage)
        {
            Series = series;
            this.previousPage = previousPage;
        }

        public void Back() => MainView.NavigateTo(previousPage);

        public void StartReading() => MainView.NavigateTo(new ReaderViewModel(Series));

        public void AddRemoveLibrary()
        {

        }
    }
}
