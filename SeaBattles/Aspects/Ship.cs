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

        //private Dictionary<Type, IMessageHandler> handlersMap = new Dictionary<Type, IMessageHandler>();
        //private Dictionary<Type, LinkedList<IMessageHandler>> handlersMap = new Dictionary<Type, LinkedList<IMessageHandler>>();

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

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="shipVerts"></param>
        public Ship(PointF position)
        {
            float scale = 1f;
            float width = 10f;
            float length = 40f;
            List<Vector3> shipVerts = new List<Vector3>();
            shipVerts.Add(new Vector3(-1f * width / 2 + position.X, -1f * length / 2 + position.Y, 0));
            shipVerts.Add(new Vector3(-1f * width / 2 + position.X, 0.25f * length / 2 + position.Y, 0));
            shipVerts.Add(new Vector3(0 * width / 2 + position.X, 1f * length / 2 + position.Y, 0));
            shipVerts.Add(new Vector3(1f * width / 2 + position.X, 0.25f * length / 2 + position.Y, 0));
            shipVerts.Add(new Vector3(1f * width / 2 + position.X, -1f * length / 2 + position.Y, 0));
            shipVerts.Add(new Vector3(-1f * width / 2 + position.X, -1f * length / 2 + position.Y, 0));

            mechanics = new VehicleWithGearboxAspect(this);
            physics = new PhysicsAspect(this);
            graphics = new GraphicsAspect(this, shipVerts, 3);

            rearCannon = new Weapon(this, Side.Rear);
            leftCannon = new Weapon(this, Side.Left);
            rightCannon = new Weapon(this, Side.Right);

            RegisterAllStuff();
        }
    }
}
