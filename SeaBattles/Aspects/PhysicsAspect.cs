using System;
using System.Collections.Generic;
using System.Text;
using SeaBattles.Messages;
using OpenTK.Input;
using System.Drawing;
using OpenTK;

namespace SeaBattles
{
    internal class PhysicsAspect : Aspect
    {
        private Vector3 position = new Vector3(0, 0, 0);
        private Vector3 prevPosition = new Vector3(0, 0, 0);
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
        private readonly float speedMultiplier = 1;

        /// <summary>
        /// Глобальный коэффициент, влияющий на ускорение движения всех объектов в игре.
        /// По идее, должен задаваться извне.
        /// </summary>
        private readonly float accelerationMultiplier = 1;

        /// <summary>
        /// Скорость текущего объекта (вектор направления всегда единичный)
        /// </summary>
        private float speed = 0;
        /// <summary>
        /// Ускорение текущего объекта (пока что вектор ускорения совпадает с вектором скорости)
        /// </summary>
        private Vector2 acceleration = Vector2.Zero;
        /// <summary>
        /// Ускорение за текущий промежуток времени.
        /// </summary>
        private Vector2 accelerationDt = Vector2.Zero;
        private Vector2 initialFacing = new Vector2(0, 1);
        private Vector2 velocity = Vector2.Zero;


        internal float Speed
        {
            get
            {
                return speed;
            }
        }

        internal Vector2 Acceleration
        {
            get
            {
                return acceleration;
            }
        }

        internal Vector2 Velocity
        {
            get
            {
                return this.velocity;
                //Vector2 temp = Vector2.Multiply(initialFacing, speed * speedMultiplier);
                //// поворот вектора
                //float newX = temp.X * (float)Math.Cos(angle / 180 * Math.PI) - temp.Y * (float)Math.Sin(angle / 180 * Math.PI);
                //float newY = temp.X * (float)Math.Sin(angle / 180 * Math.PI) + temp.Y * (float)Math.Cos(angle / 180 * Math.PI);
                //return new Vector2(newX, newY);
            }
        }

        internal Vector2 Facing
        {
            get { return Misc.RotateVector(initialFacing, angle); }
        }

        internal Vector2 PrevFacing
        {
            get
            {
                //// http://en.wikipedia.org/wiki/Rotation_(mathematics)
                //// поворот вектора
                //float newX = facing.X * (float)Math.Cos(prevAngle / 180 * Math.PI) - facing.Y * (float)Math.Sin(prevAngle / 180 * Math.PI);
                //float newY = facing.X * (float)Math.Sin(prevAngle / 180 * Math.PI) + facing.Y * (float)Math.Cos(prevAngle / 180 * Math.PI);
                //return new Vector2(newX, newY);
                return Misc.RotateVector(initialFacing, prevAngle);
            }
        }

        public static PhysicsAspect Create(object owner)
        {
            PhysicsAspect aspect = new PhysicsAspect(owner);
            aspect.RegisterAllStuff();
            return aspect;
        }

        public static PhysicsAspect Create(object owner, Vector3 position, Vector2 facing)
        {
            PhysicsAspect aspect = new PhysicsAspect(owner, position, facing);
            aspect.RegisterAllStuff();
            return aspect;
        }

        public static PhysicsAspect Create(object owner, Vector3 position, Vector2 facing, float speed)
        {
            PhysicsAspect aspect = new PhysicsAspect(owner, position, facing, speed);
            aspect.RegisterAllStuff();
            return aspect;
        }

        private PhysicsAspect(object owner)
            : base(owner)
        {
            messageHandler.Handlers.Add(typeof(SetSpeed), HandleSetSpeed);
            messageHandler.Handlers.Add(typeof(SetForwardAcceleration), HandleSetForwardAcceleration);
            messageHandler.Handlers.Add(typeof(SetTargetAcceleration), HandleSetTargetAcceleration);
            messageHandler.Handlers.Add(typeof(GetOwnerPosition), HandleGetPosition);
            messageHandler.Handlers.Add(typeof(ButtonHold), HandleButtonHold);
        }

        private PhysicsAspect(object owner, Vector3 position, Vector2 facing)
            : this(owner)
        {
            this.position = position;
            this.initialFacing = facing;
        }

        private PhysicsAspect(object owner, Vector3 position, Vector2 facing, float speed)
            : this(owner, position, facing)
        {
            this.speed = speed;
        }

