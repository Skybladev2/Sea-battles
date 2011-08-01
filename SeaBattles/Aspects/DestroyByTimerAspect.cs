using System;
using System.Collections.Generic;
using System.Text;
using SeaBattles.Messages;

namespace SeaBattles
{
    /// <summary>
    /// Аспект, уничтожающий своих соседей (если они есть) и владельца по истечении определённого времени.
    /// Возможно, стоит заменить на более универсальный аспект, который принимает дельту и на её основе генерирует события.
    /// Хотя это можно тупо сделать в главном цикле Update.
    /// </summary>
    internal class DestroyByTimerAspect : Aspect
    {
        private TimeSpan timeToLive;
        private double lifeInSeconds = 0;

        public DestroyByTimerAspect(object owner, TimeSpan timeToLive) : base(owner)
        {
            this.timeToLive = timeToLive;
            RegisterAllStuff();
        }

        public void Update (double dt)
        {
            lifeInSeconds += dt;
            if (lifeInSeconds >= timeToLive.TotalSeconds)
                MessageDispatcher.Post(new DestroyChildrenOf(owner));
        }
    }
}
