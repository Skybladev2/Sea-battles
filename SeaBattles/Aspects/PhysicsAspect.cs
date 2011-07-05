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
    internal class PhysicsAspect : Aspect
    {
        private Vector2 position = new Vector2(0, 0);
        private Vector2 prevPosition = new Vector2(0, 0);
        private float angle = 0;
        private float prevAngle = 0;
        private float lastDT = 1;

        /// <summary>
        /// Глобальный коэффициент, влияющий на скорость поворота всех объектов в игре.
        /// По идее, должен задаваться извне.
        /// </summary>
        private readonly float rotationSpeed = 100;
        /// <summary>
        /// Глобальный коэффициент, влияющий на скорость движения всех объектов в игре.
        /// По идее, должен задаваться извне.
        /// </summary>
        private readonly float speedMultiplier = 0.1f;

        /// <summary>
        /// Скорость текущего объекта (вектор направления всегда единичный)
        /// </summary>
        private float speed = 0;
        private Vector2 facing = new Vector2(0, 1);
        
        internal Vector2 Velocity
        {
            get
            {
                Vector2 temp = Vector2.Multiply(facing, speed * speedMultiplier);
                // поворот вектора
                float newX = temp.X * (float)Math.Cos(angle / 180 * Math.PI) - temp.Y * (float)Math.Sin(angle / 180 * Math.PI);
                float newY = temp.X * (float)Math.Sin(angle / 180 * Math.PI) + temp.Y * (float)Math.Cos(angle / 180 * Math.PI);
                return new Vector2(newX, newY);
            }
        }

        internal Vector2 PrevFacing
        {
            get
            {
                // поворот вектора
                float newX = facing.X * (float)Math.Cos(prevAngle / 180 * Math.PI) - facing.Y * (float)Math.Sin(prevAngle / 180 * Math.PI);
                float newY = facing.X * (float)Math.Sin(prevAngle / 180 * Math.PI) + facing.Y * (float)Math.Cos(prevAngle / 180 * Math.PI);
                return new Vector2(newX, newY);
            }
        }

        public PhysicsAspect(object owner) : base(owner)
        {
            handlers.Add(typeof(SetSpeed), HandleSetSpeed);
            handlers.Add(typeof(GetOwnerPosition), HandleGetPosition);
            RegisterSelf();
        }

        public PhysicsAspect(object owner, Vector2 position, Vector2 facing)
            : base(owner)
        {
            this.position = position;
            this.facing = facing;
            handlers.Add(typeof(SetSpeed), HandleSetSpeed);
            handlers.Add(typeof(GetOwnerPosition), HandleGetPosition);
            RegisterSelf();
        }

        public PhysicsAspect(object owner, Vector2 position, Vector2 facing, float speed) : base(owner)
        {
            this.speed = speed;
            this.position = position;
            this.facing = facing;
            handlers.Add(typeof(SetSpeed), HandleSetSpeed);
            handlers.Add(typeof(GetOwnerPosition), HandleGetPosition);
            RegisterSelf();
        }

        private void HandleSetSpeed(object message)
        {
            SetSpeed setSpeed = (SetSpeed)message;
            this.speed = setSpeed.Speed;

            //MessageDispatcher.Post(new TraceText("Velocity: " + this.Velocity.ToString() + ", Angle: " + angle));
        }

        private void HandleGetPosition(object message)
        {
            GetOwnerPosition getPosition = (GetOwnerPosition)message;

           // аспект, запрашивающий положение родителя, должен принадлежать тому же родителю, что и физика
            if (getPosition.Target.Equals(this.owner))
            {
                float newX = facing.X * (float)Math.Cos(angle / 180 * Math.PI) - facing.Y * (float)Math.Sin(angle / 180 * Math.PI);
                float newY = facing.X * (float)Math.Sin(angle / 180 * Math.PI) + facing.Y * (float)Math.Cos(angle / 180 * Math.PI);

                MessageDispatcher.Post(new InformPosition(getPosition.Caller,
                                                            this.owner,
                                                            new Vector2(newX, newY),
                                                            this.Velocity,
                                                            this.position,
                                                            this.prevPosition,
                                                            this.lastDT));
            }
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

            //MessageDispatcher.Post(new TraceText("Velocity: " + this.Velocity.ToString() + ", Angle: " + angle));
        }

        internal void Update(double dt)
        {
            this.prevPosition.X = position.X;
            this.prevPosition.Y = position.Y;
            this.lastDT = (float)dt;

            position = Vector2.Add(position, Vector2.Multiply(this.Velocity, (float)dt));
            MessageDispatcher.Post(new SetPosition(this.owner, this.Velocity, this.position, this.angle));
            if (this.owner.GetType() == typeof(Ship))
                MessageDispatcher.Post(new TraceText(this.position.ToString()));
        }
    }
}
