using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattles.Messages
{
    /// <summary>
    /// Тестовое сообщение, чтобы при прекращении столкновения рисовать примитивы прежним цветом.
    /// </summary>
    internal class BoundSetNotCollision
    {
        private BoundSetAspect[] boundObjects;

        public BoundSetAspect[] Objects
        {
            get { return boundObjects; }
        }

        public BoundSetNotCollision(BoundSetAspect bounds1, BoundSetAspect bounds2)
        {
            this.boundObjects = new BoundSetAspect[2];
            this.boundObjects[0] = bounds1;
            this.boundObjects[1] = bounds2;
        }
    }
}
