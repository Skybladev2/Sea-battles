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
    internal class ShootAspect : Aspect
    {
        //private GraphicsAspect graphics = null;
        private float startVelocity = 1; // начальная скорость снаряда в м/с

        public ShootAspect(object owner) : base(owner)
        {
            handlers.Add(typeof(Shoot), new HandlerMethodDelegate(Shoot));
            RegisterSelf();
        }

        private void Shoot(object message)
        {
            Shoot shoot = (Shoot)message;
            Shell shell = new Shell(shoot.From, shoot.Direction, shoot.ShooterVelocity, 1);
            //Shell shell = new Shell(new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), 0.0001f);
        }
    }
}
