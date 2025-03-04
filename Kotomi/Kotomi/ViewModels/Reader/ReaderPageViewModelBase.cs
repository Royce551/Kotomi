using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kotomi.ViewModels.Reader
{
    public abstract class ReaderPageViewModelBase(ReaderViewModel reader) : ViewModelBase
    {
        public ReaderViewModel Reader => reader;

        public abstract void LoadImages(CancellationToken cancellationToken);
    }
}
