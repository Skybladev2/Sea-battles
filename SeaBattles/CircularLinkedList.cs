using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattles
{
    class CircularLinkedList<T> : LinkedList<T>
    {
        public CircularLinkedList()
        {

        }

        public CircularLinkedList(IEnumerable<T> list, bool addClosure) : base(list)
        {
            if (addClosure)
                this.AddFirst(this.Last);
        }

        public CircularLinkedList(Triangle<T> triangle)
        {
            for (int i = 0; i < 3; i++)
            {
                this.AddLast(triangle[i]);
            }
            this.AddLast(triangle[0]);
        }

        public LinkedListNode<T> NextOrFirst(LinkedListNode<T> current)
        {
            if (current.Next == null)
                return current.List.First;
            return current.Next;
        }

        public LinkedListNode<T> PreviousOrLast(LinkedListNode<T> current)
        {
            if (current.Previous == null)
                return current.List.Last;
            return current.Previous;
        }

        public CircularLinkedList<T> Reverse()
        {
            CircularLinkedList<T> temp = new CircularLinkedList<T>();
            foreach (var current in this)
                temp.AddFirst(current);

            return temp;
        }
    }
}
