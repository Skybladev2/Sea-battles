using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattles.Messages
{
    /// <summary>
    /// Тестовое сообщение, чтобы при прекращении столкновения рисовать примитивы прежним цветом.
    /// </summary>
    internal class BoundSetCollision
    {
        private BoundSetAspect[] boundObjects;

        public BoundSetAspect[] Objects
        {
            get { return boundObjects; }
        }

        public BoundSetCollision(BoundSetAspect boundSet1, BoundSetAspect boundSet2)
        {
            this.boundObjects = new BoundSetAspect[2];
            this.boundObjects[0] = boundSet1;
            this.boundObjects[1] = boundSet2;
        }
    }
}
