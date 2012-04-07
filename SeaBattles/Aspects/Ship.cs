using System;
using System.Collections.Generic;
using System.Text;
using SeaBattles.Messages;
using OpenTK;
using System.Drawing;

namespace SeaBattles
{
    /// <summary>
    /// Корабль. Может управляться игроком или ИИ.
    /// </summary>
    internal class Ship : Aspect, IDamageable
    {
        private PhysicsAspect physics;
        private MassAspect mass;
        //private VehicleWithGearboxAspect mechanics;
        private ThrusterController thrusterController;
        private GraphicsAspect graphics;
        private BoundSetAspect bounds;
        private DamageAspect damage;

        private Weapon leftCannon;
        private Weapon rightCannon;
        private Weapon rearCannon;
        private DestroyByTimerAspect timer = null;

        internal ThrusterController ThrusterController
        {
            get { return this.thrusterController; }
        }

        //internal VehicleWithGearboxAspect Mechanics
        //{
        //    get { return mechanics; }
        //    //set { mechanics = value; }
        //}

        internal GraphicsAspect Graphics
        {
            get { return graphics; }
            //set { graphics = value; }
        }

        internal PhysicsAspect Physics
        {
            get { return physics; }
            //set { physics = value; }
        }

        internal MassAspect Mass
        {
            get { return mass; }
        }

        internal Weapon RearCannon
        {
            get { return rearCannon; }
        }

