﻿using System;
using System.Collections.Generic;
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

        public static ShootAspect Create(object owner)
        {
            ShootAspect aspect = new ShootAspect(owner);
            aspect.RegisterAllStuff();
            return aspect;
        }

        private ShootAspect(object owner)
            : base(owner)
        {
            messageHandler.Handlers.Add(typeof(Shoot), Shoot);
        }

        private bool Shoot(object message)
        {
            Shoot shoot = (Shoot)message;
            if (shoot.Owner != null && shoot.Owner.Equals(this.owner))
            {
                Weapon ownWeapon = (Weapon)this.owner;
                Ship ownShip = (Ship)ownWeapon.GetOwner();
                // 400 м/с
                Shell shell = new Shell(shoot.From, shoot.Direction, shoot.ShooterVelocity, 400, ownShip);
            }
            //Shell shell = new Shell(new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), 0.0001f);

            return true;
        }
    }
}
