using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaBattles
{
    public abstract class Aspect : IMessageHandler
    {
        protected object owner = null;
        // соответствие сообщений классам (аспектам), их обрабатывающим
        protected Dictionary<Type, LinkedList<IMessageHandler>> handlersMap = new Dictionary<Type, LinkedList<IMessageHandler>>();
        // соответствие сообщений методам, их обрабатывающим
        protected Dictionary<Type, HandlerMethodDelegate> handlers = new Dictionary<Type, HandlerMethodDelegate>();

        public Aspect()
        {
            this.owner = null;
        }

        public Aspect(object owner)
        {
            this.owner = owner;
            //RegisterSelf();
        }

        public virtual void Destroy()
        {
            UnregisterSelf();
        }

        protected void RegisterSelf()
        {
            AspectLists.AddAspect(this);
        }

        protected void UnregisterSelf()
        {
            AspectLists.RemoveAspect(this);
        }

        protected void AddHandlerToMap(Type type, IMessageHandler handler)
        {
            if (!handlersMap.ContainsKey(type))
            {
                LinkedList<IMessageHandler> list = new LinkedList<IMessageHandler>();
                list.AddFirst(handler);
                handlersMap.Add(type, list);
            }
            else
                if (!handlersMap[type].Contains(handler))
                    handlersMap[type].AddLast(handler);
        }

        #region IMessageHandler Members

        public void ProcessMessage(object message)
        {
            Type type = message.GetType();

            // так как данный класс выступает и как аспект, и как агрегация аспектов,
            // то у него 2 списка обработчиков -
            // собственные методы и объекты-аспекты.
            // Сначала сообщения проходят через собственные методы
            if (handlers.ContainsKey(type))
                handlers[type](message);

            if (handlersMap.ContainsKey(type))
                foreach (IMessageHandler handler in handlersMap[type])
                {
                    handler.ProcessMessage(message);
                }
        }

        #endregion
    }
}
