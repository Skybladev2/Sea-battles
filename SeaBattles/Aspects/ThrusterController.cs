using System;
using System.Collections.Generic;
using System.Text;
using SeaBattles.Messages;
using OpenTK.Input;
using System.Drawing;
using OpenTK;

namespace SeaBattles
{
    /// <summary>
    /// Ведает тяговыми двигателями в пределах объекта.
    /// </summary>
    internal class ThrusterController : Aspect
    {
        IList<Thruster> thrusters = null;

        private ThrusterController(object owner)
            : base(owner)
        {
            // при движении физического аспекта _поворачиваем_ и тяговый двигатель
            messageHandler.Handlers.Add(typeof(SetPosition), HandleSetPosition);
            messageHandler.Handlers.Add(typeof(ButtonDown), HandleButtonDown);
        }

        public static ThrusterController Create(object owner, IList<Thruster> thrusters)
        {
            ThrusterController aspect = new ThrusterController(owner);
            aspect.thrusters = thrusters;
            aspect.RegisterAllStuff();
            return aspect;
        }

        protected override void Cleanup()
        {
            base.Cleanup();
            foreach (Thruster thruster in thrusters)
            {
                MessageDispatcher.Post(new DestroySelf(thruster));
            }
            thrusters.Clear();
        }

        private bool HandleSetPosition(object message)
        {
            SetPosition setPosition = (SetPosition)message;
            if (setPosition.Target == this.owner)
            {
                foreach (Thruster thruster in this.thrusters)
                {
                    thruster.SetRotation(setPosition.Facing);
                }
            }

            return true;
        }

        private bool HandleButtonDown(object message)
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

            return true;
        }

        private void DecreaseSpeed()
        {
            foreach (Thruster thruster in thrusters)
            {
                if (thruster.IsOn)
                {
                    thruster.IsOn = false;
                    break;
                }
            }
        }

        private void IncreaseSpeed()
        {
            foreach (Thruster thruster in thrusters)
            {
                if (!thruster.IsOn)
                {
                    thruster.IsOn = true;
                    break;
                }
            }
        }
    }
}
