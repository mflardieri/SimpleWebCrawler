﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWebCrawler.Core.Components.Enums
{
    public enum PageLoadSpeed : int
    {
        None = 0,
        SuperSlow,
        Slow,
        Medium,
        Fast,
        SuperFast
    }
}
