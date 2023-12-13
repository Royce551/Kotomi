using CommunityToolkit.Mvvm.ComponentModel;
using Kotomi.Models.Library;
using Kotomi.Models.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kotomi.ViewModels
{
    public partial class LibraryViewModel : PageViewModelBase
    {
        [ObservableProperty]
        private ObservableCollection<ISeries> allSeries = new();

        [ObservableProperty]
        private ISeries selectedSeries;

        public LibraryViewModel()
        {

        }

        public void UpdateLibrary()
        {
            AllSeries = new(MainView.Library.GetAllSeries());
        }

        public void OpenSelectedSeries()
        {
            MainView.NavigateTo(new ReaderViewModel(SelectedSeries));
        }

        public void Import(string url)
        {
            MainView.Library.Import($"folder://{url}");
            UpdateLibrary();
        }
    }
}
