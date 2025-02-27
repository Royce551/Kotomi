using Kotomi.Models.Series;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kotomi.Models.Library
{
    public class Library
    {
        public List<ISeries> Series { get; private set; }

        private string filePath;

        public Library(string filePath)
        {
            this.filePath = filePath;

            Series = GetAllSeries();
        }

        public void Import(string url)
        {
            Series.Add(SeriesLocator.GetSeriesForPrefixedURL(url));
            Close();
        }

        public void Close()
        {
            var seriesAsDatabaseSeries = Series.Select(x => new DatabaseSeries(x));
            Write(seriesAsDatabaseSeries.ToList());
        }

        private List<ISeries> GetAllSeries()
        {
            var series = Read();

            var concreteSeries = new List<ISeries>();
            foreach (var x in series)
            {
                concreteSeries.Add(SeriesLocator.GetSeriesForPrefixedURL(x.URL));
            }

            return concreteSeries;
        }

        private List<DatabaseSeries> Read()
        {
            if (!File.Exists(Path.Combine(filePath, "library.json"))) Write(new List<DatabaseSeries>());

            using StreamReader file = File.OpenText(Path.Combine(filePath, "library.json"));
            var jsonSerializer = new JsonSerializer();

            return (List<DatabaseSeries>?)jsonSerializer.Deserialize(file, typeof(List<DatabaseSeries>)) ?? throw new Exception();
        }
        private void Write(List<DatabaseSeries> config)
        {
            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

            using StreamWriter file = File.CreateText(Path.Combine(filePath, "library.json"));
            new JsonSerializer().Serialize(file, config);
        }
    }

    public class DatabaseSeries
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string URL { get; set; }

        [JsonConstructor]
        public DatabaseSeries(string title, string url)
        {
            Title = title;
            URL = url;
        }

        public DatabaseSeries(ISeries series)
        {
            Title = series.Title!;
            URL = series.PrefixedURL!;
        }
    }
}
