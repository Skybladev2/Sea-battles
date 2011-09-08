using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using OpenTK;
using SeaBattles.Messages;

namespace SeaBattles
{
    /// <summary>
    /// Снаряд. То, чем стреляют пушки корабля.
    /// </summary>
    internal class Shell : Aspect
    {
        private GraphicsAspect graphics = null;
        private PhysicsAspect physics = null;
        private DestroyByTimerAspect timer = null;
        private float radius = 0.76f; // калибр 152 мм

        public Shell(Vector3 position, Vector3 cannonFacing, Vector3 cannonVelocity, float startSpeed)
        {
            int segments = 8;
            List<Vector3> vertices = new List<Vector3>(segments + 1);

            //float unitradius = (float)Math.Sqrt(8);
            float angleStep = (float)(2 * Math.PI / segments);
            for (int i = 0; i < segments; i++)
            {
                vertices.Add(new Vector3((float)Math.Cos(angleStep * i) * radius, (float)Math.Sin(angleStep * i) * radius, 0));
            }
            vertices.Add(new Vector3(radius, 0, 0));

            graphics = new GraphicsAspect(this, vertices, position, 1, Color.White, Color.Red);

            // вычисляем вектор полёта снаряда - сумма импульса выстрела и собственной скорости оружия
            cannonFacing.NormalizeFast();
            Vector3 shootVelocity = cannonFacing * startSpeed + cannonVelocity;
            Vector2 shootDirection = shootVelocity.Xy;
            shootDirection.NormalizeFast();
            //physics = new PhysicsAspect(this, position, shootVelocity.Xy, startSpeed);

            physics = new PhysicsAspect(this, position, shootDirection, shootVelocity.LengthFast);
            //physics = new PhysicsAspect(this, position, Vector2.Zero, 0);
            timer = new DestroyByTimerAspect(this, new TimeSpan(0, 0, 0, 2, 500));

            MessageDispatcher.RegisterHandler(typeof(SetPosition), graphics);
            MessageDispatcher.RegisterHandler(typeof(DestroyChildrenOf), this);
        }

        protected override void Cleanup()
        {
            base.Cleanup();
            MessageDispatcher.UnRegisterHandler(typeof(DestroyChildrenOf), this);
            MessageDispatcher.UnRegisterHandler(typeof(SetPosition), graphics);
        }
    }
}



