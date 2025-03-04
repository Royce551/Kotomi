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
    public partial class TwoPageViewModel(ReaderViewModel reader) : ReaderPageViewModelBase(reader)
    {
        [ObservableProperty]
        private Control? leftImage;

        [ObservableProperty]
        private Control? rightImage;

        public override async void LoadImages(CancellationToken cancellationToken)
        {
            if (Reader.MainView.Config.ReadingDirectionRightToLeft)
            {
                if (cancellationToken.IsCancellationRequested) return;
                RightImage = await Reader.CurrentChapter.GetPageAsControlAsync(Reader.Page, Reader.Cache);

                if (cancellationToken.IsCancellationRequested) return;
                if (Reader.SecondPage <= Reader.CurrentChapter.TotalPages)
                    LeftImage = await Reader.CurrentChapter.GetPageAsControlAsync(Reader.SecondPage, Reader.Cache);
            }
            else
            {
                if (cancellationToken.IsCancellationRequested) return;
                LeftImage = await Reader.CurrentChapter.GetPageAsControlAsync(Reader.Page, Reader.Cache);

                if (cancellationToken.IsCancellationRequested) return;
                if (Reader.SecondPage <= Reader.CurrentChapter.TotalPages)
                    RightImage = await Reader.CurrentChapter.GetPageAsControlAsync(Reader.SecondPage, Reader.Cache);
            }
        }
    }
}
