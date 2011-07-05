using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Input;

namespace SeaBattles.Messages
{
    internal class ButtonDown
    {
        private InputVirtualKey button;

        public InputVirtualKey Button
        {
            get { return button; }
            //set { button = value; }
        }

        public ButtonDown(InputVirtualKey button)
        {
            this.button = button;
        }
    }
}
