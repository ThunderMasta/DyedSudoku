using System;

namespace DyedSudoku
{
    public class GameFieldModel
    {
        private const byte cellLineCount = 9;
        private const byte blockLineCount = 3;

        // Zero is left bottom couse CTM is inversed
        private byte[,] field;

        public byte CellLineCount
        {
            get { return cellLineCount; }
        }

        public byte BlockLineCount
        {
            get { return blockLineCount; }
        }

        public GameFieldModel()
        {
            InitField();
        }

        private void InitField()
        {
            field = new byte[CellLineCount, CellLineCount];

            for (byte i = 0; i < CellLineCount; i++)
                for (byte j = 0; j < CellLineCount; j++)
                    field[i, j] = (byte) ((i + j) % 9 + 1);
        }

        public byte GetItem(byte x, byte y)
        {
            if (x >= CellLineCount || y >= CellLineCount)
                throw new ArgumentOutOfRangeException();

            return field[x, y];
        }
    }
}