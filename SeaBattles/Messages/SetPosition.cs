using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Input;
using OpenTK;

namespace SeaBattles.Messages
{
    /// <summary>
    /// Сообщает о том, что положение объекта изменилось.
    /// </summary>
    internal class SetPosition
    {
        private object target = null;
        private Vector2 velocity;
        private Vector3 position;
        private float angle;
        private float dt;
        private Vector2 facing;

        /// <summary>
        /// Чьё положение изменилось. Это аспект-родитель, а не тот аспект, кто послал сообщение.
        /// </summary>
 
        public object Target
        {
            get { return target; }
            //set { target = value; }
        }

        public float Angle
        {
            get { return angle; }
            //set { angle = value; }
        }

        public Vector3 Position
        {
            get { return position; }
            //set { position = value; }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
            //set { velocity = value; }
        }

        public float Dt
        {
            get { return dt; }
        }

        public Vector2 Facing
        {
            get { return facing; }
        }

        public SetPosition(object target, Vector2 velocity, Vector3 position, float angle, float dt, Vector2 facing)
        {
            this.target = target;
            this.velocity = velocity;
            this.position = position;
            this.angle = angle;
            this.dt = dt;
            this.facing = facing;
        }
    }
}
