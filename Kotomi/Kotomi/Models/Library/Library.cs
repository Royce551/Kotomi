using Kotomi.Models.Series;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kotomi.Models.Library
{
    public class Library
    {
        public LiteDatabase Database { get; private set; }

        public const string SeriesCollectionName = "Series";

        public Library(LiteDatabase database)
        {
            Database = database;
        }

        public List<ISeries> GetAllSeries()
        {
            var series = Database.GetCollection<DatabaseSeries>(SeriesCollectionName).Query().ToList();

            var concreteSeries = new List<ISeries>();
            foreach (var x in series)
            {
                concreteSeries.Add(SeriesLocator.GetSeriesForPrefixedURL(x.URL));
            }


            return concreteSeries;
        }

        public void Import(string url)
        {
            Database.GetCollection<DatabaseSeries>(SeriesCollectionName).Insert(new DatabaseSeries(SeriesLocator.GetSeriesForPrefixedURL(url), url));
        }
    }

    public class DatabaseSeries
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public byte[] Cover { get; set; }

        public string URL { get; set; }

        public DatabaseSeries(string title, byte[] cover, string url)
        {
            Title = title;
            Cover = cover;
            URL = url;
        }

        public DatabaseSeries(ISeries series, string url)
        {
            Title = series.Title!;
            Cover = series.Cover!;
            URL = url;
        }
    }
}
