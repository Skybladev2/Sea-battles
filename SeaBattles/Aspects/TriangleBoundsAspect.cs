using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using SeaBattles.Messages;

namespace SeaBattles
{
    internal class TriangleBoundsAspect : BoundsAspect
    {
        // Координаты считаются от центра прямоугольника (объекта).
        // 
        private Vector2 startA;
        private Vector2 startB;
        private Vector2 startC;

        private Vector2 a;
        private Vector2 b;
        private Vector2 c;

        // центр треугольника
        private Vector2 position;
        float angle;

        public TriangleBoundsAspect(object owner, Vector2 a, Vector2 b, Vector2 c)
            : base(owner)
        {
            this.startA = a;
            this.startB = b;
            this.startC = c;

            this.position = new Vector2((a.X + b.X + c.X) / 3, (a.Y + b.Y + c.Y) / 3);
            this.angle = 0;

            handlers.Add(typeof(SetPosition), new HandlerMethodDelegate(HandleUpdatePosition));

            RegisterAllStuff();
        }

        public override bool IntersectsWith(Vector2 point)
        {
            //http://www.blackpawn.com/texts/pointinpoly/default.html
            Vector2 v0 = c - a;
            Vector2 v1 = b - a;
            Vector2 v2 = point - a;

            float dot00 = Vector2.Dot(v0, v0);
            float dot01 = Vector2.Dot(v0, v1);
            float dot02 = Vector2.Dot(v0, v2);
            float dot11 = Vector2.Dot(v1, v1);
            float dot12 = Vector2.Dot(v1, v2);

            // Compute barycentric coordinates
            float invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
            float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
            float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

            //MessageDispatcher.Post(new TraceText("u = " + u + ", v = " + v));
            return (u > 0) && (v > 0) && (u + v < 1);
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
                this.angle = setPosition.Angle;

                a = position + Misc.RotateVector(startA, angle);
                b = position + Misc.RotateVector(startB, angle);
                c = position + Misc.RotateVector(startC, angle);
            }
        }
    }
}


