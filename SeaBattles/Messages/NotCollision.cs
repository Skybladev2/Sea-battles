using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattles.Messages
{
    /// <summary>
    /// Тестовое сообщение, чтобы при прекращении столкновения рисовать примитивы прежним цветом.
    /// </summary>
    internal class NotCollision
    {
        private BoundsAspect[] boundObjects;

        public BoundsAspect[] Objects
        {
            get { return boundObjects; }
        }

        public NotCollision(BoundsAspect bounds1, BoundsAspect bounds2)
        {
            this.boundObjects = new BoundsAspect[2];
            this.boundObjects[0] = bounds1;
            this.boundObjects[1] = bounds2;
        }
    }
}
