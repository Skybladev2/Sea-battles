using System;
using System.Collections.Generic;
using System.Text;
using SeaBattles.Messages;
using OpenTK;

namespace SeaBattles
{
    /// <summary>
    /// Штурвал. Поворачивает объект (корабль) вокруг оси, т.е. прикладывает угловую скорость.
    /// Влияние штурвала на корабль не учитывает сопротивление воды, поэтому не является физически обоснованным.
    /// </summary>
    internal class SteeringWheel : Aspect
    {
        /// <summary>
        /// Текущий угол поворота штурвала (угловая скорость в градусах/сек)
        /// </summary>
        protected float currentAngleSpeed = 0;

        /// <summary>
        /// На сколько градусов/сек поворачивается корабль против часовой стрелки, когда штурвал выкручен до упора.
        /// </summary>
        protected float maxAngleSpeed = 10;

        // так как при _не_ удерживаемые кнопки не обрабатываются, аспекту нужно самостоятельно запрашивать состояние кнопок
        // inputLayer.QueryButtonHold(turnLeft)

        private SteeringWheel(object owner) : base(owner)
        {
            // при движении физического аспекта _поворачиваем_ и тяговый двигатель
            //messageHandler.Handlers.Add(typeof(SetPosition), HandleUpdatePosition);
        }

        public static SteeringWheel Create(object owner)
        {
            SteeringWheel aspect = new SteeringWheel(owner);
            aspect.RegisterAllStuff();
            return aspect;
        }
    }
}
