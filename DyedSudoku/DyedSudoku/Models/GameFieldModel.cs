using System;
using System.Collections.Generic;
using System.Linq;

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
            GameFieldFillHelper.Init(this);
        }

        public byte GetItem(byte x, byte y)
        {
            return field[x, y];
        }

        public void SetItem(byte value, byte x, byte y)
        {
            field[x, y] = value;
        }
    }
}