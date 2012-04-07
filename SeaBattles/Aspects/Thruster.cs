using System;
using System.Collections.Generic;
using System.Text;
using SeaBattles.Messages;
using OpenTK;

namespace SeaBattles
{
    /// <summary>
    /// Тяговый двигатель. Прикладывает силу к массе. Повторяет все перемещения массы, так как находится (по умолчанию) в той же точке.
    /// </summary>
    internal class Thruster : Aspect
    {
        protected Vector2 facing;
        /// <summary>
        /// По умолчанию тяговый двигатель смотрит в ту же сторону, что и родитель.
        /// </summary>
        protected float angleFromParentFacing = 0;
        protected float maxForce = 1;
        protected bool isOn;

        public bool IsOn
        {
            get
            {
                return isOn;
            }
            set
            {
                isOn = value;
            }
        }

        private Thruster(object owner) : base(owner)
        {
            // при движении физического аспекта _поворачиваем_ и тяговый двигатель
            //messageHandler.Handlers.Add(typeof(SetPosition), HandleUpdatePosition);
        }

        public static Thruster Create(object owner, Vector2 facing)
        {
            Thruster aspect = new Thruster(owner);
            aspect.facing = facing;
            aspect.RegisterAllStuff();
            return aspect;
        }

        //private bool HandleUpdatePosition(object message)
        //{
        //    SetPosition setPosition = (SetPosition)message;
        //    if (this.owner == setPosition.Target)
        //    {
        //        this.facing = setPosition.Facing;
        //    }

        //    return true;
        //}

        public void ApplyForce()
        {
            if (isOn)
                MessageDispatcher.Post(new ApplyForce(this.owner, facing * maxForce));
        }

        internal void SetRotation(Vector2 velocity)
        {
            // пока забиваем на angleFromParentFacing
            this.facing = velocity;
        }
    }
}
