using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaBattles
{
    public interface IMessageHandler
    {
        void ProcessMessage(object message);
    }
}
