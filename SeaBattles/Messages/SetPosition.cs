﻿using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Input;
using OpenTK;

namespace SeaBattles.Messages
{
    internal class SetPosition
    {
        private object target = null;
        private Vector2 velocity;
        private Vector3 position;
        private float angle;
        private float dt;

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

        public SetPosition(object target, Vector2 velocity, Vector3 position, float angle, float dt)
        {
            this.target = target;
            this.velocity = velocity;
            this.position = position;
            this.angle = angle;
            this.dt = dt;
        }
    }
}
