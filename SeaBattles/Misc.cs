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
    }
}
