using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using System.Drawing;

namespace SeaBattles
{
    internal class Misc
    {
        public static Vector2 RotateVector(Vector2 vector, float angle)
        {
            // http://en.wikipedia.org/wiki/Rotation_(mathematics)
            // поворот вектора
            float newX = vector.X * (float)Math.Cos(angle / 180 * Math.PI) - vector.Y * (float)Math.Sin(angle / 180 * Math.PI);
            float newY = vector.X * (float)Math.Sin(angle / 180 * Math.PI) + vector.Y * (float)Math.Cos(angle / 180 * Math.PI);
            return new Vector2(newX, newY);
        }

        /// <summary>
        /// Определение пересечения двух отрезков.
        /// Взято отсюда: http://users.livejournal.com/_winnie/152327.html
        /// </summary>
        /// <param name="start1"></param>
        /// <param name="end1"></param>
        /// <param name="start2"></param>
        /// <param name="end2"></param>
        /// <returns></returns>
        public static bool IntersectSegment(Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2)
        {
            Vector2 dir1 = end1 - start1;
            Vector2 dir2 = end2 - start2;

            //считаем уравнения прямых проходящих через отрезки
            float a1 = -dir1.Y;
            float b1 = +dir1.X;
            float d1 = -(a1 * start1.X + b1 * start1.Y);

            float a2 = -dir2.Y;
            float b2 = +dir2.X;
            float d2 = -(a2 * start2.X + b2 * start2.Y);

            //подставляем концы отрезков, для выяснения в каких полуплоскотях они
            float seg1_line2_start = a2 * start1.X + b2 * start1.Y + d2;
            float seg1_line2_end = a2 * end1.X + b2 * end1.Y + d2;

            float seg2_line1_start = a1 * start2.X + b1 * start2.Y + d1;
            float seg2_line1_end = a1 * end2.X + b1 * end2.Y + d1;

            //если концы одного отрезка имеют один знак, значит он в одной полуплоскости и пересечения нет.
            if (seg1_line2_start * seg1_line2_end >= 0 || seg2_line1_start * seg2_line1_end >= 0)
                return false;

            return true;
        }

        /// <summary>
        /// Finds projection of the p to line. {p1;p2} defines line
        /// http://www.rsdn.ru/forum/alg/1655970.flat.aspx
        /// </summary>
        /// <param name="p">Point to project</param>
        /// <param name="p1">First point of line segment</param>
        /// <param name="p2">Second point of line segment</param>
        /// <returns></returns>
        public static float GetProjection(Vector2 p, Vector2 p1, Vector2 p2)
        {
            float fDenominator = (p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y);
            if (fDenominator == 0) // p1 and p2 are the same
                //return p1;
                return 0;

            float t = (p.X * (p2.X - p1.X) - (p2.X - p1.X) * p1.X + p.Y * (p2.Y - p1.Y) - (p2.Y - p1.Y) * p1.Y) / fDenominator;

            return t;
            //return new Vector2(p1.X + (p2.X - p1.X) * t, p1.Y + (p2.Y - p1.Y) * t);
        }

        public static void Swap<T>(ref T left, ref T right) where T : class
        {
            T temp;
            temp = left;
            left = right;
            right = temp;
        }

        public static bool AreEqual<T>(LinkedList<T> a, LinkedList<T> b) where T : IEquatable<T>
        {
            if (a.Count != b.Count)
            {
                return false;
            }

            LinkedListNode<T> currA = a.First;
            LinkedListNode<T> currB = b.First;

            while (currA != null)
            {
                if (!currA.Value.Equals(currB.Value))
                    return false;

                currA = currA.Next;
                currB = currB.Next;
            }

            return true;
        }

        public static bool TriangleIntersectsWithTriangle(Triangle<Vector2> first, Triangle<Vector2> second)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (Misc.IntersectSegment(first[i], first[(i + 1) % 3], second[j], second[(j + 1) % 3]))
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
                if (!Misc.TriangleIntersectsWithPoint(first, second[i]))
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
                    if (!Misc.TriangleIntersectsWithPoint(second, first[i]))
                        return false;
                }
            }

            return true;
        }

        public static bool TriangleIntersectsWithPoint(Triangle<Vector2> triangle, Vector2 point)
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

        public static bool CircleIntersectsWithTriangle(CircleBoundsAspect circle, Triangle<Vector2> triangle)
        {
            // Для произвольных выпуклых объектов используем Gilbert–Johnson–Keerthi distance algorithm
            // тут мы используем теорему о разделяющей оси http://www.gamedev.ru/code/terms/SAT
            // небольшие пояснения тут http://www.ryandudley.com/docs/spheretri.pdf

            // сначала проверяем 3 прямые, образованные сторонами треугольника
            for (int i = 0; i < 3; i++)
            {
                Vector2 start = triangle[i];
                Vector2 end = triangle[(i + 1) % 3];

                if (!TestSegmentIntersection(circle, triangle, ref start, ref end))
                    return false;
            }

            // теперь проверяем 3 прямые, проходящие через центр круга и каждую из вершин треугольника
            for (int i = 0; i < 3; i++)
            {
                Vector2 start = triangle[i];
                Vector2 end = circle.Position;

                if (!TestSegmentIntersection(circle, triangle, ref start, ref end))
                    return false;
            }

            return true;
        }

        public static bool TestSegmentIntersection(CircleBoundsAspect circle, Triangle<Vector2> triangle, ref Vector2 start, ref Vector2 end)
        {
            // проецируем каждую вершину треугольника на прямую
            float triangleProjection;
            float triangleMinProjection = float.MaxValue;
            float triangleMaxProjection = float.MinValue;

            for (int j = 0; j < 3; j++)
            {
                triangleProjection = Misc.GetProjection(triangle[j], start, end);
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
    }


}