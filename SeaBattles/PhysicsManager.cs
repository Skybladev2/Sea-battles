using System;
using System.Collections.Generic;
using System.Text;
using SeaBattles.Messages;

namespace SeaBattles
{
    internal static class PhysicsManager
    {
        internal static void Update(double dt)
        {
            // движители генерируют силу
            foreach (Thruster thruster in AspectLists.GetAspects(typeof(Thruster)))
            {
                thruster.ApplyForce();
            }
            ICollection<Aspect> physicsAspects = AspectLists.GetAspects(typeof(PhysicsAspect));
            // применяем силы сопротивления среды
            EnvironmentSpace.ApplyForce(physicsAspects);

            // !тут нужна синхронизация!
            
            // в соответствии с силами двигаем физику
            foreach (PhysicsAspect p in physicsAspects)
            {
                p.Update(dt);
            }
        }
    }
}
