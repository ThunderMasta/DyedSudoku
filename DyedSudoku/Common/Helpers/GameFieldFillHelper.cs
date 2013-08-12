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
            new Thread(() => {
                InitModel(model);}).Start();
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
            // Start init blocks from left top corner
            for (int j = model.CellLineCount - 1; j >= 0; j -= model.BlockLineCount)
                for (int i = 0; i < model.CellLineCount; i += model.BlockLineCount)
                {
                    var blockPairs = model.GetBlockPairs(i, j);
                    foreach (var pair in blockPairs)
                    {
                        if (!GenerateNumber(model, rand, pair.X, pair.Y))
                            return false;
                    }
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

        private static IEnumerable<sbyte> GetHeuristicsAvailableNumbers(GameFieldModel model, int x, int y, bool isVisibleOnly = false)
        {
            var blockDict = GetBlockAvailableDict(model, x, y, isVisibleOnly);
            var available = GetAvailableByPairs(blockDict, x, y);

            var rowDict = GetRowAvailableDict(model, y, isVisibleOnly);
            available = available.Intersect(GetAvailableByPairs(rowDict, x, y));

            var columnDict = GetColumnAvailableDict(model, x, isVisibleOnly);
            available = available.Intersect(GetAvailableByPairs(columnDict, x, y));

            return available;
        }

        private static IEnumerable<sbyte> GetAvailableByPairs(Dictionary<IndexPair, IEnumerable<sbyte>> dict, int x, int y)
        {
            bool isModified;

            do
            {
                isModified = false;
                foreach (var keyValuePair in dict.Where(item => item.Value.Count() > 1).ToDictionary(item => item.Key, item => item.Value))
                {
                    foreach (sbyte number in keyValuePair.Value)
                    {
                        if (!dict.Any(item => !item.Equals(keyValuePair) && item.Value.Contains(number)))
                        {
                            dict.Remove(keyValuePair.Key);
                            dict.Add(keyValuePair.Key, number.Yield());
                            isModified = true;
                        }
                    }
                }

                foreach (var singleValuePair in dict.Where(item => item.Value.Count() == 1).ToDictionary(item => item.Key, item => item.Value))
                {
                    foreach (var keyValuePair in dict.Where(item => !item.Equals(singleValuePair) && item.Value.Contains(singleValuePair.Value.First())).ToDictionary(item => item.Key, item => item.Value))
                    {
                        dict.Remove(keyValuePair.Key);
                        dict.Add(keyValuePair.Key, keyValuePair.Value.Except(singleValuePair.Value));
                        isModified = true;
                    }
                }
            }
            while(isModified);

            return dict.First(item => item.Key.X == x && item.Key.Y == y).Value;
        }

        private static Dictionary<IndexPair, IEnumerable<sbyte>> GetBlockAvailableDict(GameFieldModel model, int x, int y, bool isVisibleOnly)
        {
            var dict = new Dictionary<IndexPair, IEnumerable<sbyte>>();
            var blockPairs = model.GetBlockPairs(x, y);

            foreach (var pair in blockPairs)
            {
                dict.Add(pair, model.GetAvailableNumbers(pair.X, pair.Y, isVisibleOnly));
            }

            return dict;
        }

        private static Dictionary<IndexPair, IEnumerable<sbyte>> GetRowAvailableDict(GameFieldModel model, int y, bool isVisibleOnly)
        {
            var dict = new Dictionary<IndexPair, IEnumerable<sbyte>>();
            var blockPairs = model.GetRowPairs(y);

            foreach (var pair in blockPairs)
            {
                dict.Add(pair, model.GetAvailableNumbers(pair.X, pair.Y, isVisibleOnly));
            }

            return dict;
        }

        private static Dictionary<IndexPair, IEnumerable<sbyte>> GetColumnAvailableDict(GameFieldModel model, int x, bool isVisibleOnly)
        {
            var dict = new Dictionary<IndexPair, IEnumerable<sbyte>>();
            var blockPairs = model.GetColumnPairs(x);

            foreach (var pair in blockPairs)
            {
                dict.Add(pair, model.GetAvailableNumbers(pair.X, pair.Y, isVisibleOnly));
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
            var pairs = GetAllIndexPairs(model).ToList();
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

        private static IEnumerable<IndexPair> GetAllIndexPairs(GameFieldModel model)
        {
            for (int i = 0; i < model.CellLineCount; i++)
                for (int j = 0; j < model.CellLineCount; j++)
                {
                    yield return new IndexPair(i, j);
                }
        }

        private static IEnumerable<IndexPair> GetAvailableToHideIndexPairs(GameFieldModel model, IEnumerable<IndexPair> pairs)
        {
            foreach (var pair in pairs)
            {
                model.SetItemVisible(false, pair.X, pair.Y);

                if (GetHeuristicsAvailableNumbers(model, pair.X, pair.Y, true).Count() <= 1)
                    yield return pair;

                model.SetItemVisible(true, pair.X, pair.Y);
            }
        }
    }
}

