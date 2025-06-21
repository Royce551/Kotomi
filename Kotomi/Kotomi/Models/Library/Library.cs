using Kotomi.Models.Series;
using Newtonsoft.Json;
using Realms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kotomi.Models.Library
{
    public partial class DatabaseSeries : IRealmObject
    {
        public string Title { get; set; }

        [PrimaryKey]
        public string URL { get; set; }

        public int LastReadChapterIndex { get; set; } = 0;

        public int LastReadPage { get; set; } = 1;

        [Ignored]
        public ISeries ConcreteSeries => SeriesLocator.GetSeriesForPrefixedURL(URL);

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
