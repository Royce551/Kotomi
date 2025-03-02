using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Kotomi.Models.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Kotomi.ViewModels
{
    public class SeriesCachingContext
    {
        public Dictionary<(decimal chapter, int page), Bitmap> Cache { get; set; } = new();

        public async Task<Bitmap> CacheAndGetBitmapAsync(decimal chapter, int page, Func<Task<Bitmap>> addToCacheAction)
        {
            if (!Cache.ContainsKey((chapter, page))) Cache.Add((chapter, page), await addToCacheAction.Invoke());
            
            var cachedControl = Cache[(chapter, page)];

            return cachedControl;
        }
    }
}
