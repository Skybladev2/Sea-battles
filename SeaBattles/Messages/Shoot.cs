using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Input;
using OpenTK;

namespace SeaBattles.Messages
{
    internal class Shoot
    {
        public Vector3 direction;
        public Vector3 from;

        public Shoot(Vector3 from, Vector3 direction)
        {
            this.from = from; 
            this.direction= direction;
        }
    }
}
