using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattles
{
    /// <summary>
    /// Пара значений. Порядок не имеет значения.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    struct Pair<T> : IEquatable<Pair<T>>
    {
        readonly T first;
        readonly T second;

        public Pair(T first, T second)
        {
            this.first = first;
            this.second = second;
        }

        public T First { get { return first; } }
        public T Second { get { return second; } }

        public override int GetHashCode()
        {
            return first.GetHashCode() ^ second.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return Equals((Pair<T>)obj);
        }

        public bool Equals(Pair<T> other)
        {
            return other.first.Equals(first) && other.second.Equals(second) ||
                    other.first.Equals(second) && other.second.Equals(first);
        }
    }
}
