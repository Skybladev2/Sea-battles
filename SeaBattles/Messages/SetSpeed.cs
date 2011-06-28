using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaBattles.Messages
{
    internal class SetSpeed
    {
        private float speed;

        public float Speed
        {
            get { return speed; }
            //set { speed = value; }
        }

        public SetSpeed(float speed)
        {
            this.speed = speed;
        }
    }
}
