﻿using Avalonia.Media.Imaging;
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

        public string Prefix => "folder";

        public ISeries GetSeriesForURL(string url)
        {
            var chapterDirectories = Directory.GetDirectories(url, "*", SearchOption.TopDirectoryOnly);

            var series = new FolderSeries();
            series.Title = Path.GetFileName(url);
            series.Cover = File.ReadAllBytes(Path.Combine(url, "cover.jpg"));

            var chapters = new List<FolderChapter>();
            foreach (var directory in chapterDirectories)
            {
                var chapter = new FolderChapter();
                chapter.Title = Path.GetFileName(directory);
                var pages = Directory.GetFiles(directory).ToList();
                pages.Sort();
                chapter.Pages = pages;
                chapters.Add(chapter);
            }
            series.Chapters = chapters.ToArray();

            return series;
        }
    }

    public class FolderSeries : ISeries
    {
        public string? Title { get; set; }

        public string URL { get; set; }

        public byte[]? Cover { get; set; }

        public IChapter[]? Chapters { get; set; }
    }

    public class FolderChapter : IChapter
    {
        public string? Title { get; set; }
        public int? TotalPages => Pages.Count + 1;

        public List<string> Pages { get; set; } = default!;

        public Bitmap GetPageAsBitmap(int pageNumber) => new Bitmap(Pages[pageNumber - 1]);
    }
}
