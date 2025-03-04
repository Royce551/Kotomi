using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kotomi.ViewModels.Reader
{
    public partial class SinglePageViewModel(ReaderViewModel reader) : ReaderPageViewModelBase(reader)
    {
        [ObservableProperty]
        private Control? pageImage;

        public override async void LoadImages(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;
            PageImage = await Reader.CurrentChapter.GetPageAsControlAsync(Reader.Page, Reader.Cache);
        }
    }
}
