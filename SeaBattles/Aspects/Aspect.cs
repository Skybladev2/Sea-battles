using System;
using System.Collections.Generic;
using System.Text;
using SeaBattles.Messages;

namespace SeaBattles
{
    public abstract class Aspect : IMessageHandler
    {
        protected object owner = null;
        protected MessageHandler messageHandler = new MessageHandler();
        //protected LinkedList<Aspect> aspects = new LinkedList<Aspect>();

        public Aspect()
        {
            this.owner = null;
            messageHandler.Handlers.Add(typeof(DestroySelf), Destroy);
            messageHandler.Handlers.Add(typeof(DestroyChildrenOf), DestroyByOwner);
            MessageDispatcher.RegisterHandler(typeof(DestroySelf), this);
            MessageDispatcher.RegisterHandler(typeof(DestroyChildrenOf), this);
        }

        public Aspect(object owner)
        {
            this.owner = owner;
            messageHandler.Handlers.Add(typeof(DestroySelf), this.Destroy);
            messageHandler.Handlers.Add(typeof(DestroyChildrenOf), DestroyByOwner);
            MessageDispatcher.RegisterHandler(typeof(DestroySelf), this);
            MessageDispatcher.RegisterHandler(typeof(DestroyChildrenOf), this);

            //if (owner != null)
            //{
            //    Aspect ownerAspect = owner as Aspect;
            //    if (ownerAspect != null)
            //    {
            //        ownerAspect.AddToOwnAspectList(this);
            //    }
            //}

            //RegisterSelf();
        }

        //protected void AddToOwnAspectList(Aspect child)
        //{
        //    this.aspects.AddLast(child);
        //}

        protected void UnRegisterHandlerInParent(Type type)
        {
            if (owner != null)
            {
                Aspect ownerAspect = owner as Aspect;
                if (ownerAspect != null)
                {
                    ownerAspect.messageHandler.Handlers.Remove(type);
                    ownerAspect.messageHandler.HandlersMap.Remove(type);
                    ownerAspect.UnRegisterHandlerInParent(type);
                }
            }
        }

        protected void RegisterHandlersInParent()
        {
            if (owner != null)
            {
                Aspect ownerAspect = owner as Aspect;
                if (ownerAspect != null)
                {
                    foreach (KeyValuePair<Type, HandlerMethodDelegate> d in messageHandler.Handlers)
                    {
                        ownerAspect.AddHandlerToMap(d.Key, this);
                    }

                    foreach (KeyValuePair<Type, LinkedList<IMessageHandler>> handlerMap in messageHandler.HandlersMap)
                    {
                        foreach (IMessageHandler aspectHandler in handlerMap.Value)
                        {
                            ownerAspect.AddHandlerToMap(handlerMap.Key, aspectHandler);
                        }
                    }
                }
            }
        }

        protected void RegisterAllStuff()
        {
            // регистрируем собстввенные обработчики у родителя, чтобы он получал соотвествующие сообщения и передавал их нам
            RegisterHandlersInParent();

            // регистрируем себя в списке аспектов
            RegisterSelf();
        }

        protected bool DestroyByOwner(object message)
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
                else
                    if (this.Equals(destroy.Owner)) // сообщение об уничтожении дочерних объектов родителя пришло родителю
                    {
                        UnregisterSelf();
                        Cleanup();
                    }
            // иначе игнорируем

            return true;
        }

        protected bool Destroy(object message)
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

            return true;
        }

        /// <summary>
        /// Метод для освобождения ресурсов и корректного удаления аспекта.
        /// </summary>
        protected virtual void Cleanup()
        {
            //this.aspects.Clear();
            //UnregisterSelf();
        }

        protected void RegisterSelf()
        {
            AspectLists.AddAspect(this);
        }

        protected void UnregisterSelf()
        {
            AspectLists.RemoveAspect(this);
            MessageDispatcher.UnRegisterHandler(typeof(DestroySelf), this);
            MessageDispatcher.UnRegisterHandler(typeof(DestroyChildrenOf), this);
        }

        protected void AddHandlerToMap(Type type, IMessageHandler handler)
        {
            if (!messageHandler.HandlersMap.ContainsKey(type))
            {
                LinkedList<IMessageHandler> list = new LinkedList<IMessageHandler>();
                list.AddFirst(handler);
                messageHandler.HandlersMap.Add(type, list);
            }
            else
                if (!messageHandler.HandlersMap[type].Contains(handler))
                    messageHandler.HandlersMap[type].AddLast(handler);
        }

        #region IMessageHandler Members

        public void ProcessMessage(object message)
        {
            messageHandler.ProcessMessage(message);
        }

        #endregion
    }
}
