﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OpenTK;

namespace SeaBattles
{
    /// <summary>
    /// Снаряд. То, чем стреляют пушки корабля.
    /// </summary>
    internal class Shell
    {
        private GraphicsAspect graphics = null;
        private PhysicsAspect physics = null;
        private float radius = 1;

        public Shell(Vector3 position, Vector3 cannonFacing, Vector3 cannonVelocity, float startSpeed)
        {
            List<Vector3> vertices = new List<Vector3> (8);

            //float unitradius = (float)Math.Sqrt(8);
            float unitradius = 100;
            // рисуем с левого верхнего угла по часовой стрелке
            // грубое приближение круга 8 вершинами
            // радиус sqrt(2^2 + 2^2) = sqrt(8)

            vertices.Add(new Vector3(position.X + -2 / unitradius, position.Y + 1 / unitradius, -1));
            vertices.Add(new Vector3(position.X + -1 / unitradius, position.Y + 2 / unitradius, -1));

            vertices.Add(new Vector3(position.X + 1 / unitradius, position.Y + 2 / unitradius, -1));
            vertices.Add(new Vector3(position.X + 2 / unitradius, position.Y + 1 / unitradius, -1));

            vertices.Add(new Vector3(position.X + 2 / unitradius, position.Y + -1 / unitradius, -1));
            vertices.Add(new Vector3(position.X + 1 / unitradius, position.Y + -2 / unitradius, -1));

            vertices.Add(new Vector3(position.X + -1 / unitradius, position.Y + -2 / unitradius, -1));
            vertices.Add(new Vector3(position.X + -2 / unitradius, position.Y + -1 / unitradius, -1));

            //vertices.Add(new Vector3(position.X + -1 / unitradius, position.Y + -2 / unitradius, -1));
            //vertices.Add(new Vector3(-position.X + 2 / unitradius, position.Y + -1 / unitradius, -1));

            vertices.Add(new Vector3(position.X + -2 / unitradius, position.Y + 1 / unitradius, -1));

            graphics = new GraphicsAspect(this, vertices, 1);

            // вычисляем вектор полёта снаряда - сумма импульса выстрела и собственной скорости оружия
            cannonFacing.NormalizeFast();
            Vector3 shootVelocity = cannonFacing * startSpeed + cannonVelocity;

            physics = new PhysicsAspect(this, position.Xy, shootVelocity.Xy, startSpeed);
        }
    }
}
