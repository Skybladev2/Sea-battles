using System;
using System.Collections.Generic;
using System.Text;
using SeaBattles.Messages;
using OpenTK;

namespace SeaBattles.Managers
{
    public class PhysicsManager : Manager<PhysicsAspect>
    {
        private static bool initialized = false;
        private PhysicsManager() { }
        private static readonly PhysicsManager _instance = new PhysicsManager();
        public static PhysicsManager Instance
        {
            get
            {
                if (!initialized)
                {
                    _instance.Init();
                    initialized = true;
                }

                return _instance;
            }
        }

        protected override void Init()
        {
            base.Init();

            messageHandler.Handlers.Add(typeof(SetSpeed), HandleSetSpeedMany);
            messageHandler.Handlers.Add(typeof(GetOwnerPosition), HandleGetPositionMany);
            messageHandler.Handlers.Add(typeof(ButtonHold), HandleButtonHoldMany);
        }

        private bool HandleSetSpeedMany(object message)
        {
            SetSpeed setSpeed = (SetSpeed)message;
            Iterate<SetSpeed>(setSpeed, HandleSetSpeed);

            return true;
        }

        private void HandleSetSpeed(SetSpeed setSpeed, PhysicsAspect aspect)
        {
            if (setSpeed.Owner == aspect.GetOwner())
                aspect.Speed = setSpeed.Speed;
        }

        private bool HandleGetPositionMany(object message)
        {
            GetOwnerPosition getPosition = (GetOwnerPosition)message;
            Iterate<GetOwnerPosition>(getPosition, HandleGetPosition);
            return true;
        }

        private void HandleGetPosition(GetOwnerPosition getPosition, PhysicsAspect aspect)
        {
            // аспект, запрашивающий положение родителя, должен принадлежать тому же родителю, что и физика
            if (getPosition.Target.Equals(aspect.GetOwner()))
            {
                float newX =    aspect.Facing.X * (float)Math.Cos(aspect.angle / 180 * Math.PI) -
                                aspect.Facing.Y * (float)Math.Sin(aspect.angle / 180 * Math.PI);
                float newY =    aspect.Facing.X * (float)Math.Sin(aspect.angle / 180 * Math.PI) +
                                aspect.Facing.Y * (float)Math.Cos(aspect.angle / 180 * Math.PI);

                MessageDispatcher.Post(new InformPosition(getPosition.Caller,
                                                            aspect.GetOwner(),
                                                            new Vector2(newX, newY),
                                                            aspect.Velocity,
                                                            aspect.Position,
                                                            aspect.PrevPosition,
                                                            aspect.LastDT));
            }

        }

        private bool HandleButtonHoldMany(object message)
        {
            // TODO: нужно проверять, должен ли этот аспект принимать ввод
            ButtonHold buttonHold = (ButtonHold)message;
            // кагбе тут должно стоять условие, а что же делать, когда удерживается кнопка
            // но мы пока безусловно поворачиваем физику
            Iterate<ButtonHold>(buttonHold, UpdateRotation);

            return true;
        }

        private void UpdateRotation(ButtonHold buttonHold, PhysicsAspect aspect)
        {
            InputVirtualKey inputVirtualKey = buttonHold.Button;
            double dt = buttonHold.DT;

            switch (inputVirtualKey)
            {
                case InputVirtualKey.AxisLeft:
                    aspect.angle += (float)dt * aspect.rotationSpeed;
                    break;
                case InputVirtualKey.AxisRight:
                    aspect.angle -= (float)dt * aspect.rotationSpeed;
                    break;
            }

            if (aspect.angle > 360)
                aspect.angle -= 360;

            if (aspect.angle < 0)
                aspect.angle += 360;
        }
    }
}
