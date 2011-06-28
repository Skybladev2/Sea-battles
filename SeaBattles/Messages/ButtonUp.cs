using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Input;

namespace SeaBattles.Messages
{
    internal class ButtonUp
    {
        private InputVirtualKey button;

        public InputVirtualKey Button
        {
            get { return button; }
            //set { button = value; }
        }

        public ButtonUp(InputVirtualKey button)
        {
            this.button = button;
        }
    }
}
