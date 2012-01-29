using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using SeaBattles.Messages;

namespace SeaBattles
{
    internal class TriangleBoundsAspect : BoundsAspect
    {
        //private Dictionary<Vector2, bool> outerVertices = new Dictionary<Vector2, bool>(3);

        // Координаты считаются от центра прямоугольника (объекта).
        // 
        private Vector2[] startVertices = new Vector2[3];
        private VertexDescription[] vertices = new VertexDescription[3];

        // Векторы от центра прямоугольника до вершин (чтобы было удобно поворачивать)
        // Конечные координаты вершин есть position + centerToVertsVectors[i]
        private Vector2[] centerToVertsVectors = new Vector2[3];
        // временный массив повёрнутых векторов вершин
        private Vector2[] rotatedVertsVectors = new Vector2[3];

        /// <summary>
        /// Центр треугольника
        /// </summary>
        private Vector2 position;
        private float angle;

        /// <summary>
        /// Наибольшее рассояние от центра до одной из вершин
        /// </summary>
        private float longestRadius = 0;

        internal override Vector2 Position
        {
            get { return position; }
        }

        internal override float Radius
        {
            get { return longestRadius; }
        }

        public static TriangleBoundsAspect Create(object owner, Vector2 a, Vector2 b, Vector2 c)
        {
            TriangleBoundsAspect aspect = new TriangleBoundsAspect(owner, a, b, c);
            aspect.RegisterAllStuff();
            return aspect;
        }

        private TriangleBoundsAspect(object owner, Vector2 a, Vector2 b, Vector2 c)
            : base(owner)
        {
            this.startVertices[0] = a;
            this.startVertices[1] = b;
            this.startVertices[2] = c;

            for (int i = 0; i < 3; i++)
                this.vertices[i] = new VertexDescription();

            this.vertices[0].Position = a;
            this.vertices[1].Position = b;
            this.vertices[2].Position = c;

            this.position = new Vector2((a.X + b.X + c.X) / 3, (a.Y + b.Y + c.Y) / 3);
            this.angle = 0;

            this.centerToVertsVectors[0] = a - position;
            this.centerToVertsVectors[1] = b - position;
            this.centerToVertsVectors[2] = c - position;

            for (int i = 0; i < 3; i++)
            {
                if ((vertices[i].Position - position).LengthSquared > longestRadius)
                    longestRadius = (vertices[0].Position - position).Length;

                this.vertices[i].IsOuter = false;
            }

            // больше не подписываемся на SetPosition, так как нашим положением явно управляет родительский объект
            //handlers.Add(typeof(SetPosition), new HandlerMethodDelegate(HandleUpdatePosition));
        }

        /// <summary>
        /// Проверка нахождения точки внутри треугольника. Треугольник задан классом Triangle.
        /// </summary>
        /// <remarks>Решил сделать другую версию метода, чтобы избежать лишнего копирования объектов
        /// в памяти при передаче параметров, хотя разница в трёх строчках.</remarks>
        /// <param name="triangle"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        protected bool TriangleIntersectsWithPoint(Triangle<Vector2> triangle, Vector2 point)
        {
            //http://www.blackpawn.com/texts/pointinpoly/default.html
            Vector2 v0 = triangle[2] - triangle[0];
            Vector2 v1 = triangle[1] - triangle[0];
            Vector2 v2 = point - triangle[0];

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

        public override bool IntersectsWith(Vector2 point)
        {
            //http://www.blackpawn.com/texts/pointinpoly/default.html
            Vector2 v0 = vertices[2].Position - vertices[0].Position;
            Vector2 v1 = vertices[1].Position - vertices[0].Position;
            Vector2 v2 = point - vertices[0].Position;

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
                this.angle = setPosition.Angle;

                for (int i = 0; i < 3; i++)
                {
                    vertices[i].Position = position + Misc.RotateVector(startVertices[i], angle);
                }
            }
        }

