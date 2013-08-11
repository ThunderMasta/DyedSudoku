using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public static class RandomExt
    {
        public static Nullable<T> ChooseV<T>(this Random rand, IEnumerable<T> coll) where T : struct
        {
            if (coll == null || !coll.Any())
                return null;

            var arrayColl = coll.ToArray();
            var index = rand.Next(arrayColl.Length);
            return arrayColl[index];
        }

        public static T ChooseC<T>(this Random rand, IEnumerable<T> coll) where T : class
        {
            if (coll == null || !coll.Any())
                return null;

            var arrayColl = coll.ToArray();
            var index = rand.Next(arrayColl.Length);
            return arrayColl[index];
        }
    }
}

