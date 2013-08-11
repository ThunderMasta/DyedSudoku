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

        public sbyte CellLineCount
        {
            get { return cellLineCount; }
        }

        public sbyte BlockLineCount
        {
            get { return blockLineCount; }
        }

        public bool IsAnyZeroCell
        {
            get
            {
                for (int i = 0; i < CellLineCount; i++)
                    for (int j = 0; j < CellLineCount; j++)
                    {
                        if (field[i, j].Number == 0)
                            return true;
                    }

                return false;
            }
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
    }
}