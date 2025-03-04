using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Kotomi.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kotomi.ViewModels.Reader
{
    public partial class LongPageViewModel(ReaderViewModel reader) : ReaderPageViewModelBase(reader)
    {
        public ObservableCollection<Control> Pages { get; private set; } = new();

        public override async void LoadImages(CancellationToken cancellationToken)
        {
            for (int i = 1; i < Reader.CurrentChapter.TotalPages; i++)
            {
                if (cancellationToken.IsCancellationRequested) return;

                var page = await Reader.CurrentChapter.GetPageAsControlAsync(i, Reader.Cache);

                Pages.Add(page);
            
                if (i == 1)
                {
                    page.Bind(Layoutable.MarginProperty, new MultiBinding()
                    {
                        Converter = new CombineMarginsConverter(),
                        Bindings = [new Binding { Source = Reader.MainView, Path = nameof(Reader.MainView.SafeAreaLeftTopRight) },
                                new Binding{ Source = Reader, Path = nameof(Reader.ReadingModeLongMarginAsThickness)}]
                    });
                }
                else if (i == Reader.CurrentChapter.TotalPages)
                {
                    page.Bind(Layoutable.MarginProperty, new MultiBinding()
                    {
                        Converter = new CombineMarginsConverter(),
                        Bindings = [new Binding { Source = Reader.MainView, Path = nameof(Reader.MainView.SafeAreaLeftBottomRight) },
                                new Binding{ Source = Reader, Path = nameof(Reader.ReadingModeLongMarginAsThickness)}]
                    });
                }
                else
                {
                    page.Bind(Layoutable.MarginProperty, new MultiBinding()
                    {
                        Converter = new CombineMarginsConverter(),
                        Bindings = [new Binding { Source = Reader.MainView, Path = nameof(Reader.MainView.SafeAreaLeftRight) },
                                new Binding{ Source = Reader, Path = nameof(Reader.ReadingModeLongMarginAsThickness)}]
                    });
                }
            } 
        }
    }
}
