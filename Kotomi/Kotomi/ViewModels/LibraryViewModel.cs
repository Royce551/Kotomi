using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kotomi.ViewModels
{
    public partial class LibraryViewModel : PageViewModelBase
    {
        public LibraryViewModel(MainViewModel mainView) : base(mainView)
        {
        }

        public void SwitchToReaderView()
        {
            MainView.SelectedView = new ReaderViewModel(MainView);
        }
    }
}
