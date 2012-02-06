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

        public static CircleBoundsAspect Create(object owner, Vector2 position, float radius)
        {
            CircleBoundsAspect aspect = new CircleBoundsAspect(owner, position, radius);
            aspect.RegisterAllStuff();
            return aspect;
        }

        private CircleBoundsAspect(object owner, Vector2 position, float radius)
            : base(owner)
        {
            this.position = position;
            this.radius = radius;

            messageHandler.Handlers.Add(typeof(SetPosition), new HandlerMethodDelegate(HandleUpdatePosition));

            //RegisterAllStuff();
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

        //internal override object GetOwner()
        //{
        //    return this.owner;
        //}

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

        public override bool IntersectsWith(BoundsAspect bound)
        {
            return bound.IntersectsWith(this);
        }

        //internal override void SetParentPosition(Vector2 parentPosition)
        //{
        //    this.displacementFromParent = parentPosition - this.position;
        //}

        internal override void UpdatePosition(Vector2 parentPosition, float angle)
        {
            // перемещаем центр круга
            //
            Vector2 rotated = Misc.RotateVector(displacementFromParent, angle);
            this.position = parentPosition + rotated;
        }

        internal override Vector2[] Triangulate()
        {
            int segments = 8;
            Vector2[] vertices = new Vector2[segments];

            float angleStep = (float)(2 * Math.PI / segments);
            for (int i = 0; i < segments; i++)
            {
                vertices[i] = new Vector2((float)Math.Cos(angleStep * i) * radius, (float)Math.Sin(angleStep * i) * radius);
            }

            return vertices;
        }

        internal override void SetVertexAsOuter(Vector2 secondPoint)
        {
            throw new NotImplementedException();
        }

        public override bool IntersectsWith(Triangle<Vector2> stretchedOutlinePart)
        {
            return Misc.CircleIntersectsWithTriangle(this, stretchedOutlinePart);
        }
    }
}
