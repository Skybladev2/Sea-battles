using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattles
{
    public class MessageHandler : IMessageHandler
    {
        // соответствие сообщений классам (аспектам), их обрабатывающим
        protected Dictionary<Type, LinkedList<IMessageHandler>> handlersMap = new Dictionary<Type, LinkedList<IMessageHandler>>();
        // соответствие сообщений методам, их обрабатывающим
        protected Dictionary<Type, HandlerMethodDelegate> handlers = new Dictionary<Type, HandlerMethodDelegate>();

        public Dictionary<Type, LinkedList<IMessageHandler>> HandlersMap
        {
            get { return handlersMap; }
        }

        public Dictionary<Type, HandlerMethodDelegate> Handlers
        {
            get { return handlers; }
        }

        #region IMessageHandler Members

        public void ProcessMessage(object message)
        {
            Type type = message.GetType();

            bool continueToHandle = true;
            // так как данный класс выступает и как аспект, и как агрегация аспектов,
            // то у него 2 списка обработчиков -
            // собственные методы и объекты-аспекты.
            // Сначала сообщения проходят через собственные методы
            if (handlers.ContainsKey(type))
                continueToHandle = continueToHandle && handlers[type](message);

            if (handlersMap.ContainsKey(type))
                foreach (IMessageHandler handler in handlersMap[type])
                {
                    handler.ProcessMessage(message);
                }
        }

        #endregion
    }
}
