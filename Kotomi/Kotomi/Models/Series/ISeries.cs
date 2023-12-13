using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kotomi.Models.Series
{
    public interface ISeries
    {
        public string? Title { get; }

        public byte[]? Cover { get;}

        public IChapter[]? Chapters { get; }
    }

    public interface IChapter
    {
        public string? Title { get; }

        public int? TotalPages { get; }

        public Bitmap GetPageAsBitmap(int pageNumber);
    }
}
