using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaBattles
{
    internal interface IMessageHandler
    {
        void ProcessMessage(object message);
    }
}
