using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Input;
using OpenTK;

namespace SeaBattles.Messages
{
    /// <summary>
    /// Сообщает о координатах и скорости объекта.
    /// В отличие от SetPosition не требует от получателя по обновления положения объекта.
    /// </summary>
    internal class InformPosition
    {
        /// <summary>
        /// Кто получает данное сообщение.
        /// </summary>
        private object target = null;

        /// <summary>
        /// Объект, данные о котором передаются.
        /// </summary>
        private object informedObject = null;

        private Vector2 velocity;
        private Vector2 position;
        private Vector2 prevPosition;
        private float lastDT;

        public object InformedObject
        {
            get { return informedObject; }
        }

        public object Target
        {
            get { return target; }
        }

        public Vector2 Position
        {
            get { return position; }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
        }

        public Vector2 PrevPosition
        {
            get
            {
                return this.prevPosition;
            }
        }

        public float LastDT
        {
            get
            {
                return this.lastDT;
            }
        }

        public InformPosition(object target,
                            object informedObject,
                            Vector2 velocity,
                            Vector2 position,
                            Vector2 prevPosition,
                            float lastDT)
        {
            this.target = target;
            this.informedObject = informedObject;
            this.velocity = velocity;
            this.position = position;
            this.lastDT = lastDT; 
        }
    }
}
