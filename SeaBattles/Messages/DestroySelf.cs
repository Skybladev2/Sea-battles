using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattles.Messages
{
    /// <summary>
    /// Команда самоуничтожения аспекта.
    /// </summary>
    internal class DestroySelf
    {
        private object target;

        public object Target
        {
            get { return target; }
            //set { target = value; }
        }

        public DestroySelf(object target)
        {
            this.target = target;
        }
    }
}
