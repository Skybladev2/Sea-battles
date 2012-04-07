using System;
using System.Collections.Generic;
using System.Text;
using SeaBattles.Messages;

namespace SeaBattles
{
    internal static class PhysicsManager
    {
        /// <summary>
        /// Разгоняющиеся аспекты.
        /// Ключ - аспект.
        /// Значение - {целевая скорость, ускорение до этой скорости}
        /// </summary>
        private static Dictionary<PhysicsAspect, List<KeyValuePair<float, float>>> acceleratingAspects = new Dictionary<PhysicsAspect, List<KeyValuePair<float, float>>>();
        /// <summary>
        /// Замедляющиеся аспекты.
        /// Ключ - аспект.
        /// Значение - {целевая скорость, ускорение до этой скорости}
        /// </summary>
        private static Dictionary<PhysicsAspect, List<KeyValuePair<float, float>>> deceleratingAspects = new Dictionary<PhysicsAspect, List<KeyValuePair<float, float>>>();

        private static Dictionary<PhysicsAspect, float> targetAccelerations = new Dictionary<PhysicsAspect, float>();

        internal static void AddTargetSpeedAspect(PhysicsAspect aspect, float targetSpeed, float acceleration)
        {
            if (acceleration > 0)
            {
                if (!acceleratingAspects.ContainsKey(aspect))
                {
                    List<KeyValuePair<float, float>> queue = new List<KeyValuePair<float, float>>();
                    queue.Add(new KeyValuePair<float, float>(targetSpeed, acceleration));
                    acceleratingAspects.Add(aspect, queue);
                }
                else
                {
                    if (!acceleratingAspects[aspect].Contains(new KeyValuePair<float, float>(targetSpeed, acceleration)))
                        acceleratingAspects[aspect].Add(new KeyValuePair<float, float>(targetSpeed, acceleration));
                }
            }
            else
            {
                if (!deceleratingAspects.ContainsKey(aspect))
                {
                    List<KeyValuePair<float, float>> queue = new List<KeyValuePair<float, float>>();
                    queue.Add(new KeyValuePair<float, float>(targetSpeed, acceleration));
                    deceleratingAspects.Add(aspect, queue);
                }
                else
                {
                    if (!deceleratingAspects[aspect].Contains(new KeyValuePair<float, float>(targetSpeed, acceleration)))
                        deceleratingAspects[aspect].Insert(0, new KeyValuePair<float, float>(targetSpeed, acceleration));
                }
            }
        }

        internal static void Update(double dt)
        {
            // движители генерируют силу
            foreach (Thruster thruster in AspectLists.GetAspects(typeof(Thruster)))
            {
                thruster.ApplyForce();
            }

            // !тут нужна синхронизация!
            
            // в соответствии с силами двигаем физику
            foreach (PhysicsAspect p in AspectLists.GetAspects(typeof(PhysicsAspect)))
            {
                p.Update(dt);
            }
            
            //ProcessAcceleratingAspects();

            //ProcessDeceleratingAspects();
        }

        //private static void ProcessDeceleratingAspects()
        //{
        //    Dictionary<PhysicsAspect, List<KeyValuePair<float, float>>> newDeceleratingAspects = new Dictionary<PhysicsAspect, List<KeyValuePair<float, float>>>();

        //    foreach (KeyValuePair<PhysicsAspect, List<KeyValuePair<float, float>>> kvp in deceleratingAspects)
        //    {
        //        float targetSpeed = 0;
        //        // если в очереди есть целевые скорости
        //        if (kvp.Value.Count != 0)
        //        {
        //            // текущая скорость объекта меньше ближайшей целевой
        //            if (kvp.Key.Speed <= kvp.Value[kvp.Value.Count - 1].Key)
        //            {
        //                // запланировано переключение минимум на 2 передачи
        //                if (kvp.Value.Count > 1)
        //                {
        //                    kvp.Value.RemoveAt(kvp.Value.Count - 1);
        //                    // устанавливаем новое ускорение
        //                    MessageDispatcher.Post(new SetAcceleration(kvp.Key.GetOwner(), null, kvp.Value[kvp.Value.Count - 1].Value));
        //                    newDeceleratingAspects.Add(kvp.Key, kvp.Value);
        //                    continue;
        //                }
        //                else
        //                {
        //                    targetSpeed = kvp.Value[kvp.Value.Count - 1].Key;
        //                    kvp.Value.RemoveAt(kvp.Value.Count - 1);
        //                }

