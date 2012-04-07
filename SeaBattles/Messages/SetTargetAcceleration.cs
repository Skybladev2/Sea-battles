using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattles.Messages
{
    internal class SetTargetAcceleration
    {
        private object owner;
        private float targetAcceleration;
        private float? targetSpeed;

        public float TargetAcceleration
        {
            get { return targetAcceleration; }
            //set { acceleration = value; }
        }
        
        public object Owner
        {
            get { return owner; }
        }

        public float? TargetSpeed
        {
            get { return targetSpeed; }
            //set { speed = value; }
        }

        public SetTargetAcceleration(object owner, float? targetSpeed, float targetAcceleration)
        {
            this.owner = owner;
            this.targetSpeed = targetSpeed;
            this.targetAcceleration = targetAcceleration;

        }
    }
}
