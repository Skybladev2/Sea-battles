using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattles.Messages
{
    internal class SetSpeed
    {
        private object owner;
        private float speed;

        public object Owner
        {
            get { return owner; }
        }

        public float Speed
        {
            get { return speed; }
            //set { speed = value; }
        }

        public SetSpeed(object owner, float speed)
        {
            this.owner = owner;
            this.speed = speed;
        }
    }
}
