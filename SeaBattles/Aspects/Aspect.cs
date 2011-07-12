using System;
using System.Collections.Generic;
using System.Text;
using SeaBattles.Messages;

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
            handlers.Add(typeof(DestroySelf), Destroy);
            handlers.Add(typeof(DestroyChildrenOf), DestroyByOwner);
        }

        public Aspect(object owner)
        {
            this.owner = owner;
            handlers.Add(typeof(DestroySelf), this.Destroy);
            handlers.Add(typeof(DestroyChildrenOf), DestroyByOwner);
            //RegisterSelf();
        }

        protected void DestroyByOwner(object message)
        {
            DestroyChildrenOf destroy = (DestroyChildrenOf)message;

            // если аспект никому не принадлежит и пришло сообщение уничтожить свободные аспекты
            if (this.owner == null && destroy.Owner == null)
            {
                UnregisterSelf();
                Cleanup();
            }
            else
                // если пришло сообщение убиться от родителя
                if (this.owner != null && owner.Equals(destroy.Owner))
                {
                    UnregisterSelf();
                    MessageDispatcher.Post(new DestroyChildrenOf(this));
                    Cleanup();
                }
                // иначе игнорируем
        }

        protected void Destroy(object message)
        {
            DestroySelf destroy = (DestroySelf)message;

            if (destroy.Target.Equals(this))
            {
                UnregisterSelf();
                // посылаем сообщение об инициации удаления всех включённых аспектов
                MessageDispatcher.Post(new DestroyChildrenOf(this));
                // затем удаляем себя. нет гарантии, что аспекты внутри успели корректно удалиться
                // поэтому все аспекты не должны полагаться на то, что owner не будет null и вообще как-то пытаться зависеть от владельца
                Cleanup(); 
            }
        }

        /// <summary>
        /// Метод для освобождения ресурсов и корректного удаления аспекта.
        /// </summary>
        protected virtual void Cleanup()
        {
            //UnregisterSelf();
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
