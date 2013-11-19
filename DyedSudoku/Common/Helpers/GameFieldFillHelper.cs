using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public static class GameFieldFillHelper
    {
        private const int maxVisibleNumbers = 30;
        private const int maxTryVisibleCount = 3;

        public static void Init(GameFieldModel model, CancellationToken token)
        {
            Task.Factory.StartNew(() => InitModel(model, token), token);
        }

        private static void InitModel(GameFieldModel model, CancellationToken token)
        {
            model.BeginInitializing();

            var rand = new Random();

            while (!InitNumbers(model, rand, token))
            {
            }

            token.ThrowIfCancellationRequested();

            for (int k = 0; k < maxTryVisibleCount && !InitVisible(model, rand, token); k++)
            {
            }

            model.EndInitializing();
        }

        private static bool InitNumbers(GameFieldModel model, Random rand, CancellationToken token)
        {
            model.ClearNumbers();
            model.ClearVisible();

            return SetNumbers(model, rand, token);
        }

        private static bool SetNumbers(GameFieldModel model, Random rand, CancellationToken token)
        {
            foreach (var pair in model.GetBlockOrderedPairs())
            {
                token.ThrowIfCancellationRequested();

                if (!GenerateNumber(model, rand, pair))
                    return false;
            }

            return true;
        }

        private static bool GenerateNumber(GameFieldModel model, Random rand, IndexPair pair)
        {
            var availableNumbers = GetHeuristicsAvailableNumbers(model, pair);
            sbyte number = rand.Choose(availableNumbers);

            model.SetItemNumber(pair, number);

            return !model.IsItemEmpty(pair);
        }

        private static IEnumerable<sbyte> GetHeuristicsAvailableNumbers(GameFieldModel model, IndexPair pair)
        {
            var blockDict = GetBlockAvailableDict(model, pair);
            var available = GetAvailableByDict(blockDict, pair);

            var rowDict = GetRowAvailableDict(model, pair);
            available = available.Intersect(GetAvailableByDict(rowDict, pair));

            var columnDict = GetColumnAvailableDict(model, pair);
            available = available.Intersect(GetAvailableByDict(columnDict, pair));

            return available;
        }

        private static IEnumerable<sbyte> GetAvailableByDict(Dictionary<IndexPair, IEnumerable<sbyte>> dict, IndexPair pair)
        {
            while (SetNumbersIfSingle(dict) || RemoveSingleNumbers(dict))
            {
            }

            return dict.First(item => item.Key.X == pair.X && item.Key.Y == pair.Y).Value;
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

        private static Dictionary<IndexPair, IEnumerable<sbyte>> GetBlockAvailableDict(GameFieldModel model, IndexPair pair)
        {
            var blockPairs = model.GetBlockPairs(pair);
            return GetAvailableDictByPairs(model, blockPairs, pair);
        }

        private static Dictionary<IndexPair, IEnumerable<sbyte>> GetRowAvailableDict(GameFieldModel model, IndexPair pair)
        {
            var rowPairs = model.GetRowPairs(pair);
            return GetAvailableDictByPairs(model, rowPairs, pair);
        }

        private static Dictionary<IndexPair, IEnumerable<sbyte>> GetColumnAvailableDict(GameFieldModel model, IndexPair pair)
        {
            var columnPairs = model.GetColumnPairs(pair);
            return GetAvailableDictByPairs(model, columnPairs, pair);
        }

        private static Dictionary<IndexPair, IEnumerable<sbyte>> GetAvailableDictByPairs(GameFieldModel model, IEnumerable<IndexPair> pairs, IndexPair checkedPair)
        {
            var dict = new Dictionary<IndexPair, IEnumerable<sbyte>>();

            foreach (var pair in pairs)
            {
                dict.Add(pair, model.GetAvailableNumbers(pair, checkedPair));
            }

            return dict;
        }

        private static bool InitVisible(GameFieldModel model, Random rand, CancellationToken token)
        {
            model.ClearVisible();

            return HideItems(model, rand, token);
        }

        private static bool HideItems(GameFieldModel model, Random rand, CancellationToken token)
        {
            var orderedPairs = model.GetAllPairs().ToList();
            var pairs = new List<IndexPair>();

            do
            {
                var pair = rand.Choose(orderedPairs);

                orderedPairs.Remove(pair);

                pairs.Add(pair);
            }
            while(orderedPairs.Any());

            do
            {
                var pair = GetAvailableToHidePair(model, pairs, token);

                if (pair == null)
                    break;

                model.SetItemVisible(pair, false);

                pairs.Remove(pair);
            }
            while(true);

            return pairs.Count <= maxVisibleNumbers;
        }

        private static IndexPair GetAvailableToHidePair(GameFieldModel model, IEnumerable<IndexPair> pairs, CancellationToken token)
        {
            foreach (var pair in pairs)
            {
                token.ThrowIfCancellationRequested();

                model.SetItemVisible(pair, false);

                if (GetHeuristicsAvailableNumbers(model, pair).Count() <= 1)
                    return pair;

                model.SetItemVisible(pair, true);
            }

            return null;
        }
    }
}

