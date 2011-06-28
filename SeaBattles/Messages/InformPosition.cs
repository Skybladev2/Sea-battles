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

        public object InformedObject
        {
            get { return informedObject; }
            //set { informedObject = value; }
        }

        private Vector2 velocity;
        private Vector2 position;
        //private float angle;

        public object Target
        {
            get { return target; }
            //set { target = value; }
        }

        //public float Angle
        //{
        //    get { return angle; }
        //    //set { angle = value; }
        //}

        public Vector2 Position
        {
            get { return position; }
            //set { position = value; }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
            //set { velocity = value; }
        }

        public InformPosition(object target, object informedObject, Vector2 velocity, Vector2 position /*, float angle*/)
        {
            this.target = target;
            this.informedObject = informedObject;
            this.velocity = velocity;
            this.position = position;
            //this.angle = angle;
        }
    }
}
