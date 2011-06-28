using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaBattles
{
    public delegate void HandlerMethodDelegate(object message);

    internal class MessageDispatcher
    {
        private static Dictionary<Type, LinkedList<IMessageHandler>> handlers = new Dictionary<Type, LinkedList<IMessageHandler>>();

        public static void RegisterHandler(Type type, IMessageHandler handlerObject)
        {
            if (handlers.ContainsKey(type))
            {
                Console.WriteLine("Type found - checking handlers list");
                LinkedList<IMessageHandler> typeHandlers = handlers[type];
                if (typeHandlers == null) // для этого типа раньше не регистрировались подписчики
                {
                    typeHandlers = new LinkedList<IMessageHandler>();
                }

                // WARNING: linear search
                if (!typeHandlers.Contains(handlerObject)) // для этого типа и этого объекта не назначен этот же объект-обработчик
                    typeHandlers.AddLast(handlerObject);
            }
            else
            {
                Console.WriteLine("Type not found - creating handlers list");
                handlers.Add(type, new LinkedList<IMessageHandler>());
                handlers[type].AddLast(handlerObject);
            }
        }

        public static void UnRegisterHandler(Type type, IMessageHandler handlerObject)
        {
            if (handlers.ContainsKey(type))
            {
                if (true)
                    Console.WriteLine();

                Console.WriteLine("Type found - checking handlers list");
                LinkedList<IMessageHandler> typeHandlers = handlers[type];
                typeHandlers.Remove(handlerObject);
            }
            else
            {
                Console.WriteLine("Type not found - nothing to unregister");
            }
        }

        public static void Post(object message)
        {
            Type type = message.GetType();

            if (handlers.ContainsKey(type))
            {
                LinkedList<IMessageHandler> typeHandlers = handlers[type];
                if (typeHandlers != null)
                    foreach (IMessageHandler handler in typeHandlers)
                    {
                        handler.ProcessMessage(message);
                    }
                else
                    Console.WriteLine("Type found, but it handler list is null");
            }
            //else
                //Console.WriteLine("Type not found");
        }

        #region Singleton
        private MessageDispatcher() { }
        private static MessageDispatcher _instance = new MessageDispatcher();
        public static MessageDispatcher Instance { get { return _instance; } }
        #endregion
    }
}
