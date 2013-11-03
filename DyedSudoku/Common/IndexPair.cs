using System;

namespace Common
{
    public class IndexPair
    {
        public readonly int X;
        public readonly int Y;

        public IndexPair(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj.GetType() != GetType())
                return false;

            var second = (IndexPair)obj;

            return X == second.X && Y == second.Y;
        }
    }
}

