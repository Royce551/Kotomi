using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kotomi.Models.Series
{
    public class Series
    {
        public string? Title { get; set; }

        public Bitmap? Cover { get; set; }

        public Chapter[]? Chapters { get; set; }
    }

    public class Chapter
    {
        public string? Title { get; set; }

        public Bitmap[]? Pages { get; set; }
    }
}
