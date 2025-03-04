using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Kotomi.Models.Configuration;
using Kotomi.Models.Series;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Kotomi.ViewModels
{
    public class SeriesCachingContext(ConfigurationFile config)
    {
        public OrderedDictionary<(decimal chapter, int page), Bitmap> Cache { get; set; } = new();

        public async Task<Bitmap> CacheAndGetBitmapAsync(decimal chapter, int page, Func<Task<Bitmap>> addToCacheAction)
        {
            if (Cache.Count > config.MaxCachedPages)
            {
                Debug.WriteLine($"Page removed from cache: {Cache.First().Key.chapter} {Cache.First().Key.page}");
                Cache.Remove(Cache.First().Key);
            }

            if (!Cache.ContainsKey((chapter, page)))
            {
                Debug.WriteLine($"Cache miss: {chapter}, {page}");
                Cache.Add((chapter, page), await addToCacheAction.Invoke());
            }
            var cachedControl = Cache[(chapter, page)];
     
            return cachedControl;
        }
    }
}
