using System;
using System.Collections.Generic;
using System.Linq;
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
        private Dictionary<Type, LinkedList<IMessageHandler>> handlersMap = new Dictionary<Type, LinkedList<IMessageHandler>>();

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
            List<Vector3> shipVerts = new List<Vector3>();
            shipVerts.Add(new Vector3(-0.1f + position.X, -0.3f + position.Y, -1));
            shipVerts.Add(new Vector3(-0.1f + position.X, 0.1f + position.Y, -1));
            shipVerts.Add(new Vector3(0 + position.X, 0.3f + position.Y, -1));
            shipVerts.Add(new Vector3(0.1f + position.X, 0.1f + position.Y, -1));
            shipVerts.Add(new Vector3(0.1f + position.X, -0.3f + position.Y, -1));
            shipVerts.Add(new Vector3(-0.1f + position.X, -0.3f + position.Y, -1));

            mechanics = new VehicleWithGearboxAspect(this);
            physics = new PhysicsAspect(this);
            graphics = new GraphicsAspect(this, shipVerts, 3);

            // подписываем транспортное средство на приём пользовательского ввода
            // оно будет обновлять физику
            AddHandlerToMap(typeof(ButtonDown), mechanics);
            // подписываем графику на обновление координат
            AddHandlerToMap(typeof(SetPosition), graphics);

            rearCannon = new Weapon(this, Side.Rear);
            leftCannon = new Weapon(this, Side.Left);
            rightCannon = new Weapon(this, Side.Right);

            // пушки принимают пользовательский ввод
            AddHandlerToMap(typeof(ButtonDown), leftCannon);
            AddHandlerToMap(typeof(ButtonDown), rightCannon);
            AddHandlerToMap(typeof(ButtonDown), rearCannon);

            // физика принимает команду на изменение скорости корабля
            AddHandlerToMap(typeof(SetSpeed), physics);
            AddHandlerToMap(typeof(GetOwnerPosition), physics);

            // подписываем пушки на события выстрелов
            AddHandlerToMap(typeof(Shoot), leftCannon);
            AddHandlerToMap(typeof(Shoot), rightCannon);
            AddHandlerToMap(typeof(Shoot), rearCannon);

            AddHandlerToMap(typeof(InformPosition), leftCannon);
            AddHandlerToMap(typeof(InformPosition), rightCannon);
            AddHandlerToMap(typeof(InformPosition), rearCannon);

            // нарушение принципа Entity - Property отсюда http://flohofwoe.blogspot.com/2007/11/nebula3s-application-layer-provides.html
            //MessageDispatcher.RegisterHandler(typeof(GetOwnerPosition), physics);
        }
    }
}
