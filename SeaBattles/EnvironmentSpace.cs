using System;
using System.Collections.Generic;
using System.Text;
using SeaBattles.Messages;
using OpenTK;

namespace SeaBattles
{
    internal static class EnvironmentSpace
    {
        private static float waterResistanceCoefficient = 1.0f;

        internal static void ApplyForce(ICollection<Aspect> physicsAspects)
        {
            foreach (PhysicsAspect physicsAspect in physicsAspects)
            {
                // сопротивление воды направлено против движения корабля и прямо пропорционально скорости
                Vector2 velocityDirection = physicsAspect.Velocity;
                velocityDirection.NormalizeFast();
                MessageDispatcher.Post(new ApplyForce(physicsAspect.GetOwner(),
                    -velocityDirection * physicsAspect.Velocity.LengthFast * waterResistanceCoefficient));
            }
        }
    }
}
