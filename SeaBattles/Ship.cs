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
    internal class Ship : IMessageHandler
    {
        private PhysicsAspect physics;
        private GraphicsAspect graphics;
        private Weapon leftCannon;
        private Weapon rightCannon;
        private Weapon rearCannon;

        //private Dictionary<Type, IMessageHandler> handlersMap = new Dictionary<Type, IMessageHandler>();
        private Dictionary<Type, LinkedList<IMessageHandler>> handlersMap = new Dictionary<Type, LinkedList<IMessageHandler>>();

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

            physics = new PhysicsAspect(this);
            graphics = new GraphicsAspect(shipVerts);
            AddHandlerToMap(typeof(ButtonDown), physics);
            AddHandlerToMap(typeof(SetPosition), graphics);

            rearCannon = new Weapon(this, Side.Rear);
            leftCannon = new Weapon(this, Side.Left);
            rightCannon = new Weapon(this, Side.Right);

            // пушки принимают пользовательский ввод
            AddHandlerToMap(typeof(ButtonDown), leftCannon);
            AddHandlerToMap(typeof(ButtonDown), rightCannon);
            AddHandlerToMap(typeof(ButtonDown), rearCannon);

            MessageDispatcher.RegisterHandler(typeof(GetOwnerPosition), physics);
        }

        private void AddHandlerToMap(Type type, IMessageHandler handler)
        {
            if (!handlersMap.ContainsKey(type))
            {
                LinkedList<IMessageHandler> list = new LinkedList<IMessageHandler>();
                list.AddFirst(handler);
                handlersMap.Add(type, list);
            }
            else
                if (!handlersMap[type].Contains(handler))
                    handlersMap[type].AddLast(handler);
        }

        #region IMessageHandler Members

        public void ProcessMessage(object message)
        {
            Type type = message.GetType();
            if (handlersMap.ContainsKey(type))
                foreach (IMessageHandler handler in handlersMap[type])
                {
                    handler.ProcessMessage(message);
                }
        }

        #endregion
    }
}
