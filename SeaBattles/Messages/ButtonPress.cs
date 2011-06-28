using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Input;

namespace SeaBattles.Messages
{
    internal class ButtonPress
    {
        private InputVirtualKey button;

        public InputVirtualKey Button
        {
            get { return button; }
            //set { button = value; }
        }

        public ButtonPress(InputVirtualKey button)
        {
            this.button = button;
        }
    }
}
