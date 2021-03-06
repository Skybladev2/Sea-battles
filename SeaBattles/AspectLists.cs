﻿using System;
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

        internal static ICollection<Aspect> GetDerivedAspects(Type type)
        {
            lock (syncObj)
            {
                LinkedList<Aspect> filteredAspects = new LinkedList<Aspect>();
                Dictionary<Type, LinkedList<Aspect>>.KeyCollection keys = aspects.Keys;

                foreach (Type currentType in keys)
                {
                    if (currentType.IsSubclassOf(type))
                    {
                        foreach (Aspect aspect in aspects[currentType])
                        {
                            filteredAspects.AddLast(aspect);
                        }
                    }
                }
                //if (aspects.ContainsKey(type))
                //    //return aspects[type].ToList();
                //    return new List<Aspect>(aspects[type]);
                //else
                //    return 

                return filteredAspects;
            }
        }

        //internal static ICollection<Aspect> GetAspectsByOwner(object owner)
        //{
        //    // проходим все существующие аспекты
        //    // если будет медленно, то нужно будет в базовый класс аспекта добавить список включённых аспектов
        //    lock (syncObj)
        //    {
        //        LinkedList<Aspect> result = new LinkedList<Aspect>();
        //        foreach (KeyValuePair<Type, LinkedList<Aspect>> list in aspects)
        //        {
        //            foreach (Aspect aspect in list.Value)
        //            {
        //                if (owner.Equals(aspect))
        //                    result.AddLast(aspect);
        //            }
        //        }

        //        return result;
        //    }
        //}

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