        public override bool IntersectsWith(CircleBoundsAspect circle)
        {
            // сначала грубая проверка, аппроксимация окружностью
            float distanceBetweenCenters = (position - circle.Position).LengthFast;
            if (distanceBetweenCenters > longestRadius + circle.Radius)
                return false;

            // Для произвольных выпуклых объектов используем Gilbert–Johnson–Keerthi distance algorithm
            // тут мы используем теорему о разделяющей оси http://www.gamedev.ru/code/terms/SAT
            // небольшие пояснения тут http://www.ryandudley.com/docs/spheretri.pdf

            // сначала проверяем 3 прямые, образованные сторонами треугольника
            for (int i = 0; i < 3; i++)
            {
                Vector2 start = vertices[i].Position;
                Vector2 end = vertices[(i + 1) % 3].Position;

                if (!TestSegmentIntersection(circle, ref start, ref end))
                    return false;
            }

            // теперь проверяем 3 прямые, проходящие через центр круга и каждую из вершин треугольника
            for (int i = 0; i < 3; i++)
            {
                Vector2 start = vertices[i].Position;
                Vector2 end = circle.Position;

                if (!TestSegmentIntersection(circle, ref start, ref end))
                    return false;
            }

            return true;
        }

        private bool TestSegmentIntersection(CircleBoundsAspect circle, ref Vector2 start, ref Vector2 end)
        {
            // проецируем каждую вершину треугольника на прямую
            float triangleProjection;
            float triangleMinProjection = float.MaxValue;
            float triangleMaxProjection = float.MinValue;

            for (int j = 0; j < 3; j++)
            {
                triangleProjection = Misc.GetProjection(vertices[j].Position, start, end);
                if (triangleMinProjection > triangleProjection)
                    triangleMinProjection = triangleProjection;

                if (triangleMaxProjection < triangleProjection)
                    triangleMaxProjection = triangleProjection;
            }
            // получили 2 точки - проекция треугольника на одну из прямых
            // проецируем центр круга и прибавляем к нему по радиусу с каждой стороны
            // так как GetProjection возвращает координату, зависящую от длины отрезка,
            // нужно выразить радиус круга в длине отрезка
            float circleCenterProjection = Misc.GetProjection(circle.Position, start, end);
            float radiusLength = circle.Radius / (start - end).LengthFast;
            float circleMinProjection = circleCenterProjection - radiusLength;
            float circleMaxProjection = circleCenterProjection + radiusLength;

            // сравниваем, что cMin < tMax или cMax > tMin
            // если это не так, то круг и треугольник не пересекаются и дальше можно не проверять
            if (circleMinProjection > triangleMaxProjection || circleMaxProjection < triangleMinProjection)
                return false;
            else
                return true;
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
                    if (Misc.IntersectSegment(vertices[i].Position, vertices[(i + 1) % 3].Position, triangle.vertices[j].Position, triangle.vertices[(j + 1) % 3].Position))
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
                if (!IntersectsWith(triangle.vertices[i].Position))
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
                    if (!triangle.IntersectsWith(vertices[i].Position))
                        return false;
                }
            }

            return true;
        }

        public override bool IntersectsWith(Triangle<Vector2> triangle)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (Misc.IntersectSegment(vertices[i].Position, vertices[(i + 1) % 3].Position, triangle[j], triangle[(j + 1) % 3]))
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
                if (!IntersectsWith(triangle[i]))
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
                    if (!Misc.TriangleIntersectsWithPoint(triangle, vertices[i].Position))
                        return false;
                }
            }

            return true;
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
            // перемещаем центр треугольника
            //
            Vector2 rotated = Misc.RotateVector(displacementFromParent, angle);
            this.position = parentPosition + rotated;

            // поворачиваем вершины
            for (int i = 0; i < 3; i++)
            {
                rotatedVertsVectors[i] = Misc.RotateVector(centerToVertsVectors[i], angle);
                vertices[i].Position = this.position + rotatedVertsVectors[i];
            }
        }

        internal override Vector2[] Triangulate()
        {
            Vector2[] verts = new Vector2[3];
            for (int i = 0; i < 3; i++)
            {
                verts[i] = vertices[i].Position;
            }
            return verts;
        }

        internal override void SetVertexAsOuter(Vector2 outerPoint)
        {
            for (int i = 0; i < 3; i++)
            {
                if (outerPoint == vertices[i].Position)
                {
                    vertices[i].IsOuter = true;
                    break;
                }
            }
        }


    }
}


