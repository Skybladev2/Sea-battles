using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Input;

namespace SeaBattles.Messages
{
    internal class ButtonHold
    {
        private InputVirtualKey button;
        private double dt;

        public InputVirtualKey Button
        {
            get { return button; }
            //set { button = value; }
        }

        public double DT
        {
            get { return dt; }
            //set { button = value; }
        }

        public ButtonHold(InputVirtualKey button, double dt)
        {
            this.button = button;
            this.dt = dt;
        }
    }
}
