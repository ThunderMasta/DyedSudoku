using System;

namespace DyedSudoku
{
    public class GameFieldModel
    {
        private const short cellLineCount = 9;
        private const short blockLineCount = 3;

        public short CellLineCount
        {
            get { return cellLineCount; }
        }

        public short BlockLineCount
        {
            get { return blockLineCount; }
        }

        public GameFieldModel()
        {
        }
    }
}