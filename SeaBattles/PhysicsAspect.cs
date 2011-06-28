using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeaBattles.Messages;
using OpenTK.Input;
using System.Drawing;
using OpenTK;

namespace SeaBattles
{
    internal class PhysicsAspect : IMessageHandler
    {
        private object owner;
        private Dictionary<Type, HandlerMethodDelegate> handlers = new Dictionary<Type, HandlerMethodDelegate>();
        private Vector2 position = new Vector2(0, 0);
        private float angle = 0;
        private float rotationSpeed = 100;
        private float speedMultiplier = 0.1f;
        //private Vector2 velocity = new Vector2(0, 1);
        private Vector2 facing = new Vector2(0, 1);
        private float[] gears = new float[] { 0, 1, 2, 3 };
        // индекс передачи в массиве gears
        private int currentGear = 0;

        internal Vector2 Velocity
        {
            get
            {
                Vector2 temp = Vector2.Multiply(facing, gears[currentGear] * speedMultiplier);
                // поворот вектора
                float newX = temp.X * (float)Math.Cos(angle / 180 * Math.PI) - temp.Y * (float)Math.Sin(angle / 180 * Math.PI);
                float newY = temp.X * (float)Math.Sin(angle / 180 * Math.PI) + temp.Y * (float)Math.Cos(angle / 180 * Math.PI);
                return new Vector2(newX, newY);
            }
        }

        public PhysicsAspect(object owner)
        {
            this.owner = owner;
            handlers.Add(typeof(ButtonDown), HandleButtonDown);
            handlers.Add(typeof(GetOwnerPosition), HandleButtonDown);
        }

        public PhysicsAspect(object owner, Vector2 position, Vector2 facing)
            : this(owner)
        {
            this.position = position;
            this.facing = facing;
        }

        public PhysicsAspect(object owner, Vector2 position, Vector2 facing, float velocity) : this(owner, position, facing)
        {
            //this.v

        }

        #region IMessageHandler Members

        public void ProcessMessage(object message)
        {
            Type type = message.GetType();
            handlers[type](message);
        }

        #endregion

        private void HandleGetPosition(object message)
        {
            GetOwnerPosition getPosition = (GetOwnerPosition)message;

           // аспект, запрашивающий положение родителя, должен принадлежать тому же родителю, что и физика
            if (getPosition.Target.Equals(this.owner))
                MessageDispatcher.Post(new InformPosition(getPosition.Caller, this.owner, this.Velocity, this.position));
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
                    //ShootLeft();
                    break;
                case InputVirtualKey.Action2:
                    //ShootLeft();
                    break;
                case InputVirtualKey.Action3:
                    //ShootRight();
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

            MessageDispatcher.Post(new TraceText("Velocity: " + this.Velocity.ToString() + ", Angle: " + angle));

            //velocity.Normalize(); // не очень кошерно, так как могут быть артефакты с рывками движения
            //this.velocity = Vector2.Multiply(velocity, gears[currentGear]);
        }

        private void IncreaseSpeed()
        {
            if (currentGear < gears.Length - 1)
                currentGear++;

            MessageDispatcher.Post(new TraceText("Velocity: " + this.Velocity.ToString() + ", Angle: " + angle));
            //velocity.Normalize(); // не очень кошерно, так как могут быть артефакты с рывками движения
            //this.velocity = Vector2.Multiply(velocity, gears[currentGear]);
        }

        internal void UpdateRotation(InputVirtualKey inputVirtualKey, double dt)
        {
            switch (inputVirtualKey)
            {
                case InputVirtualKey.AxisLeft:
                    angle += (float)dt * rotationSpeed;
                    break;
                case InputVirtualKey.AxisRight:
                    angle -= (float)dt * rotationSpeed;
                    break;
            }

            if (angle > 360)
                angle -= 360;

            if (angle < 0)
                angle += 360;

            MessageDispatcher.Post(new TraceText("Velocity: " + this.Velocity.ToString() + ", Angle: " + angle));
        }

        internal void Update(double dt)
        {
            position = Vector2.Add(position, Vector2.Multiply(this.Velocity, (float)dt));
            MessageDispatcher.Post(new SetPosition(this.owner, this.Velocity, this.position, this.angle));
        }
    }
}
