using Kotomi.Models.Series;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kotomi.Models.Library
{
    public class Library
    {
        public const string SeriesCollectionName = "Series";

        private string filePath;

        public Library(string filePath)
        {
            this.filePath = filePath;
        }

        public List<ISeries> GetAllSeries()
        {
            var series = Read();

            var concreteSeries = new List<ISeries>();
            foreach (var x in series)
            {
                concreteSeries.Add(SeriesLocator.GetSeriesForPrefixedURL(x.URL));
            }

            return concreteSeries;
        }

        public void Import(string url)
        {
            var x = GetAllSeries();
            var a = SeriesLocator.GetSeriesForPrefixedURL(url);
            a.URL = url;
            x.Add(a);

            var z = new List<DatabaseSeries>();
            foreach (var y in x)
            {
                z.Add(new DatabaseSeries(y, y.URL));
            }

            Write(z);
        }

        public List<DatabaseSeries> Read()
        {
            if (!File.Exists(Path.Combine(filePath, "library.json")))
            {
                Write(new List<DatabaseSeries>());
            }
            using (StreamReader file = File.OpenText(Path.Combine(filePath, "library.json")))
            {
                var jsonSerializer = new JsonSerializer();
                return (List<DatabaseSeries>)jsonSerializer.Deserialize(file, typeof(List<DatabaseSeries>));
            }
        }
        public void Write(List<DatabaseSeries> config)
        {
            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
            using (StreamWriter file = File.CreateText(Path.Combine(filePath, "library.json")))
            {
                new JsonSerializer().Serialize(file, config);
            }
        }
    }

    public class DatabaseSeries
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public byte[] Cover { get; set; }

        public string URL { get; set; }

        [JsonConstructor]
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
