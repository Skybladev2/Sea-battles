using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Input;
using OpenTK;

namespace SeaBattles.Messages
{
    internal class GetOwnerPosition
    {
        /// <summary>
        /// Кто запрашивает положение.
        /// </summary>
        private object caller;

        /// <summary>
        /// Чьё положение запрашивают.
        /// </summary>
        private object target;

        public object Target
        {
            get { return target; }
            //set { target = value; }
        }

        public object Caller
        {
            get { return caller; }
            //set { caller = value; }
        }

        public GetOwnerPosition(object caller, object target)
        {
            this.caller = caller;
            this.target = target;
        }
    }
}
