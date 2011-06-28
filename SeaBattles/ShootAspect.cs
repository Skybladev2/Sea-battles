using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using System.Drawing;
using SeaBattles.Messages;

namespace SeaBattles
{
    /// <summary>
    /// Нечто, умеющее стрелять.
    /// </summary>
    internal class ShootAspect : IMessageHandler
    {
        private Dictionary<Type, HandlerMethodDelegate> handlers = new Dictionary<Type, HandlerMethodDelegate>();
        //private GraphicsAspect graphics = null;
        private float startVelocity = 1; // начальная скорость снаряда в м/с

        public ShootAspect()
        {
            handlers.Add(typeof(Shoot), new HandlerMethodDelegate(Shoot));
        }

        private void Shoot(object message)
        {
            Shoot shoot = (Shoot)message;
            Shell shell = new Shell(shoot.from.Xy, shoot.direction.Xy, startVelocity);
        }

        #region IMessageHandler Members

        public void ProcessMessage(object message)
        {
            Type type = message.GetType();
            handlers[type](message);
        }

        #endregion
    }
}
