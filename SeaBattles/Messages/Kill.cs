using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattles.Messages
{
    /// <summary>
    /// Команда уничтожения игрового объекта.
    /// </summary>
    internal class Kill
    {
        private object target;

        public object Target
        {
            get { return target; }
            //set { target = value; }
        }

        public Kill(object target)
        {
            this.target = target;
        }
    }
}
