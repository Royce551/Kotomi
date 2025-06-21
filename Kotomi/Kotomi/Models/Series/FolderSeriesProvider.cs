using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Kotomi.ViewModels;
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

            var volumeRegex = new Regex("(?<=[V]ol[\\w.]* )([\\d.]+)");
            var chapterRegex = new Regex("(?<=[Cc]h[\\w.]* )([\\d.]+)");
            if (!chapterDirectories.Any(x => !chapterRegex.IsMatch(x))) // If the volume and chapter info can be extracted for each folder
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

                    if (volumeMatch.Success) chapter.VolumeNumber = decimal.Parse(volumeMatch.Groups[1].Value);
                    chapter.ChapterNumber = decimal.Parse(chapterMatch.Groups[1].Value);

                    chapters.Add(chapter);
                }
            }
            else // Otherwise, resort to just providing a sorted list of chapters
            {
                int i = 1;
                foreach (var directory in chapterDirectories)
                {
                    var chapter = new FolderChapter();
                    chapter.Title = Path.GetFileName(directory);
                    var pages = Directory.GetFiles(directory).ToList();
                    pages.Sort();
                    chapter.Pages = pages;
                    chapter.ChapterNumber = i;
                    chapters.Add(chapter);
                    i++;
                }
            }

            var series = new FolderSeries(Path.GetFileName(Path.GetDirectoryName(url)), chapters.OrderBy(x => x.VolumeNumber).ThenBy(x => x.ChapterNumber).ToArray())
            {
                Cover = File.ReadAllBytes(Path.Combine(url, "cover.jpg"))
            };
            series.URL = url;
            return series;
        }
    }

    public class FolderSeries(string title, IChapter[] chapters) : ISeries
    {
        public string Source => "Local folder";

        public string? Title => title;

        public string? Author { get; set; }

        public string? Description { get; init; }

        public string[]? Genres { get; init; }

        public string[]? Tags { get; init; }

        public string? Demographic { get; init; }

        public byte[]? Cover { get; init; }

        public IChapter[] Chapters => chapters;

        public bool IsInteractive => false;

        public string? URL { get; set; }

        public string? PrefixedURL => $"folder://{URL}";
    }

    public class FolderChapter : IChapter
    {
        public string? Title { get; set; }

        public int TotalPages => Pages.Count;

        public decimal? VolumeNumber { get; set; }

        public decimal ChapterNumber { get; set; }

        public List<string> Pages { get; set; } = default!;

        public async Task CachePage(int pageNumber, SeriesCachingContext cache)
        {
            await cache.CacheAsync(ChapterNumber, pageNumber, async () =>
            {
                return await Task.Run(() => new Bitmap(Pages[pageNumber - 1]));
            });
        }

        public async Task<Control> GetPageAsControlAsync(int pageNumber, SeriesCachingContext cache) => new Image() 
        { 
            Source = await cache.CacheAndGetBitmapAsync(ChapterNumber, pageNumber, async () => 
            {
                return await Task.Run(() => new Bitmap(Pages[pageNumber - 1]));            
            })};

        public override string ToString() => Title ?? string.Empty;
    }
}