        public static Ship Create(PointF position, float length, float width)
        {
            Ship aspect = new Ship(position, length, width);

            MessageDispatcher.RegisterHandler(typeof(SetPosition), aspect);
            MessageDispatcher.RegisterHandler(typeof(SetSpeed), aspect);
            MessageDispatcher.RegisterHandler(typeof(SetAcceleration), aspect);
            MessageDispatcher.RegisterHandler(typeof(SetForwardAcceleration), aspect);
            MessageDispatcher.RegisterHandler(typeof(SetTargetAcceleration), aspect);
            MessageDispatcher.RegisterHandler(typeof(ApplyForce), aspect);
            // нужно для определения координат и скорости корабля в момент выстрела
            // в данном случае owner-ом является ship
            MessageDispatcher.RegisterHandler(typeof(GetOwnerPosition), aspect);
            MessageDispatcher.RegisterHandler(typeof(InformPosition), aspect);
            MessageDispatcher.RegisterHandler(typeof(Shoot), aspect);
            MessageDispatcher.RegisterHandler(typeof(BoundSetCollision), aspect);
            MessageDispatcher.RegisterHandler(typeof(BoundSetNotCollision), aspect);
            MessageDispatcher.RegisterHandler(typeof(Ship), aspect);
            MessageDispatcher.RegisterHandler(typeof(Kill), aspect);

            aspect.RegisterAllStuff();

            return aspect;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="shipVerts"></param>
        private Ship(PointF position, float length, float width)
        {
            float depth = 0;
            List<Vector3> shipVerts = new List<Vector3>();
            shipVerts.Add(new Vector3(-1f * width / 2 + position.X, -1f * length / 2 + position.Y, depth));
            shipVerts.Add(new Vector3(-1f * width / 2 + position.X, 0.25f * length / 2 + position.Y, depth));
            shipVerts.Add(new Vector3(0 * width / 2 + position.X, 1f * length / 2 + position.Y, depth));
            shipVerts.Add(new Vector3(1f * width / 2 + position.X, 0.25f * length / 2 + position.Y, depth));
            shipVerts.Add(new Vector3(1f * width / 2 + position.X, -1f * length / 2 + position.Y, depth));
            shipVerts.Add(new Vector3(-1f * width / 2 + position.X, -1f * length / 2 + position.Y, depth));


            //mechanics = VehicleWithGearboxAspect.Create(this);
            physics = PhysicsAspect.Create(this);
            mass = MassAspect.Create(this);
            Thruster thruster1 = Thruster.Create(this, physics.Facing);
            Thruster thruster2 = Thruster.Create(this, physics.Facing);
            IList<Thruster> thrusters = new List<Thruster>(2);
            thrusters.Add(thruster1);
            thrusters.Add(thruster2);
            thrusterController = ThrusterController.Create(this, thrusters);
            graphics = GraphicsAspect.Create(this, shipVerts, 3, Color.White, Color.Red);
            bounds = BoundSetAspect.Create(this, shipVerts);
            //bounds.GetOuterContour();
            bounds.SetAttribute(Strings.CollisionDetectionSpeedType, Strings.CollisionDetectionSpeedTypeSlowOrStatic);

            rearCannon = Weapon.Create(this, Side.Rear);
            leftCannon = Weapon.Create(this, Side.Left);
            rightCannon = Weapon.Create(this, Side.Right);
            damage = DamageAspect.Create(this, 100);

            messageHandler.Handlers.Add(typeof(Kill), HandleKill);
            messageHandler.Handlers.Add(typeof(BoundSetCollision), HandleBoundSetCollision);
        }

        private bool HandleBoundSetCollision(object message)
        {
            BoundSetCollision boundSetCollision = (BoundSetCollision)message;

            if (boundSetCollision.Objects[0] == this.bounds && boundSetCollision.Objects[1].GetOwner().GetType() == typeof(Shell))
            {
                DamageManager.ApplyDamage(this, (IDamageable)boundSetCollision.Objects[1].GetOwner());
            }

            if (boundSetCollision.Objects[1] == this.bounds && boundSetCollision.Objects[0].GetOwner().GetType() == typeof(Shell))
            {
                DamageManager.ApplyDamage(this, (IDamageable)boundSetCollision.Objects[0].GetOwner());
            }
            //bool killed = this.damage.ApplyDamage(20);
            MessageDispatcher.Post(new TraceText(damage.CurrentDamage.ToString()));

            //if (killed)
            //    MessageDispatcher.Post(new Kill(this)); // возможно, это неправильно, что сам объект решает, мёртв он или нет, так как он сам вызывает и сам обрабатывает событие уничтожения, хотя событие вызова уничтожения ещё не вернуло управление

            return true;
        }

        private bool HandleKill(object message)
        {
            Kill kill = (Kill)message;
            if (kill.Target == this)
            {
                // убираем все аспекты, кроме графического, чтобы показать анимацию уничтожения
                //MessageDispatcher.Post(new DestroySelf(mechanics));
                MessageDispatcher.Post(new DestroySelf(thrusterController));
                MessageDispatcher.Post(new DestroySelf(physics));
                MessageDispatcher.Post(new DestroySelf(bounds));
                MessageDispatcher.Post(new DestroySelf(damage));
                MessageDispatcher.Post(new DestroySelf(leftCannon));
                MessageDispatcher.Post(new DestroySelf(rightCannon));
                MessageDispatcher.Post(new DestroySelf(rearCannon));

                timer = DestroyByTimerAspect.Create(this, new TimeSpan(0, 0, 0, 2, 0));
            }

            return true;
        }

        protected override void Cleanup()
        {
            base.Cleanup();

            MessageDispatcher.UnRegisterHandler(typeof(SetForwardAcceleration), this);
            MessageDispatcher.UnRegisterHandler(typeof(ApplyForce), this);
            MessageDispatcher.UnRegisterHandler(typeof(SetPosition), this);
            MessageDispatcher.UnRegisterHandler(typeof(SetSpeed), this);
            MessageDispatcher.UnRegisterHandler(typeof(SetAcceleration), this);
            MessageDispatcher.UnRegisterHandler(typeof(GetOwnerPosition), this);
            MessageDispatcher.UnRegisterHandler(typeof(InformPosition), this);
            MessageDispatcher.UnRegisterHandler(typeof(Shoot), this);
            MessageDispatcher.UnRegisterHandler(typeof(BoundSetCollision), this);
            MessageDispatcher.UnRegisterHandler(typeof(BoundSetNotCollision), this);
            MessageDispatcher.UnRegisterHandler(typeof(Ship), this);
            MessageDispatcher.UnRegisterHandler(typeof(Kill), this);
        }

        #region IDamageable Members

        public bool ApplyDamage(int amount)
        {
            return this.damage.ApplyDamage(amount);
        }

        #endregion
    }
}
