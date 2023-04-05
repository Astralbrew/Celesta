using System.Collections.Generic;
using System.Linq;

namespace Astralbrew.Celesta.Utils
{
    internal class ArraySequenceEqualityComparer<T> : IEqualityComparer<T[]>
    {
        public bool Equals(T[] x, T[] y)
        {
            if (x == null && y == null) return true;
            return x.SequenceEqual(y);
        }

        public int GetHashCode(T[] obj)
        {
            return obj.Select(x => x.GetHashCode()).Aggregate((x, y) => x + y);
        }
    }
}
