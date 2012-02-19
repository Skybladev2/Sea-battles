using System;
using System.Collections.Generic;
using System.Text;
using SeaBattles.Messages;

namespace SeaBattles
{
    internal class DamageManager
    {
        private static Dictionary<KeyValuePair<Type, Type>, KeyValuePair<int, int>> damages = new Dictionary<KeyValuePair<Type, Type>, KeyValuePair<int, int>>();

        static DamageManager()
        {
            damages.Add(new KeyValuePair<Type, Type>(typeof(Ship), typeof(Shell)), new KeyValuePair<int, int>(20, 100));
        }

        internal static void ApplyDamage(IDamageable first, IDamageable second)
        {
            KeyValuePair<Type, Type> pair = new KeyValuePair<Type, Type>(first.GetType(), second.GetType());
            if (damages.ContainsKey(pair))
            {
                KeyValuePair<int, int> damage = damages[pair];
                bool firstKilled = first.ApplyDamage(damage.Key);
                bool secondKilled = second.ApplyDamage(damage.Value);
                if (firstKilled)
                    MessageDispatcher.Post(new Kill(first));
                if (secondKilled)
                    MessageDispatcher.Post(new Kill(second));
            }
        }
    }
}
