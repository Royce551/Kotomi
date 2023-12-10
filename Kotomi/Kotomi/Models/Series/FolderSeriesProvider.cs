using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kotomi.Models.Series
{
    public class FolderSeriesProvider : ISeriesProvider
    {
        public string Name => "Folder Series Provider";

        public Series GetSeriesForURL(string url)
        {
            var chapterDirectories = Directory.GetDirectories(url, "*", SearchOption.TopDirectoryOnly);

            var series = new Series();
            series.Title = Path.GetFileName(url);
            series.Cover = new Bitmap(Path.Combine(url, "cover.jpg"));

            var chapters = new List<Chapter>();
            foreach (var directory in chapterDirectories)
            {
                var chapter = new Chapter();
                chapter.Title = Path.GetDirectoryName(directory);
                var pages = Directory.GetFiles(directory).ToList();
                pages.Sort();
                chapter.Pages = pages.Select(x => new Bitmap(x)).ToArray();
                chapters.Add(chapter);
            }
            series.Chapters = chapters.ToArray();

            return series;
        }
    }
}
