using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;

namespace SeaBattles.Messages
{
    internal class AddForwardAcceleration
    {
        private object owner;
        private Vector2 targetAcceleration;

        public Vector2 TargetAcceleration
        {
            get { return targetAcceleration; }
            //set { acceleration = value; }
        }
        
        public object Owner
        {
            get { return owner; }
        }

        public AddForwardAcceleration(object owner, Vector2 targetAcceleration)
        {
            this.owner = owner;
            this.targetAcceleration = targetAcceleration;
        }
    }
}
