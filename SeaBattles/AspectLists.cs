using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattles
{
    internal static class AspectLists
    {
        private static object syncObj = new object();
        private static Dictionary<Type, LinkedList<Aspect>> aspects = new Dictionary<Type, LinkedList<Aspect>>();

        internal static ICollection<Aspect> GetAspects(Type type)
        {
            lock (syncObj)
            {
                if (aspects.ContainsKey(type))
                    //return aspects[type].ToList();
                    return new List<Aspect>(aspects[type]);
                else
                    return new LinkedList<Aspect>();
            }
        }

        internal static void AddAspect(Aspect aspect)
        {
            Type type = aspect.GetType();

            lock (syncObj)
            {
                if (aspects.ContainsKey(type))
                {
                    LinkedList<Aspect> list = aspects[type];
                    if (!list.Contains(aspect))
                        list.AddLast(aspect);
                }
                else
                {
                    LinkedList<Aspect> newlist = new LinkedList<Aspect>();
                    newlist.AddFirst(aspect);
                    aspects.Add(type, newlist);
                }
            }
        }

        internal static void RemoveAspect(Aspect aspect)
        {
            Type type = aspect.GetType();
            lock (syncObj)
            {
                if (aspects.ContainsKey(type))
                    aspects[type].Remove(aspect);
            }
        }
    }
}
