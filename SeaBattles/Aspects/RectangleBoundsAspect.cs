﻿using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using SeaBattles.Messages;

namespace SeaBattles
{
    /// <summary>
    /// Object-Oriented Bounding Box
    /// Прямоугольник может быть повёрнут относительно координатных осей.
    /// </summary>
    internal class RectangleBoundsAspect : BoundsAspect
    {
        // <summary>
        // Координаты считаются от центра прямоугольника (объекта).
        // </summary>
        
        private float height;
        private float width;

        private Vector2 position;
        private float angle;

        /// <summary>
        /// Расстояние от центра до угла прямоугольника.
        /// </summary>
        private float diagonalLength;

        public RectangleBoundsAspect(object owner, float width, float height, Vector2 position, float angle) :base(owner)
        {
            this.height = height;
            this.width = width;
            this.position = position;
            this.angle = angle;

            this.diagonalLength = (float)Math.Sqrt(Math.Pow(height/ 2, 2) + Math.Pow(width / 2, 2));

            handlers.Add(typeof(SetPosition), new HandlerMethodDelegate(HandleUpdatePosition));

            RegisterAllStuff();
        }

        public override bool IntersectsWith(Vector2 point)
        {
            // первая проверка - грубая, расчёт расстояния от центра до углов (аппроксимация окружностью)
            if ((point - position).Length <= diagonalLength)
            {
                // поворачиваем точку вокруг центра и смотрим, внутри ли она прямоугольника
                Vector2 difference = point - position;
                Vector2 rotated = Misc.RotateVector(difference, angle);

                if (rotated.X >= -width / 2 && rotated.X <= width / 2
                    &&
                    rotated.Y >= -height / 2 && rotated.Y <= height / 2)
                    return true;
                else
                    return false;

            }

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
                this.angle = setPosition.Angle;
                this.position = setPosition.Position.Xy;
            }
        }
    }
}