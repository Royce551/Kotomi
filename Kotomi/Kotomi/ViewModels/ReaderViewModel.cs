using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kotomi.ViewModels
{
    public partial class ReaderViewModel : PageViewModelBase
    {
        public ReaderViewModel(MainViewModel mainView) : base(mainView)
        {
        }

        public void SwitchToLibraryView()
        {
            MainView.SelectedView = new LibraryViewModel(MainView);
        }
    }
}
