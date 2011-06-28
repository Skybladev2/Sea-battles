using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeaBattles.Messages;

namespace SeaBattles
{
    class VehicleWithGearboxAspect : IMessageHandler
    {
        private object owner;
        private Dictionary<Type, HandlerMethodDelegate> handlers = new Dictionary<Type, HandlerMethodDelegate>();

        // (целевая) скорость на каждой передаче в м/с
        private float[] gears = new float[] { 0, 1, 2, 3 };
        // индекс передачи в массиве gears
        private int currentGear = 0;

        public VehicleWithGearboxAspect(object owner)
        {
            this.owner = owner;
            handlers.Add(typeof(ButtonDown), HandleButtonDown);
        }

        private void HandleButtonDown(object message)
        {
            ButtonDown butttonDown = (ButtonDown)message;
            InputVirtualKey key = butttonDown.Button;

            switch (key)
            {
                case InputVirtualKey.Unknown:
                    break;
                case InputVirtualKey.AxisLeft:
                    break;
                case InputVirtualKey.AxisRight:
                    break;
                case InputVirtualKey.AxisUp:
                    IncreaseSpeed();
                    break;
                case InputVirtualKey.AxisDown:
                    DecreaseSpeed();
                    break;
                case InputVirtualKey.Action1:
                    break;
                case InputVirtualKey.Action2:
                    break;
                case InputVirtualKey.Action3:
                    break;
                case InputVirtualKey.Action4:
                    break;
                case InputVirtualKey.Action5:
                    break;
                case InputVirtualKey.Action6:
                    break;
                case InputVirtualKey.Action7:
                    break;
                case InputVirtualKey.Action8:
                    break;
                case InputVirtualKey.Action9:
                    break;
                case InputVirtualKey.Action10:
                    break;
                case InputVirtualKey.Action11:
                    break;
                case InputVirtualKey.Action12:
                    break;
                case InputVirtualKey.Action13:
                    break;
                case InputVirtualKey.Action14:
                    break;
                case InputVirtualKey.Action15:
                    break;
                case InputVirtualKey.Action16:
                    break;
                default:
                    break;
            }
        }

        private void DecreaseSpeed()
        {
            if (currentGear > 0)
                currentGear--;

            MessageDispatcher.Post(new SetSpeed(currentGear));
            //MessageDispatcher.Post(new TraceText("Velocity: " + this.Velocity.ToString() + ", Angle: " + angle));
        }

        private void IncreaseSpeed()
        {
            if (currentGear < gears.Length - 1)
                currentGear++;

            MessageDispatcher.Post(new SetSpeed(currentGear));
            //MessageDispatcher.Post(new TraceText("Velocity: " + this.Velocity.ToString() + ", Angle: " + angle));
        }

        #region IMessageHandler Members

        public void ProcessMessage(object message)
        {
            Type type = message.GetType();
            handlers[type](message);
        }

        #endregion
    }
}
