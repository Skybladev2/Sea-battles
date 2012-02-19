using System;
using System.Collections.Generic;
using System.Text;
using SeaBattles.Messages;

namespace SeaBattles
{
    internal class DamageAspect : Aspect
    {
        protected int maxDamage = 100;
        protected int minDamage = 0;

        protected int currentDamage = 0;

        public int CurrentDamage
        {
            get { return currentDamage; }
        }

        public DamageAspect(object owner) : base(owner)
        {}

        public DamageAspect(object owner, int maxDamage) : this(owner)
        {
            this.maxDamage = maxDamage;
        }

        public static DamageAspect Create(object owner, int maxDamage)
        {
            DamageAspect aspect = new DamageAspect(owner, maxDamage);
            aspect.RegisterAllStuff();
            return aspect;
        }

        public bool ApplyDamage(int amount)
        {
            currentDamage += amount;
            if (currentDamage >= maxDamage)
            {
                //MessageDispatcher.Post(new Kill(owner));
                return true;
            }
            else
                return false;
        }
    }
}
