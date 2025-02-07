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
        public string Source { get; }

        public string? Title { get; }

        public string? Author { get; }

        public byte[]? Cover { get; }

        public string? URL { get; set; }

        public string? Description { get; }

        public string[]? Genres { get; }

        public string[]? Tags { get; }

        public string? Demographic { get; }

        public IChapter[] Chapters { get; }
    }

    public interface IChapter
    {
        public string? Title { get; }

        public decimal? VolumeNumber { get; }

        public decimal? ChapterNumber { get; }

        public int? TotalPages { get; }

        public Bitmap GetPageAsBitmap(int pageNumber);
    }
}
