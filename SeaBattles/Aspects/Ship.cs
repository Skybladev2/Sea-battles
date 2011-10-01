using System;
using System.Collections.Generic;
using System.Text;
using SeaBattles.Messages;
using OpenTK;
using System.Drawing;

namespace SeaBattles
{
    /// <summary>
    /// Корабль. Может управляться игроком или ИИ.
    /// </summary>
    internal class Ship : Aspect
    {
        private PhysicsAspect physics;
        private VehicleWithGearboxAspect mechanics;
        private GraphicsAspect graphics;
        private Weapon leftCannon;
        private Weapon rightCannon;
        private Weapon rearCannon;

        internal VehicleWithGearboxAspect Mechanics
        {
            get { return mechanics; }
            //set { mechanics = value; }
        }

        internal GraphicsAspect Graphics
        {
            get { return graphics; }
            //set { graphics = value; }
        }

        internal PhysicsAspect Physics
        {
            get { return physics; }
            //set { physics = value; }
        }

        public static Ship Create(PointF position, float length, float width)
        {
            Ship aspect = new Ship(position, length, width);
            aspect.RegisterAllStuff();
            return aspect;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="shipVerts"></param>
        private Ship(PointF position, float length, float width)
        {
            float depth = 0;
            List<Vector3> shipVerts = new List<Vector3>();
            shipVerts.Add(new Vector3(-1f * width / 2 + position.X, -1f * length / 2 + position.Y, depth));
            shipVerts.Add(new Vector3(-1f * width / 2 + position.X, 0.25f * length / 2 + position.Y, depth));
            shipVerts.Add(new Vector3(0 * width / 2 + position.X, 1f * length / 2 + position.Y, depth));
            shipVerts.Add(new Vector3(1f * width / 2 + position.X, 0.25f * length / 2 + position.Y, depth));
            shipVerts.Add(new Vector3(1f * width / 2 + position.X, -1f * length / 2 + position.Y, depth));
            shipVerts.Add(new Vector3(-1f * width / 2 + position.X, -1f * length / 2 + position.Y, depth));

            mechanics = VehicleWithGearboxAspect.Create(this);
            physics = PhysicsAspect.Create(this);
            graphics = GraphicsAspect.Create(this, shipVerts, 3, Color.White, Color.Red);

            rearCannon = Weapon.Create(this, Side.Rear);
            leftCannon = Weapon.Create(this, Side.Left);
            rightCannon = Weapon.Create(this, Side.Right);
        }
    }
}
