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
        private BoundSetAspect bounds;

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

        internal Weapon RearCannon
        {
            get { return rearCannon; }
        }

        public static Ship Create(PointF position, float length, float width)
        {
            Ship aspect = new Ship(position, length, width);

            MessageDispatcher.RegisterHandler(typeof(SetPosition), aspect);
            MessageDispatcher.RegisterHandler(typeof(SetSpeed), aspect);
            // нужно для определения координат и скорости корабля в момент выстрела
            // в данном случае owner-ом является ship
            MessageDispatcher.RegisterHandler(typeof(GetOwnerPosition), aspect);
            MessageDispatcher.RegisterHandler(typeof(InformPosition), aspect);
            MessageDispatcher.RegisterHandler(typeof(Shoot), aspect);
            MessageDispatcher.RegisterHandler(typeof(BoundSetCollision), aspect);
            MessageDispatcher.RegisterHandler(typeof(BoundSetNotCollision), aspect);

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
            bounds = BoundSetAspect.Create(this, shipVerts);
            //bounds.GetOuterContour();
            bounds.SetAttribute(Strings.CollisionDetectionSpeedType, Strings.CollisionDetectionSpeedTypeSlowOrStatic);

            rearCannon = Weapon.Create(this, Side.Rear);
            leftCannon = Weapon.Create(this, Side.Left);
            rightCannon = Weapon.Create(this, Side.Right);
        }
    }
}
