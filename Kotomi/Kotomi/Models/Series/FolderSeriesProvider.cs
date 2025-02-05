using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kotomi.Models.Series
{
    public class FolderSeriesProvider : ISeriesProvider
    {
        public string Name => "Local Folder";

        public string Prefix => "folder";

        public ISeries GetSeriesForURL(string url)
        {
            var chapterDirectories = Directory.GetDirectories(url, "*", SearchOption.TopDirectoryOnly);

            var chapters = new List<FolderChapter>();

            var volumeRegex = new Regex("[Vv]ol[^\\d]+(\\d+)");
            var chapterRegex = new Regex("[Cc]h.+(\\d+)");
            if (!chapterDirectories.Any(x => !chapterRegex.IsMatch(x)))
            {
                foreach (var directory in chapterDirectories)
                {
                    var volumeMatch = volumeRegex.Match(directory);
                    var chapterMatch = chapterRegex.Match(directory);

                    var chapter = new FolderChapter();
                    chapter.Title = Path.GetFileName(directory);
                    var pages = Directory.GetFiles(directory).ToList();
                    pages.Sort();
                    chapter.Pages = pages;

                    if (volumeMatch.Success) chapter.VolumeNumber = int.Parse(volumeMatch.Groups[1].Value);
                    chapter.ChapterNumber = int.Parse(chapterMatch.Groups[1].Value);

                    chapters.Add(chapter);
                }
            }
            else
            {
                foreach (var directory in chapterDirectories)
                {
                    var chapter = new FolderChapter();
                    chapter.Title = Path.GetFileName(directory);
                    var pages = Directory.GetFiles(directory).ToList();
                    pages.Sort();
                    chapter.Pages = pages;
                    chapters.Add(chapter);
                }
            }

            var series = new FolderSeries(Path.GetFileName(url), chapters.OrderBy(x => x.VolumeNumber).ThenBy(x => x.ChapterNumber).ToArray())
            {
                Cover = File.ReadAllBytes(Path.Combine(url, "cover.jpg"))
            };

            return series;
        }
    }

    public class FolderSeries(string title, IChapter[] chapters) : ISeries
    {
        public string Source => "Local folder";

        public string? Title => title;

        public string? Author { get; set; }

        public string? URL { get; set; }

        public string? Description { get; init; }

        public string[]? Genres { get; init; }

        public string[]? Tags { get; init; }

        public string? Demographic { get; init; }

        public byte[]? Cover { get; init; }

        public IChapter[] Chapters => chapters;
    }

    public class FolderChapter : IChapter
    {
        public string? Title { get; set; }

        public int? TotalPages => Pages.Count;

        public int? VolumeNumber { get; set; }

        public int? ChapterNumber { get; set; }

        public List<string> Pages { get; set; } = default!;

        public Bitmap GetPageAsBitmap(int pageNumber) => new Bitmap(Pages[pageNumber - 1]);

        public override string ToString() => Title;
    }
}
