using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public static class RandomExt
    {
        public static T Choose<T>(this Random rand, IEnumerable<T> coll)
        {
            if (coll == null || !coll.Any())
                return default(T);

            var arrayColl = coll.ToArray();
            var index = rand.Next(arrayColl.Length);
            return arrayColl[index];
        }
    }
}

