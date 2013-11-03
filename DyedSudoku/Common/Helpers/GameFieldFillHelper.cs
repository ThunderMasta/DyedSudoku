using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Common
{
    public static class GameFieldFillHelper
    {
        private const int maxVisibleNumbers = 30;
        private const int maxTryVisibleCount = 3;

        public static void Init(GameFieldModel model)
        {
            new Thread(() =>
            {
                InitModel(model);
            }).Start();
        }

        private static void InitModel(GameFieldModel model)
        {
            model.BeginInitializing();

            var rand = new Random();

            while (!InitNumbers(model, rand))
            {
            }

            for (int k = 0; k < maxTryVisibleCount && !InitVisible(model, rand); k++)
            {
            }

            model.EndInitializing();
        }

        private static bool InitNumbers(GameFieldModel model, Random rand)
        {
            model.ClearNumbers();
            model.ClearVisible();

            return SetNumbers(model, rand);
        }

        private static bool SetNumbers(GameFieldModel model, Random rand)
        {
            foreach (var pair in model.GetBlockOrderedPairs())
            {
                if (!GenerateNumber(model, rand, pair.X, pair.Y))
                    return false;
            }

            return true;
        }

        private static bool GenerateNumber(GameFieldModel model, Random rand, int x, int y)
        {
            var availableNumbers = GetHeuristicsAvailableNumbers(model, x, y);
            sbyte number = rand.Choose(availableNumbers);

            model.SetItemNumber(number, x, y);

            return !model.IsItemEmpty(x, y);
        }

        private static IEnumerable<sbyte> GetHeuristicsAvailableNumbers(GameFieldModel model, int x, int y)
        {
            var blockDict = GetBlockAvailableDict(model, x, y);
            var available = GetAvailableByDict(blockDict, x, y);

            var rowDict = GetRowAvailableDict(model, y);
            available = available.Intersect(GetAvailableByDict(rowDict, x, y));

            var columnDict = GetColumnAvailableDict(model, x);
            available = available.Intersect(GetAvailableByDict(columnDict, x, y));

            return available;
        }

        private static IEnumerable<sbyte> GetAvailableByDict(Dictionary<IndexPair, IEnumerable<sbyte>> dict, int x, int y)
        {
            while (SetNumbersIfSingle(dict) || RemoveSingleNumbers(dict))
            {
            }

            return dict.First(item => item.Key.X == x && item.Key.Y == y).Value;
        }

        private static bool SetNumbersIfSingle(Dictionary<IndexPair, IEnumerable<sbyte>> dict)
        {
            var isModified = false;

            var manyValuesItems = dict.Where(item => item.Value.Count() > 1).ToList();
            foreach (var dictItem in manyValuesItems)
            {
                foreach (sbyte number in dictItem.Value)
                {
                    if (!dict.Any(item => !item.Equals(dictItem) && item.Value.Contains(number)))
                    {
                        dict.Remove(dictItem.Key);
                        dict.Add(dictItem.Key, number.Yield());
                        isModified = true;
                    }
                }
            }

            return isModified;
        }

        private static bool RemoveSingleNumbers(Dictionary<IndexPair, IEnumerable<sbyte>> dict)
        {
            var isModified = false;

            var singleValueItems = dict.Where(item => item.Value.Count() == 1).ToList();
            foreach (var singleDictItem in singleValueItems)
            {
                var itemsContainsSingle = dict.Where(item => !item.Equals(singleDictItem) && item.Value.Contains(singleDictItem.Value.First())).ToList();
                foreach (var dictItem in itemsContainsSingle)
                {
                    dict.Remove(dictItem.Key);
                    dict.Add(dictItem.Key, dictItem.Value.Except(singleDictItem.Value));
                    isModified = true;
                }
            }

            return isModified;
        }

        private static Dictionary<IndexPair, IEnumerable<sbyte>> GetBlockAvailableDict(GameFieldModel model, int x, int y)
        {
            var blockPairs = model.GetBlockPairs(x, y);
            return GetAvailableDictByPairs(model, blockPairs);
        }

        private static Dictionary<IndexPair, IEnumerable<sbyte>> GetRowAvailableDict(GameFieldModel model, int y)
        {
            var rowPairs = model.GetRowPairs(y);
            return GetAvailableDictByPairs(model, rowPairs);
        }

        private static Dictionary<IndexPair, IEnumerable<sbyte>> GetColumnAvailableDict(GameFieldModel model, int x)
        {
            var columnPairs = model.GetColumnPairs(x);
            return GetAvailableDictByPairs(model, columnPairs);
        }

        private static Dictionary<IndexPair, IEnumerable<sbyte>> GetAvailableDictByPairs(GameFieldModel model, IEnumerable<IndexPair> pairs)
        {
            var dict = new Dictionary<IndexPair, IEnumerable<sbyte>>();

            foreach (var pair in pairs)
            {
                dict.Add(pair, model.GetAvailableNumbers(pair.X, pair.Y));
            }

            return dict;
        }

        private static bool InitVisible(GameFieldModel model, Random rand)
        {
            model.ClearVisible();

            return HideItems(model, rand);
        }

        private static bool HideItems(GameFieldModel model, Random rand)
        {
            var pairs = model.GetAllPairs().ToList();
            do
            {
                var hidePairs = GetAvailableToHideIndexPairs(model, pairs);
                var pair = rand.Choose(hidePairs);

                if (pair == null)
                    break;

                model.SetItemVisible(false, pair.X, pair.Y);

                pairs.Remove(pair);
            }
            while(true);

            return pairs.Count <= maxVisibleNumbers;
        }

        private static IEnumerable<IndexPair> GetAvailableToHideIndexPairs(GameFieldModel model, IEnumerable<IndexPair> pairs)
        {
            foreach (var pair in pairs)
            {
                model.SetItemVisible(false, pair.X, pair.Y);

                if (GetHeuristicsAvailableNumbers(model, pair.X, pair.Y).Count() <= 1)
                    yield return pair;

                model.SetItemVisible(true, pair.X, pair.Y);
            }
        }
    }
}

