﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kotomi.ViewModels
{
    public class PageViewModelBase : ViewModelBase
    {
        public MainViewModel MainView { get; set; } = default!;

        public PageViewModelBase()
        {

        }
    }
}