        private bool HandleSetSpeed(object message)
        {
            SetSpeed setSpeed = (SetSpeed)message;
            if (setSpeed.Owner == this.owner)
                this.speed = setSpeed.Speed;

            //MessageDispatcher.Post(new TraceText("Velocity: " + this.Velocity.ToString() + ", Angle: " + angle));
            return true;
        }

        private bool HandleSetTargetAcceleration(object message)
        {
            SetTargetAcceleration setAcceleration = (SetTargetAcceleration)message;
            if (setAcceleration.Owner == this.owner)
            {
                //this.acceleration = setAcceleration.Acceleration;
                if (setAcceleration.TargetSpeed != null)
                    PhysicsManager.AddTargetSpeedAspect(this, setAcceleration.TargetSpeed.Value, setAcceleration.TargetAcceleration);
            }

            //MessageDispatcher.Post(new TraceText("Velocity: " + this.Velocity.ToString() + ", Angle: " + angle));
            return true;
        }

        private bool HandleSetForwardAcceleration(object message)
        {
            SetForwardAcceleration setAcceleration = (SetForwardAcceleration)message;
            if (setAcceleration.Owner == this.owner)
                this.acceleration = setAcceleration.TargetAcceleration;

            return true;
        }

        private bool HandleGetPosition(object message)
        {
            GetOwnerPosition getPosition = (GetOwnerPosition)message;

            // аспект, запрашивающий положение родителя, должен принадлежать тому же родителю, что и физика
            if (getPosition.Target.Equals(this.owner))
            {
                float newX = initialFacing.X * (float)Math.Cos(angle / 180 * Math.PI) -
                             initialFacing.Y * (float)Math.Sin(angle / 180 * Math.PI);
                float newY = initialFacing.X * (float)Math.Sin(angle / 180 * Math.PI) +
                             initialFacing.Y * (float)Math.Cos(angle / 180 * Math.PI);

                MessageDispatcher.Post(new InformPosition(getPosition.Caller,
                                                            this.owner,
                                                            new Vector2(newX, newY),
                                                            this.Velocity,
                                                            this.position,
                                                            this.prevPosition,
                                                            this.lastDT));
            }

            return true;
        }

        private bool HandleButtonHold(object message)
        {
            ButtonHold buttonHold = (ButtonHold)message;
            // кагбе тут должно стоять условие, а что же делать, когда удерживается кнопка
            // но мы пока безусловно поворачиваем физику
            UpdateRotation(buttonHold.Button, buttonHold.DT);

            return true;
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

            this.velocity = Misc.RotateVector(initialFacing, angle) * this.velocity.Length;

            MessageDispatcher.Post(new SetPosition(this.owner,
                                                    this.velocity,
                                                    this.position,
                                                    this.angle,
                                                    this.lastDT,
                                                    Misc.RotateVector(initialFacing, angle)));

            //MessageDispatcher.Post(new TraceText("Velocity: " + this.Velocity.ToString() + ", Angle: " + angle));
        }

        internal void Update(double dt)
        {
            this.prevPosition.X = position.X;
            this.prevPosition.Y = position.Y;
            this.lastDT = (float)dt;

            this.velocity += acceleration * accelerationMultiplier * (float)dt;
            //this.speed += acceleration * accelerationMultiplier * (float)dt;

            //Vector2 currentFacing = this.Velocity;
            //if (currentFacing != Vector2.Zero)
            //    currentFacing.Normalize();

            position = Vector3.Add(position,
                                    new Vector3(
                //Vector2.Multiply(currentFacing, acceleration * (float)Math.Pow(dt, 2)) +
                                                Vector2.Multiply(this.Velocity,
                                                                (float)dt)
                                               )
                                    );
            MessageDispatcher.Post(new SetPosition(this.owner,
                                                this.Velocity,
                                                this.position,
                                                this.angle,
                                                (float)dt,
                                                Misc.RotateVector(initialFacing, angle)));

            if (((Aspect)this.owner).Name == "player")
                MessageDispatcher.Post(new TraceText(this.acceleration + " " + this.Velocity.LengthFast.ToString()));
        }

        internal object GetOwner()
        {
            return this.owner;
        }

        //internal void StopAcceleration()
        //{
        //    acceleration = 0;
        //}

        //internal void SetSpeed(float speed)
        //{
        //    this.speed = speed;
        //}
    }
}
