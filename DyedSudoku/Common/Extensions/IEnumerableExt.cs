using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public static class IEnumerableExt
    {
        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }

        public static bool Contains<T>(this IEnumerable<T> coll, IEnumerable<T> valueColl)
        {
            if (valueColl.Count() > coll.Count())
                return false;

            var sourceItemsList = coll.ToList();

            return valueColl.All(sourceItemsList.Contains);
        }

        public static IEnumerable<List<T>> GetAllSubItems<T>(this IEnumerable<T> coll, int count)
        {
            if (count == 1)
            {
                foreach (var x in coll)
                {
                    yield return new List<T> { x };
                }
            }
            else if (count == 2)
            {
                foreach (var x in coll)
                {
                    foreach (var y in coll.Where(item => !item.Equals(x)))
                    {
                        yield return new List<T> { x, y };
                    }
                }
            }
            else if (count == 3)
            {
                foreach (var x in coll)
                {
                    foreach (var y in coll.Where(item => !item.Equals(x)))
                    {
                        foreach (var z in coll.Where(item => !item.Equals(x) && !item.Equals(y)))
                        {
                            yield return new List<T> { x, y, z };
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentException("count");
            }
        }
    }
}

