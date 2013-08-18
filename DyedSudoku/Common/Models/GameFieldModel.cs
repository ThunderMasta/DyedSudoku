using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public class GameFieldModel
    {
        private const sbyte cellLineCount = 9;
        private const sbyte blockLineCount = 3;
        // Zero is left bottom couse CTM is inversed
        private CellItem[,] field;
        private static readonly List<sbyte> numberList = new List<sbyte> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        public sbyte CellLineCount
        {
            get { return cellLineCount; }
        }

        public sbyte BlockLineCount
        {
            get { return blockLineCount; }
        }

        private bool isInitializing;

        public bool IsInitializing
        { 
            get{ return isInitializing;}
        }

        public GameFieldModel()
        {
            InitField();
        }

        private void InitField()
        {
            field = new CellItem[CellLineCount, CellLineCount];

            for (int i = 0; i < CellLineCount; i++)
                for (int j = 0; j < CellLineCount; j++)
                    field[i, j] = new CellItem();

            GameFieldFillHelper.Init(this);
        }

        public sbyte GetItemNumber(int x, int y)
        {
            return field[x, y].Number;
        }

        public void SetItemNumber(sbyte value, int x, int y)
        {
            field[x, y].Number = value;
        }

        public bool GetItemVisible(int x, int y)
        {
            return field[x, y].IsVisible;
        }

        public void SetItemVisible(bool isVisible, int x, int y)
        {
            field[x, y].IsVisible = isVisible;
        }

        public bool IsItemEmpty(int x, int y)
        {
            return field[x, y].IsEmpty;
        }

        public void ClearNumbers()
        {
            for (int i = 0; i < CellLineCount; i++)
                for (int j = 0; j < CellLineCount; j++)
                    field[i, j].ResetNumber();
        }

        public void ClearVisible()
        {
            for (int i = 0; i < CellLineCount; i++)
                for (int j = 0; j < CellLineCount; j++)
                    field[i, j].ResetIsVisible();
        }

        public void BeginInitializing()
        {
            isInitializing = true;
        }

        public void EndInitializing()
        {
            isInitializing = false;
        }

        public IEnumerable<sbyte> GetAvailableNumbers(int x, int y, bool isVisibleOnly)
        {
            return IsItemNotAvailable(x, y, isVisibleOnly)
                    ? GetCheckedAvailableNumbers(x, y, isVisibleOnly)
                    : GetDefaultAvailableNumbers(x, y);
        }

        private bool IsItemNotAvailable(int x, int y, bool isVisibleOnly)
        {
            return IsItemEmpty(x, y) || isVisibleOnly && !GetItemVisible(x, y);
        }

        private IEnumerable<sbyte> GetDefaultAvailableNumbers(int x, int y)
        {
            return GetItemNumber(x, y).Yield();
        }

        private IEnumerable<sbyte> GetCheckedAvailableNumbers(int x, int y, bool isVisibleOnly)
        {
            var cellRowNumbers = GetRowNumbers(y, isVisibleOnly);
            var cellColumnNumbers = GetColumnNumbers(x, isVisibleOnly);
            var cellBlockNumbers = GetBlockNumbers(x, y, isVisibleOnly);

            var alreadyGeneratedNumbers = cellRowNumbers.Union(cellColumnNumbers).Union(cellBlockNumbers).Distinct();

            return numberList.Except(alreadyGeneratedNumbers);
        }

        private IEnumerable<sbyte> GetRowNumbers(int y, bool isVisibleOnly)
        {
            return GetAvailableNumbersByPairs(GetRowPairs(y), isVisibleOnly);
        }

        private IEnumerable<sbyte> GetColumnNumbers(int x, bool isVisibleOnly)
        {
            return GetAvailableNumbersByPairs(GetColumnPairs(x), isVisibleOnly);
        }

        private IEnumerable<sbyte> GetBlockNumbers(int x, int y, bool isVisibleOnly)
        {
            return GetAvailableNumbersByPairs(GetBlockPairs(x, y), isVisibleOnly);
        }

        private IEnumerable<sbyte> GetAvailableNumbersByPairs(IEnumerable<IndexPair> pairs, bool isVisibleOnly)
        {
            foreach (var pair in pairs)
            {
                if (IsItemNotAvailable(pair.X, pair.Y, isVisibleOnly))
                    continue;

                yield return GetItemNumber(pair.X, pair.Y);
            }
        }

        public IEnumerable<IndexPair> GetBlockPairs(int x, int y)
        {
            int xBlock = x / BlockLineCount;
            int yBlock = y / BlockLineCount;

            int blockColumn = xBlock * BlockLineCount;
            int blockRow = yBlock * BlockLineCount;

            for (int i = 0; i < BlockLineCount; i++)
                for (int j = 0; j < BlockLineCount; j++)
                {
                    var xCell = blockColumn + i;
                    var yCell = blockRow + j;

                    yield return new IndexPair(xCell, yCell);
                }
        }

        public IEnumerable<IndexPair> GetRowPairs(int y)
        {
            for (int i = 0; i < CellLineCount; i++)
            {
                yield return new IndexPair(i, y);
            }
        }

        public IEnumerable<IndexPair> GetColumnPairs(int x)
        {
            for (int j = CellLineCount - 1; j >= 0; j--)
            {
                yield return new IndexPair(x, j);
            }
        }
    }
}