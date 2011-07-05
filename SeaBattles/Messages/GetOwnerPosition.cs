using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Input;
using OpenTK;

namespace SeaBattles.Messages
{
    /// <summary>
    /// Получает координаты PhysicsAspect объекта, к которому принадлежит запрашивающий аспект.
    /// Так как аспекты не знают, какие другие аспекты находятся рядом с ними (принадлежат одному
    /// и тому же объекту), то запрос идёт через родителя - отсюда такое странное название сообщения.
    /// </summary>
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
