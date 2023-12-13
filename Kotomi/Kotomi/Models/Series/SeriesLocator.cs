using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kotomi.Models.Series
{
    public static class SeriesLocator
    {
        public static ISeries GetSeriesForPrefixedURL(string prefixedURL)
        { // TODO: implement some sort of plugin system
            return new FolderSeriesProvider().GetSeriesForURL(prefixedURL.Replace("folder://", ""));
        }
    }
}
