using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;

namespace SeaBattles
{
    /// <summary>
    /// Упорядоченный массив из 3 вершин.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class Triangle<T>
    {
        private T a;
        private T b;
        private T c;

        public T this[int index]
        {
            get
            {
                if (index == 0)
                    return a;
                else
                    if (index == 1)
                        return b;
                    else
                        if (index == 2)
                            return c;
                        else
                            throw new ArgumentOutOfRangeException("index");
            }
            set
            {
                if (index == 0)
                    a = value;
                else
                    if (index == 1)
                        b = value;
                    else
                        if (index == 2)
                            c = value;
                        else
                            throw new ArgumentOutOfRangeException("index");
            }
        }

        public Triangle(T[] array)
        {
            if (array.Length > 3)
                throw new ArgumentException("Initial array too long", "array");

            a = array[0];
            b = array[1];
            c = array[2];
        }

        public Triangle(T first, T second, T third)
        {
            a = first;
            b = second;
            c = third;
        }
    }
}
