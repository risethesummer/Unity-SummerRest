﻿using System.Collections.Generic;
using SummerRest.Utilities.DataStructures;

namespace SummerRest.Runtime.Parsers
{
    public interface IUrlBuilder : IDefaultSupport<IUrlBuilder, DefaultUrlBuilder>
    {
        string BuildUrl(string url, IEnumerable<KeyValuePair<string, ICollection<string>>> parameters);
    }
}