﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kotomi.Models.Series
{
    public interface ISeriesProvider
    {
        public string Name { get; }

        public Series GetSeriesForURL(string url);
    }
}
