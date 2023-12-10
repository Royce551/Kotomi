using Kotomi.Models.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kotomi.ViewModels
{
    public partial class LibraryViewModel : PageViewModelBase
    {
        public LibraryViewModel()
        {

        }

        public void OpenReader(string url)
        {
            MainView.NavigateTo(new ReaderViewModel(new FolderSeriesProvider().GetSeriesForURL(url)));
        }
    }
}
