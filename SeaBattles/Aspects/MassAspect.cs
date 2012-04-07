using System;
using System.Collections.Generic;
using System.Text;
using SeaBattles.Messages;
using OpenTK.Input;
using System.Drawing;
using OpenTK;

namespace SeaBattles
{
    internal class MassAspect : Aspect
    {
        private float mass = 1;

        public static MassAspect Create(object owner)
        {
            MassAspect aspect = new MassAspect(owner);
            aspect.RegisterAllStuff();
            return aspect;
        }

        private MassAspect(object owner)
            : base(owner)
        {
            messageHandler.Handlers.Add(typeof(ApplyForce), HandleApplyForce);
        }

        private MassAspect(object owner, float mass)
            : this(owner)
        {
            this.mass = mass;
        }

        private bool HandleApplyForce(object message)
        {
            ApplyForce applyForce = (ApplyForce)message;
            if (applyForce.Owner == this.owner)
            {
                // посылаем ускорение, которое будет применяться вдоль вектора направления скорости
                // за поворот отвечает угловое ускорение
                MessageDispatcher.Post(new AddForwardAcceleration(this.owner, applyForce.Force / mass));
            }

            //MessageDispatcher.Post(new TraceText("Velocity: " + this.Velocity.ToString() + ", Angle: " + angle));
            return true;
        }
    }
}
