using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Common
{
	public class GameFieldModel
	{
		private const sbyte cellLineCount = 9;
		private const sbyte blockLineCount = 3;
		// Zero is left bottom couse CTM is inversed
		private CellItem[,] field;
		private static readonly List<sbyte> numberList = new List<sbyte> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
		private CancellationTokenSource cancellationTokenSource;
		private IndexPair[,] indexPairs;

		public sbyte CellLineCount
		{
			get { return cellLineCount; }
		}

		public sbyte BlockLineCount
		{
			get { return blockLineCount; }
		}

		public IndexPair SelectedPair { get; set; }

		private bool isInitializing;

		public bool IsInitializing
		{ 
			get{ return isInitializing; }
		}

		public bool IsAllItemsVisible
		{
			get { return GetAllPairs().All(GetItemVisible); }
		}

		public bool IsEmpty
		{
			get { return GetAllPairs().All(IsItemEmpty); }
		}

		public int VisibleCount
		{
			get { return GetAllPairs().Count(GetItemVisible); }
		}

		public GameFieldModel()
		{
			InitField();
		}

		public void Cancel()
		{
			if (IsInitializing && cancellationTokenSource != null)
				cancellationTokenSource.Cancel();
		}

		private void InitField()
		{
			field = new CellItem[CellLineCount, CellLineCount];
			indexPairs = new IndexPair[CellLineCount, CellLineCount];

			for (int i = 0; i < CellLineCount; i++)
				for (int j = 0; j < CellLineCount; j++)
				{
					indexPairs[i, j] = new IndexPair(i, j);
				}

			foreach (var pair in GetAllPairs())
				field[pair.X, pair.Y] = new CellItem();

			cancellationTokenSource = new CancellationTokenSource();

			GameFieldFillHelper.Init(this, cancellationTokenSource.Token);
		}

		public sbyte GetItemNumber(IndexPair pair)
		{
			return field[pair.X, pair.Y].Number;
		}

		public void SetItemNumber(IndexPair pair, sbyte value)
		{
			field[pair.X, pair.Y].Number = value;
		}

		public bool GetItemVisible(IndexPair pair)
		{
			return field[pair.X, pair.Y].IsVisible;
		}

		public void SetItemVisible(IndexPair pair, bool isVisible)
		{
			field[pair.X, pair.Y].IsVisible = isVisible;
		}

		public bool IsItemEmpty(IndexPair pair)
		{
			return field[pair.X, pair.Y].IsEmpty;
		}

		public void ClearNumbers()
		{
			foreach (var pair in GetAllPairs())
				ResetNumber(pair);
		}

		public void ClearVisible()
		{
			foreach (var pair in GetAllPairs())
				ResetIsVisible(pair);
		}

		private void ResetNumber(IndexPair pair)
		{
			field[pair.X, pair.Y].ResetNumber();
		}

		private void ResetIsVisible(IndexPair pair)
		{
			field[pair.X, pair.Y].ResetIsVisible();
		}

		public void BeginInitializing()
		{
			isInitializing = true;
		}

		public void EndInitializing()
		{
			isInitializing = false;
		}

		public bool IsSelectedPair(IndexPair pair)
		{
			var selectedPair = SelectedPair;
			return isInitializing && selectedPair != null && selectedPair.X == pair.X && selectedPair.Y == pair.Y;
		}

		public IEnumerable<sbyte> GetAvailableNumbers(IndexPair pair, IndexPair checkedPair)
		{
			return IsItemNotAvailable(pair, checkedPair)
                    ? GetCheckedAvailableNumbers(pair, checkedPair)
                    : GetDefaultAvailableNumbers(pair);
		}

		public bool IsItemNotAvailable(IndexPair pair, IndexPair checkedPair)
		{
			return IsItemEmpty(pair) || !GetItemVisible(pair) || checkedPair.X == pair.X && checkedPair.Y == pair.Y;
		}

		private IEnumerable<sbyte> GetDefaultAvailableNumbers(IndexPair pair)
		{
			return GetItemNumber(pair).Yield();
		}

		private IEnumerable<sbyte> GetCheckedAvailableNumbers(IndexPair pair, IndexPair checkedPair)
		{
			var cellBlockNumbers = GetBlockNumbers(pair, checkedPair);
			var cellRowNumbers = GetRowNumbers(pair, checkedPair);

			var blockRowNumbers = cellBlockNumbers.Union(cellRowNumbers).Distinct().ToList();

			if (blockRowNumbers.Count == CellLineCount)
				return new List<sbyte>();

			var cellColumnNumbers = GetColumnNumbers(pair, checkedPair);

			var alreadyGeneratedNumbers = blockRowNumbers.Union(cellColumnNumbers).Distinct();

			return numberList.Except(alreadyGeneratedNumbers);
		}

		private IEnumerable<sbyte> GetRowNumbers(IndexPair pair, IndexPair checkedPair)
		{
			return GetAvailableNumbersByPairs(GetRowPairs(pair), checkedPair);
		}

		private IEnumerable<sbyte> GetColumnNumbers(IndexPair pair, IndexPair checkedPair)
		{
			return GetAvailableNumbersByPairs(GetColumnPairs(pair), checkedPair);
		}

		private IEnumerable<sbyte> GetBlockNumbers(IndexPair pair, IndexPair checkedPair)
		{
			return GetAvailableNumbersByPairs(GetBlockPairs(pair), checkedPair);
		}

		private IEnumerable<sbyte> GetAvailableNumbersByPairs(IEnumerable<IndexPair> pairs, IndexPair checkedPair)
		{
			foreach (var pair in pairs)
			{
				if (IsItemNotAvailable(pair, checkedPair))
					continue;

				yield return GetItemNumber(pair);
			}
		}

		public IEnumerable<IndexPair> GetBlockPairs(IndexPair pair)
		{
			int xBlock = pair.X / BlockLineCount;
			int yBlock = pair.Y / BlockLineCount;

			int blockColumn = xBlock * BlockLineCount;
			int blockRow = yBlock * BlockLineCount;

			for (int i = 0; i < BlockLineCount; i++)
				for (int j = 0; j < BlockLineCount; j++)
				{
					var xCell = blockColumn + i;
					var yCell = blockRow + j;

					yield return indexPairs[xCell, yCell];
				}
		}

		public IEnumerable<IndexPair> GetRowPairs(IndexPair pair)
		{
			for (int i = 0; i < CellLineCount; i++)
			{
				yield return indexPairs[i, pair.Y];
			}
		}

		public IEnumerable<IndexPair> GetColumnPairs(IndexPair pair)
		{
			for (int j = CellLineCount - 1; j >= 0; j--)
			{
				yield return indexPairs[pair.X, j];
			}
		}

		public IEnumerable<IndexPair> GetBlockOrderedPairs()
		{
			var result = new List<IndexPair>();

			for (int j = CellLineCount - 1; j >= 0; j -= BlockLineCount)
				for (int i = 0; i < CellLineCount; i += BlockLineCount)
				{
					result.AddRange(GetBlockPairs(indexPairs[i, j]));
				}

			return result;
		}

		public IEnumerable<IndexPair> GetAllPairs()
		{
			for (int i = 0; i < CellLineCount; i++)
				for (int j = 0; j < CellLineCount; j++)
				{
					yield return indexPairs[i, j];
				}
		}
	}
}