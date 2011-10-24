﻿using System;
using System.Collections.Generic;
using System.Text;
using SeaBattles.Messages;

namespace SeaBattles
{
    internal class CollisionManager
    {
        //private static Dictionary<object, Dictionary<BoundSetAspect, byte>> collisions = new Dictionary<object, Dictionary<BoundSetAspect, byte>>();
        private static Dictionary<Pair<BoundSetAspect>, byte> collisions = new Dictionary<Pair<BoundSetAspect>, byte>();

        private static void SendCollision(BoundSetAspect boundSet1, BoundSetAspect boundSet2)
        {
            if (boundSet1.GetOwner().GetType() == typeof(Shell) && boundSet2.GetOwner().GetType() != typeof(Shell))
            {
                Shell shell = (Shell)boundSet1.GetOwner();
                if (shell.ShellOwner != boundSet2.GetOwner())
                {
                    Ship target = (Ship)boundSet2.GetOwner();
                    target.RearCannon.PerformShoot();
                    MessageDispatcher.Post(new BoundSetCollision(boundSet1, boundSet2));
                }
            }
            else
                if (boundSet2.GetOwner().GetType() == typeof(Shell) && boundSet1.GetOwner().GetType() != typeof(Shell))
                {
                    Shell shell = (Shell)boundSet2.GetOwner();
                    if (shell.ShellOwner != boundSet1.GetOwner())
                    {
                        Ship target = (Ship)boundSet1.GetOwner();
                        target.RearCannon.PerformShoot();
                        MessageDispatcher.Post(new BoundSetCollision(boundSet1, boundSet2));
                    }
                }
                else
                    MessageDispatcher.Post(new BoundSetCollision(boundSet1, boundSet2));
        }

        private static void SendNotCollision(BoundSetAspect boundSet1, BoundSetAspect boundSet2)
        {
            MessageDispatcher.Post(new BoundSetNotCollision(boundSet1, boundSet2));
        }

        internal static void CheckIntersection(BoundSetAspect boundSet1, BoundSetAspect boundSet2)
        {
            Pair<BoundSetAspect> pair = new Pair<BoundSetAspect>(boundSet1, boundSet2);
            // если объекты пересекаются, то ищем их в словаре.
            // если нашли, то НЕ генерируем событие Collision
            // если не нашли, то генерируем событие Collision и заносим их в словарь
            if (boundSet1.IntersectsWith(boundSet2))
            {
                if (!collisions.ContainsKey(pair))
                {
                    collisions.Add(pair, 0);
                    SendCollision(boundSet1, boundSet2);
                }
            }
            // объекты не пересекаются.
            // нужно удалить их из словарей, если они там есть,
            // и сгенерировать событие NotCollision, если они там были
            else
            {
                if (collisions.ContainsKey(pair))
                {
                    collisions.Remove(pair);
                    SendNotCollision(boundSet1, boundSet2);
                }
            }
        }
    }
}
