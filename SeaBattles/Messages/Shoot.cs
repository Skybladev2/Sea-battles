using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Input;
using OpenTK;

namespace SeaBattles.Messages
{
    internal class Shoot
    {
        private object owner;
        private Vector3 direction;
        private Vector3 from;
        private Vector3 shooterVelocity;

        public Vector3 Direction
        {
            get { return direction; }
        }

        public Vector3 From
        {
            get { return from; }
        }

        public Vector3 ShooterVelocity
        {
            get { return shooterVelocity; }
        }

        public object Owner
        {
            get
            {
                return owner;
            }
        }

        public Shoot(object owner, Vector3 from, Vector3 direction, Vector3 shooterVelocity)
        {
            this.owner = owner;
            this.from = from; 
            this.direction= direction;
            this.shooterVelocity = shooterVelocity; 
        }
    }
}
