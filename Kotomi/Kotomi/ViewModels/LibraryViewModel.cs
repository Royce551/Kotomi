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

        public void SwitchToReaderView() => MainView.NavigateTo(new ReaderViewModel());
    }
}