        //                targetAccelerations.Remove(kvp.Key);
        //                MessageDispatcher.Post(new SetAcceleration(kvp.Key.GetOwner(), null, 0));
        //                if (kvp.Value.Count != 0)
        //                    MessageDispatcher.Post(new SetSpeed(kvp.Key.GetOwner(), kvp.Value[kvp.Value.Count - 1].Key));
        //                else
        //                    MessageDispatcher.Post(new SetSpeed(kvp.Key.GetOwner(), targetSpeed));
        //            }
        //            else
        //            {
        //                if (!targetAccelerations.ContainsKey(kvp.Key))
        //                {
        //                    targetAccelerations.Add(kvp.Key, kvp.Value[kvp.Value.Count - 1].Value);
        //                    MessageDispatcher.Post(new SetAcceleration(kvp.Key.GetOwner(), kvp.Value[kvp.Value.Count - 1].Key, kvp.Value[kvp.Value.Count - 1].Value));
        //                }
        //                else
        //                {
        //                    // уcтановлено другое ускорение - заменяем его
        //                    if (targetAccelerations[kvp.Key] != kvp.Value[kvp.Value.Count - 1].Value)
        //                    {
        //                        targetAccelerations[kvp.Key] = kvp.Value[kvp.Value.Count - 1].Value;
        //                        MessageDispatcher.Post(new SetAcceleration(kvp.Key.GetOwner(), kvp.Value[kvp.Value.Count - 1].Key, kvp.Value[kvp.Value.Count - 1].Value));
        //                    }
        //                }
        //                newDeceleratingAspects.Add(kvp.Key, kvp.Value);
        //            }
        //        }
        //    }

        //    deceleratingAspects = newDeceleratingAspects;
        //}

        //private static void ProcessAcceleratingAspects()
        //{
        //    Dictionary<PhysicsAspect, List<KeyValuePair<float, float>>> newAcceleratingAspects = new Dictionary<PhysicsAspect, List<KeyValuePair<float, float>>>();

        //    foreach (KeyValuePair<PhysicsAspect, List<KeyValuePair<float, float>>> kvp in acceleratingAspects)
        //    {
        //        float targetSpeed = 0;
        //        // если в очереди есть целевые скорости
        //        if (kvp.Value.Count != 0)
        //        {
        //            // текущая скорость объекта больше ближайшей целевой
        //            if (kvp.Key.Speed >= kvp.Value[0].Key)
        //            {
        //                // запланировано переключение минимум на 2 передачи
        //                if (kvp.Value.Count > 1)
        //                {
        //                    kvp.Value.RemoveAt(0);
        //                    // устанавливаем новое ускорение
        //                    MessageDispatcher.Post(new SetAcceleration(kvp.Key.GetOwner(), null, kvp.Value[0].Value));
        //                    newAcceleratingAspects.Add(kvp.Key, kvp.Value);
        //                    continue;
        //                }
        //                else
        //                {
        //                    targetSpeed = kvp.Value[0].Key;
        //                    kvp.Value.RemoveAt(0);
        //                }

        //                targetAccelerations.Remove(kvp.Key);
        //                MessageDispatcher.Post(new SetAcceleration(kvp.Key.GetOwner(), null, 0));
        //                if (kvp.Value.Count != 0)
        //                    MessageDispatcher.Post(new SetSpeed(kvp.Key.GetOwner(), kvp.Value[0].Key));
        //                else
        //                    MessageDispatcher.Post(new SetSpeed(kvp.Key.GetOwner(), targetSpeed));
        //            }
        //            else
        //            {
        //                if (!targetAccelerations.ContainsKey(kvp.Key))
        //                {
        //                    targetAccelerations.Add(kvp.Key, kvp.Value[0].Value);
        //                    MessageDispatcher.Post(new SetAcceleration(kvp.Key.GetOwner(), kvp.Value[0].Key, kvp.Value[0].Value));
        //                }
        //                else
        //                {
        //                    // уcтановлено другое ускорение - заменяем его
        //                    if (targetAccelerations[kvp.Key] != kvp.Value[0].Value)
        //                    {
        //                        targetAccelerations[kvp.Key] = kvp.Value[0].Value;
        //                        MessageDispatcher.Post(new SetAcceleration(kvp.Key.GetOwner(), kvp.Value[0].Key, kvp.Value[0].Value));
        //                    }
        //                }
        //                newAcceleratingAspects.Add(kvp.Key, kvp.Value);
        //            }
        //        }
        //    }

        //    acceleratingAspects = newAcceleratingAspects;
        //}
    }
}
