using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Common
{
    public static class GameFieldFillHelper
    {
        private static readonly List<sbyte> numberList = new List<sbyte> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        static GameFieldFillHelper()
        {

        }

        public static void Init(GameFieldModel model)
        {
            new Thread(() => {
                InitModel(model);}).Start();
        }

        private static void InitModel(GameFieldModel model)
        {
            model.BeginInitializing();

            while (!InitModelInLoop(model))
            {
            }

            model.EndInitializing();
        }

        private static bool InitModelInLoop(GameFieldModel model)
        {
            var rand = new Random();

            while (!InitNumbers(model, rand))
            {
            }

            for (int k = 0; k < 10 && !InitVisible(model, rand); k++)
            {
            }

            return true;
        }

        private static bool InitNumbers(GameFieldModel model, Random rand)
        {
            model.ClearNumbers();
            model.ClearVisible();

            return SetNumbers(model, rand);
        }

        private static bool SetNumbers(GameFieldModel model, Random rand)
        {
            // Start init rows from left top corner
            for (int j = model.CellLineCount - 1; j >= 0; j--)
                for (int i = 0; i < model.CellLineCount; i++)
                {
                    if (!GenerateNumber(model, rand, i, j))
                        return false;
                }

            return true;
        }

        private static bool GenerateNumber(GameFieldModel model, Random rand, int x, int y)
        {
            var availableNumbers = GetAvailableNumbers(model, x, y);
            sbyte? number = rand.ChooseV(availableNumbers);

            if (!number.HasValue)
                return false;

            model.SetItemNumber(number.Value, x, y);

            return true;
        }

        private static IEnumerable<sbyte> GetAvailableNumbers(GameFieldModel model, int x, int y, bool isVisibleOnly = false)
        {
            var cellRowNumbers = GetRowNumbers(model, x, y, isVisibleOnly);
            var cellColumnNumbers = GetColumnNumbers(model, x, y, isVisibleOnly);
            var cellBlockNumbers = GetBlockNumbers(model, x, y, isVisibleOnly);

            var alreadyGeneratedNumbers = cellRowNumbers.Union(cellColumnNumbers).Union(cellBlockNumbers).Distinct();

            return numberList.Except(alreadyGeneratedNumbers);
        }

        private static IEnumerable<sbyte> GetRowNumbers(GameFieldModel model, int x, int y, bool isVisibleOnly = false)
        {
            for (int i = 0; i < x; i++)
            {
                if (model.IsItemEmpty(i, y) || isVisibleOnly && !model.GetItemVisible(i, y))
                    continue;

                yield return model.GetItemNumber(i, y);
            }
        }

        private static IEnumerable<sbyte> GetColumnNumbers(GameFieldModel model, int x, int y, bool isVisibleOnly = false)
        {
            for (int j = model.CellLineCount - 1; j > y; j--)
            {
                if (model.IsItemEmpty(x, j) || isVisibleOnly && !model.GetItemVisible(x, j))
                    continue;

                yield return model.GetItemNumber(x, j);
            }
        }

        private static IEnumerable<sbyte> GetBlockNumbers(GameFieldModel model, int x, int y, bool isVisibleOnly = false)
        {
            int xBlock = x / model.BlockLineCount;
            int yBlock = y / model.BlockLineCount;

            int blockColumn = xBlock * model.BlockLineCount;
            int blockRow = yBlock * model.BlockLineCount;

            for (int i = 0; i < model.BlockLineCount; i++)
                for (int j = 0; j < model.BlockLineCount; j++)
                {
                    var xCell = blockColumn + i;
                    var yCell = blockRow + j;

                    if (model.IsItemEmpty(xCell, yCell) || isVisibleOnly && !model.GetItemVisible(xCell, yCell))
                        continue;

                    yield return model.GetItemNumber(xCell, yCell);
                }
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
                var pair = rand.ChooseC(hidePairs);

                if (pair == null)
                    break;

                model.SetItemVisible(false, pair.X, pair.Y);

                pairs.Remove(pair);
            }
            while(true);

            return pairs.Count < 39;
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
                if (GetAvailableNumbers(model, pair.X, pair.Y, true).Count() <= 1)
                    yield return pair;
            }
        }
    }
}

