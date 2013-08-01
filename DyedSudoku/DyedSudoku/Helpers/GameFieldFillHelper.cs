using System;
using System.Collections.Generic;
using System.Linq;

namespace DyedSudoku
{
    public static class GameFieldFillHelper
    {
        private static readonly byte[] numberList = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        public static void Init(GameFieldModel model)
        {
            var rand = new Random();

            for (byte i = 0; i < model.CellLineCount; i++)
                for (byte j = 0; j < model.CellLineCount; j++)
                    GenerateNumber(model, rand, i, j);
        }

        private static void GenerateNumber(GameFieldModel model, Random rand, byte x, byte y)
        {
            var availableNumbers = GetAvailableNumbers(model, x, y).ToArray();

            var availableNumbersIndex = (byte)rand.Next(availableNumbers.Length);

            model.SetItem(availableNumbers[availableNumbersIndex], x, y);
        }

        private static IEnumerable<byte> GetAvailableNumbers(GameFieldModel model, byte x, byte y)
        {
            List<byte> alreadyGeneratedNumbers = new List<byte>();

            for (byte i = 0; i < x; i++)
            {
                var number = model.GetItem(i, y);
                if (number == 0 || alreadyGeneratedNumbers.Contains(number))
                    continue;

                alreadyGeneratedNumbers.Add(number);
            }

            for (byte j = 0; j < y; j++)
            {
                var number = model.GetItem(x, j);
                if (number == 0 || alreadyGeneratedNumbers.Contains(number))
                    continue;

                alreadyGeneratedNumbers.Add(number);
            }

            if (alreadyGeneratedNumbers.Count == model.CellLineCount)
                return new byte[] { 0 };

            return numberList.Except(alreadyGeneratedNumbers);
        }
    }
}

