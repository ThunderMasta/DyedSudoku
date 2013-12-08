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

		private static IEnumerable<sbyte> GetHeuristicsAvailableNumbers(GameFieldModel model, IndexPair pair, int rank = 1)
		{
			var blockDict = GetBlockAvailableDict(model, pair, rank);
			var available = GetAvailableByDict(blockDict, pair).ToList();

			if (available.Count < 2)
				return available;

			var rowDict = GetRowAvailableDict(model, pair, rank);
			available = available.Intersect(GetAvailableByDict(rowDict, pair)).ToList();

			if (available.Count < 2)
				return available;

			var columnDict = GetColumnAvailableDict(model, pair, rank);
			return available.Intersect(GetAvailableByDict(columnDict, pair));
		}

		private static IEnumerable<sbyte> GetAvailableByDict(Dictionary<IndexPair, IEnumerable<sbyte>> dict, IndexPair pair)
		{
			var currentPairValue = dict.First(item => item.Key.X == pair.X && item.Key.Y == pair.Y).Value;
			if (currentPairValue.Count() == 1)
				return currentPairValue;

			while (/*RemoveFilledGroupNumbers(dict, 3) ||
                   SetGroupNumbersIfFilled(dict, 3) ||
				RemoveFilledGroupNumbers(dict, 2) ||
				SetGroupNumbersIfFilled(dict, 2) ||
				RemoveFilledGroupNumbers(dict, 1) ||
				SetGroupNumbersIfFilled(dict, 1)*/
				RemoveSingleNumbers(dict) ||
				SetNumbersIfSingle(dict))
			{
				currentPairValue = dict.First(item => item.Key.X == pair.X && item.Key.Y == pair.Y).Value;
				if (currentPairValue.Count() == 1)
					return currentPairValue;
			}

			return dict.First(item => item.Key.X == pair.X && item.Key.Y == pair.Y).Value;
		}

		private static bool SetGroupNumbersIfFilled(Dictionary<IndexPair, IEnumerable<sbyte>> dict, int count)
		{
			var isModified = false;

			var manyValuesItems = dict.Where(item => item.Value.Count() > count).ToList();
			var subItemsList = manyValuesItems.SelectMany(item => item.Value.OrderBy(x => x).GetAllSubItems(count));

			var distinctSubItems = new List<List<sbyte>>();
			foreach (var subItems in subItemsList)
			{
				var currentSubItems = subItems;
				if (!distinctSubItems.Any(item => item.SequenceEqual(currentSubItems)))
					distinctSubItems.Add(currentSubItems);
			}

			foreach (var groupItems in distinctSubItems)
			{
				if (groupItems.Any(item => dict.Count(x => x.Value.Contains(item)) != count))
					continue;

				var currentItems = groupItems;
				var itemsContainsGroupItems = dict.Where(item => item.Value.Contains(currentItems)).ToList();

				if (itemsContainsGroupItems.Count == count)
				{
					foreach (var item in itemsContainsGroupItems)
					{
						dict.Remove(item.Key);
						dict.Add(item.Key, currentItems);
						isModified = true;
					}
				}
			}

			return isModified;
		}

		private static bool RemoveFilledGroupNumbers(Dictionary<IndexPair, IEnumerable<sbyte>> dict, int count)
		{
			var isModified = false;

			var filledGroupItems = dict
                .Select(item => item.Value.ToList())
				.Where(item => item.Count == count && dict.Count(x => x.Value.SequenceEqual(item)) == count)
                .ToList();

			foreach (var filledGroupItem in filledGroupItems)
			{
				var currentItem = filledGroupItem;
				var itemsContainsFilledGroupItems = dict.Where(item => !item.Value.SequenceEqual(currentItem) && item.Value.Any(currentItem.Contains)).ToList();
				foreach (var dictItem in itemsContainsFilledGroupItems)
				{
					dict.Remove(dictItem.Key);
					dict.Add(dictItem.Key, dictItem.Value.Except(filledGroupItem));
					isModified = true;
				}
			}

			return isModified;
		}

		private static bool SetNumbersIfSingle(Dictionary<IndexPair, IEnumerable<sbyte>> dict)
		{
			var isModified = false;

			for (sbyte i = 1; i <= 9; i++)
			{
				var currentItem = i;
				var itemsContainsNumber = dict.Where(item => item.Value.Contains(currentItem)).ToList();

				if (itemsContainsNumber.Count == 1)
				{
					var itemContainsNumber = itemsContainsNumber[0];
					if (itemContainsNumber.Value.Count() > 1)
					{
						dict.Remove(itemContainsNumber.Key);
						dict.Add(itemContainsNumber.Key, currentItem.Yield());
						isModified = true;
					}
				}
			}

			return isModified;
		}

		private static bool RemoveSingleNumbers(Dictionary<IndexPair, IEnumerable<sbyte>> dict)
		{
			var isModified = false;

			var singleNumbers = dict.Where(item => item.Value.Count() == 1).SelectMany(item => item.Value).ToList();

			foreach (var singleNumber in singleNumbers)
			{
				var currentNumber = singleNumber;
				var itemsContainsNumber = dict.Where(item => item.Value.Count() > 1 && item.Value.Contains(currentNumber)).ToList();
				foreach (var dictItem in itemsContainsNumber)
				{
					dict.Remove(dictItem.Key);
					dict.Add(dictItem.Key, dictItem.Value.Except(currentNumber.Yield()));
					isModified = true;
				}
			}

			return isModified;
		}

		private static Dictionary<IndexPair, IEnumerable<sbyte>> GetBlockAvailableDict(GameFieldModel model, IndexPair pair, int rank)
		{
			var blockPairs = model.GetBlockPairs(pair);
			return GetAvailableDictByPairs(model, blockPairs, pair, rank);
		}

		private static Dictionary<IndexPair, IEnumerable<sbyte>> GetRowAvailableDict(GameFieldModel model, IndexPair pair, int rank)
		{
			var rowPairs = model.GetRowPairs(pair);
			return GetAvailableDictByPairs(model, rowPairs, pair, rank);
		}

		private static Dictionary<IndexPair, IEnumerable<sbyte>> GetColumnAvailableDict(GameFieldModel model, IndexPair pair, int rank)
		{
			var columnPairs = model.GetColumnPairs(pair);
			return GetAvailableDictByPairs(model, columnPairs, pair, rank);
		}

		private static Dictionary<IndexPair, IEnumerable<sbyte>> GetAvailableDictByPairs(GameFieldModel model, IEnumerable<IndexPair> pairs, IndexPair checkedPair, int rank)
		{
			var dict = new Dictionary<IndexPair, IEnumerable<sbyte>>();

			foreach (var pair in pairs)
			{
				if (rank > 1 && model.IsItemNotAvailable(pair, checkedPair))
				{
					model.SelectedPair = pair;
					dict.Add(pair, GetHeuristicsAvailableNumbers(model, pair, rank - 1));
					model.SelectedPair = null;
				}
				else
				{
					dict.Add(pair, model.GetAvailableNumbers(pair, checkedPair));
				}
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
			var orderedPairs = model.GetAllPairs();
			var pairs = rand.Shake(orderedPairs);

			var rank = 1;
			const int maxRank = 2;
			do
			{
				var pair = GetAvailableToHidePair(model, pairs, token, rank);

				if (pair == null)
				{
					if (rank == maxRank)
						break;

					rank++;
					orderedPairs = model.GetAllPairs().Where(model.GetItemVisible);
					pairs = rand.Shake(orderedPairs);
				}
				else
				{
					pairs.Remove(pair);
				}
			}
			while(true);

			return model.VisibleCount <= maxVisibleNumbers;
		}

		private static IndexPair GetAvailableToHidePair(GameFieldModel model, List<IndexPair> pairs, CancellationToken token, int rank)
		{
			foreach (var pair in pairs.ToList())
			{
				token.ThrowIfCancellationRequested();

				model.SetItemVisible(pair, false);

				if (GetHeuristicsAvailableNumbers(model, pair, rank).Count() <= 1)
					return pair;

				model.SetItemVisible(pair, true);

				pairs.Remove(pair);
			}

			return null;
		}
	}
}

