using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Input;
using OpenTK;

namespace SeaBattles.Messages
{
    internal class ApplyForces
    {
        private object owner;
        private Vector2[] forces; 
        
        public Vector2[] Forces
        {
            get { return forces; }
            //set { button = value; }
        }

        public object Owner
        {
            get { return owner; }
        }

        public ApplyForces(Vector2[] forces)
        {
            this.forces = forces;
        }
    }
}
