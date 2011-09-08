using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;

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
    }
}
