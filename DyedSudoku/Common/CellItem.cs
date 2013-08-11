using System;

namespace Common
{
    public class CellItem
    {
        public sbyte Number { get; set; }

        public bool IsVisible { get; set; }

        public bool IsEmpty
        {
            get { return Number == 0; }
        }

        public CellItem()
        {
            IsVisible = true;
        }

        public void ResetNumber()
        {
            Number = 0;
        }

        public void ResetIsVisible()
        {
            IsVisible = true;
        }
    }
}

