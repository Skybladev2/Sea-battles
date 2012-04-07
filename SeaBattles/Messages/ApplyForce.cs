using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Input;
using OpenTK;

namespace SeaBattles.Messages
{
    internal class ApplyForce
    {
        private object owner;
        private Vector2 force; 
        
        public Vector2 Force
        {
            get { return force; }
            //set { button = value; }
        }

        public object Owner
        {
            get { return owner; }
        }

        public ApplyForce(object owner, Vector2 force)
        {
            this.owner = owner;
            this.force = force;
        }
    }
}
