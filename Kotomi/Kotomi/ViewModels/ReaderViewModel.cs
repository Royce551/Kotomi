using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kotomi.ViewModels
{
    public partial class ReaderViewModel : PageViewModelBase
    {
        public ReaderViewModel()
        {
        }

        [ObservableProperty]
        private Control? currentPage;

        public void SwitchToLibraryView() => MainView.NavigateTo(new LibraryViewModel());

        public void PageLeft()
        {

        }

        public void PageRight()
        {

        }
    }
}
