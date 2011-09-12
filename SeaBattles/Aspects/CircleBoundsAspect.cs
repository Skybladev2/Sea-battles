using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using SeaBattles.Messages;

namespace SeaBattles
{
    internal class CircleBoundsAspect : BoundsAspect
    {
        private Vector2 position;
        private float radius;

        internal override Vector2 Position
        {
            get { return position; }
        }

        internal override float Radius
        {
            get { return radius; }
        }

        public CircleBoundsAspect(object owner, Vector2 position, float radius)
            : base(owner)
        {
            this.position = position;
            this.radius = radius;

            handlers.Add(typeof(SetPosition), new HandlerMethodDelegate(HandleUpdatePosition));

            RegisterAllStuff();
        }

        public override bool IntersectsWith(Vector2 point)
        {
            if ((point - position).Length <= radius)
                return true;
            else
                return false;
        }

        public override bool IntersectsWith(CircleBoundsAspect circle)
        {
            if ((this.radius + circle.Radius) >= (this.position - circle.Position).Length)
                return true;
            else
                return false;
        }

        internal override object GetOwner()
        {
            return this.owner;
        }

        private void HandleUpdatePosition(object message)
        {
            SetPosition setPosition = (SetPosition)message;
            if (this.owner == setPosition.Target)
            {
                this.position = setPosition.Position.Xy;
            }
        }

        public override bool IntersectsWith(TriangleBoundsAspect triangle)
        {
            return triangle.IntersectsWith(this);
        }
    }
}
