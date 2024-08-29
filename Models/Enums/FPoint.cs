using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace Caro.Models.Enums
{
    public class FPoint
    {
        public int X { get; set; }
        public int Y { get; set; }

        public static readonly FPoint NULL = new FPoint(int.MinValue, int.MinValue);
        public FPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        //Overloading == 
        public override bool Equals(object? obj)
        {
            if (obj is FPoint other)
            {
                return X == other.X && Y == other.Y;
            }
            return false;
        }

        public override int GetHashCode()
        {
            // Sử dụng HashCode.Combine trong .NET Core 2.1 trở lên
            return HashCode.Combine(X, Y);
        }

        public static FPoint Point(int x, int y)
        {
            return new FPoint(x, y);
        }

        public override string ToString()
        {
            return X.ToString() + ',' + Y.ToString();
        }
    }
}
