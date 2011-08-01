using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattles.Messages
{
    internal class DestroyChildrenOf
    {
        private object owner;

        public object Owner
        {
            get { return owner; }
            //set { parent = value; }
        }

        public DestroyChildrenOf(object owner)
        {
            this.owner = owner;
        }
    }
}
