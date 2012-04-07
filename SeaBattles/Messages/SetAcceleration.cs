using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;

namespace SeaBattles.Messages
{
    internal class SetAcceleration
    {
        private object owner;
        private Vector2 acceleration;
        private float? targetSpeed;

        public Vector2 Acceleration
        {
            get { return acceleration; }
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

        public SetAcceleration(object owner, float? targetSpeed, Vector2 acceleration)
        {
            this.owner = owner;
            this.targetSpeed = targetSpeed;
            this.acceleration = acceleration;
        }
    }
}
