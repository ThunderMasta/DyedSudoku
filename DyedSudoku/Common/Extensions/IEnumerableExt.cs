using System;
using System.Collections.Generic;

namespace Common
{
    public static class IEnumerableExt
    {
        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }
    }
}

