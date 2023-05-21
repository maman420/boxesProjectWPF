using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace roeiProjectWpf
{
    internal class Box : IComparable<Box>
    {
        private double _bottom;
        private double _height;

        public double Bottom => _bottom;
        public double Height => _height;
        public Queue<DateTime> Stock { get; }
        public int MaxStock { get; } = 20;
        public Queue<DateTime> StockList => Stock;

        public Box(double bottom, double height)
        {
            _bottom = bottom * bottom;
            _height = height;
            Stock = new Queue<DateTime>();
            Stock.Enqueue(NewExpirationDate());
        }
        public int CompareTo(Box other)
        {
            if (this._bottom == other._bottom)
                return this._height.CompareTo(other._height);
            return this._bottom.CompareTo(other._bottom);
        }
        public int CompareTo(double bottom, double height)
        {
            if (this._bottom == bottom)
                return this._height.CompareTo(height);
            return this._bottom.CompareTo(bottom);
        }
        public static DateTime NewExpirationDate() // because of multiple places of writing expiration date, this function is created to synchronize them
        {
            return DateTime.Now.AddMinutes(1);
        }
    }
}
