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
			var valueCollList = valueColl.ToList();
			var collList = coll.ToList();

			if (valueCollList.Count > collList.Count)
                return false;

			var sourceItemsList = collList.ToList();

			return valueCollList.All(sourceItemsList.Contains);
        }

        public static IEnumerable<List<T>> GetAllSubItems<T>(this IEnumerable<T> coll, int count)
        {
			var collArray = coll.ToArray();

			if (!collArray.Any())
				yield break;

            if (count == 1)
            {
				foreach (var x in collArray)
                {
                    yield return new List<T> { x };
                }
            }
            else if (count == 2)
            {
				for (int i = 0; i < collArray.Length - 1; i++)
                {
					for (int j = i + 1; j < collArray.Length; j++)
                    {
						yield return new List<T> { collArray[i], collArray[j] };
                    }
                }
            }
            else if (count == 3)
            {
				for (int i = 0; i < collArray.Length - 2; i++)
				{
					for (int j = i + 1; j < collArray.Length - 1; j++)
					{
						for (int k = j + 1; k < collArray.Length ; k++)
						{
							yield return new List<T> { collArray[i], collArray[j], collArray[k] };
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

