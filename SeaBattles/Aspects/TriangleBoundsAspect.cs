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
        private Vector2[] startVertices = new Vector2[3];
        private Vector2[] vertices = new Vector2[3];

        /// <summary>
        /// Центр треугольника
        /// </summary>
        private Vector2 position;
        float angle;
        /// <summary>
        /// Наибольшее рассояние от центра до одной из вершин
        /// </summary>
        float longestRadius = 0;

        public TriangleBoundsAspect(object owner, Vector2 a, Vector2 b, Vector2 c)
            : base(owner)
        {
            this.startVertices[0] = a;
            this.startVertices[1] = b;
            this.startVertices[2] = c;

            this.vertices[0] = a;
            this.vertices[1] = b;
            this.vertices[2] = c;

            this.position = new Vector2((a.X + b.X + c.X) / 3, (a.Y + b.Y + c.Y) / 3);
            this.angle = 0;

            for (int i = 0; i < 3; i++)
            {
                if ((vertices[i] - position).LengthSquared > longestRadius)
                    longestRadius = (vertices[0] - position).Length;
            }

            handlers.Add(typeof(SetPosition), new HandlerMethodDelegate(HandleUpdatePosition));

            RegisterAllStuff();
        }

        public override bool IntersectsWith(Vector2 point)
        {
            //http://www.blackpawn.com/texts/pointinpoly/default.html
            Vector2 v0 = vertices[2] - vertices[0];
            Vector2 v1 = vertices[1] - vertices[0];
            Vector2 v2 = point - vertices[0];

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

                vertices[0] = position + Misc.RotateVector(startVertices[0], angle);
                vertices[1] = position + Misc.RotateVector(startVertices[1], angle);
                vertices[2] = position + Misc.RotateVector(startVertices[2], angle);
            }
        }

        public override bool IntersectsWith(CircleBoundsAspect circle)
        {
            throw new NotImplementedException();
        }

        public override bool IntersectsWith(TriangleBoundsAspect triangle)
        {
            // сначала грубая проверка, аппроксимация окружностью
            float distanceBetweenCenters = (position - triangle.position).LengthFast;
            if (distanceBetweenCenters > longestRadius + triangle.longestRadius)
                return false;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (Misc.IntersectSegment(vertices[i], vertices[(i + 1) % 3], triangle.vertices[j], triangle.vertices[(j + 1) % 3]))
                    {
                        return true;
                    }
                }
            }

            // Если нет пересечений, то возможен ещё один вариант - один треугольник в другом
            // Но мы не знаем, какой в каком, поэтому проверяем оба по очереди
            // на включение всех трёх вершин внутри себя

            bool threeVerts = true;

            for (int i = 0; i < 3; i++)
            {
                if (!IntersectsWith(triangle.vertices[i]))
                {
                    threeVerts = false;
                    break;
                }
            }

            // этот треугольник не содержит внутри себя второй треугольник, проверяем другой
            if (!threeVerts)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (!triangle.IntersectsWith(vertices[i]))
                        return false;
                }
            }

            return true;
        }
    }
}


