using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeaBattles.Messages;
using OpenTK;

namespace SeaBattles
{
    internal class Weapon : Aspect
    {
        //private PhysicsAspect physics;
        //private GraphicsAspect graphics;
        private ShootAspect shooter;

        /// <summary>
        /// С какого борта корабля расположена пушка.
        /// </summary>
        private Side shipSide;

        /// <summary>
        /// Отправили сообщение о том, что нужны текущие координаты для выстрела.
        /// </summary>
        private bool waitForWeaponCoords = false;

        public Weapon(object owner, Side weaponSide) : base(owner)
        {
            this.shipSide = weaponSide;
            //physics = new PhysicsAspect();
            //graphics = new GraphicsAspect(shipVerts);
            shooter = new ShootAspect(this);
            //handlersMap.Add(typeof(ButtonDown), physics);
            //handlersMap.Add(typeof(UpdatePosition), graphics);
            //handlersMap.Add(typeof(Shoot), shooter);
            handlers.Add(typeof(ButtonDown), HandleButtonDown);
            handlers.Add(typeof(InformPosition), HandleInformPosition);

            AddHandlerToMap(typeof(Shoot), shooter);
        }

        private void HandleInformPosition(object message)
        {
            InformPosition position = (InformPosition)message;

            // если пришла информация о чужом объекте - выходим
            if (position.InformedObject != this.owner)
                return;

            if (waitForWeaponCoords)
            {
                waitForWeaponCoords = false;

                // вычисляем направление выстрела
                // оно зависит от борта, с которого производится выстрел

                Vector2 temp = position.Velocity;
                float angle = 0;

                // в зависимости от борта поворачиваем вектор скорости корабля -
                // получаем направление, куда смотрит пушка
                switch (this.shipSide)
                {
                    case Side.Front:
                        break;
                    case Side.Rear:
                        angle = 180;
                        break;
                    case Side.Left:
                        angle = -90;
                        break;
                    case Side.Right:
                        angle = 90;
                        break;
                    default:
                        break;
                }

                // поворачиваем вектор
                float newX = temp.X * (float)Math.Cos(angle / 180 * Math.PI) - temp.Y * (float)Math.Sin(angle / 180 * Math.PI);
                float newY = temp.X * (float)Math.Sin(angle / 180 * Math.PI) + temp.Y * (float)Math.Cos(angle / 180 * Math.PI);

                // один из векторов - его мы будем удлинять на силу выстрела
                Vector3 weaponFacing = new Vector3(newX, newY, 0);
                //weaponFacing.Normalize();

                Vector2 lastFrameWeaponVelocity2D = Vector2.Divide(position.Position - position.PrevPosition, position.LastDT);
                Vector3 lastFrameWeaponVelocity = new Vector3(lastFrameWeaponVelocity2D);

                MessageDispatcher.Post(new Shoot(new Vector3(position.Position), weaponFacing, lastFrameWeaponVelocity));
                //Shell shell = new Shell(position.Position, weaponFacing, new Vector3(lastFrameWeaponVelocity), 1);
            }
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
                    break;
                case InputVirtualKey.AxisDown:
                    break;
                case InputVirtualKey.Action1:
                    ShootSide(Side.Left);
                    break;
                case InputVirtualKey.Action2:
                    ShootSide(Side.Right);
                    break;
                case InputVirtualKey.Action3:
                    ShootSide(Side.Rear);
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

        /// <summary>
        /// Сверху мы получаем сообщение, что нужно стрелять, но ещё не знаем, нужное ли оружие приняло сообщение.
        /// Каждая сторона корабля с пушками стреляет по отдельной кнопке.
        /// </summary>
        private void ShootSide(Side requestedSide)
        {
            if (this.shipSide == requestedSide)
                PerformShoot();
        }

        private void PerformShoot()
        {
            // отправляем сообщение об определении текущего местоположения
            // мы знаем, что принадлежим какому-то объекту - запрашиваем его координаты
            // на их основе вычисляем свои координаты
            // в данном случае мы должны принадлежать объекту Ship
            waitForWeaponCoords = true;
            MessageDispatcher.Post(new GetOwnerPosition(this, this.owner));
        }

        //#region IMessageHandler Members

        //public void ProcessMessage(object message)
        //{
        //    Type type = message.GetType();

        //    // так как данный класс выступает и как аспект, и как агрегация аспектов,
        //    // то у него 2 списка обработчиков -
        //    // собственные методы и объекты-аспекты.
        //    // Сначала сообщения проходят через собственные методы
        //    if (handlers.ContainsKey(type))
        //        handlers[type](message);

        //    if (handlersMap.ContainsKey(type))
        //        handlersMap[type].ProcessMessage(message);
        //}

        //#endregion
    }
}
