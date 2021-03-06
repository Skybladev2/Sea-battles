﻿using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Input;
using OpenTK;

namespace SeaBattles.Messages
{
    /// <summary>
    /// Сообщает о координатах и скорости объекта.
    /// В отличие от SetPosition не требует от получателя по обновления положения объекта.
    /// </summary>
    internal class InformPosition
    {
        /// <summary>
        /// Кто получает данное сообщение.
        /// </summary>
        private object target = null;

        /// <summary>
        /// Объект, данные о котором передаются.
        /// </summary>
        private object informedObject = null;

        private Vector2 facing;
        private Vector2 velocity;
        private Vector3 position;
        private Vector3 prevPosition;
        private float lastDT;

        /// <summary>
        /// Объект, данные о котором передаются.
        /// </summary>
        public object InformedObject
        {
            get { return informedObject; }
        }

        /// <summary>
        /// Кто получает данное сообщение.
        /// </summary>
        public object Target
        {
            get { return target; }
        }

        public Vector3 Position
        {
            get { return position; }
        }

        public Vector2 Facing
        {
            get { return facing; }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
        }

        public Vector3 PrevPosition
        {
            get
            {
                return this.prevPosition;
            }
        }

        public float LastDT
        {
            get
            {
                return this.lastDT;
            }
        }

        public InformPosition(object target,
                            object informedObject,
                            Vector2 facing,
                            Vector2 velocity,
                            Vector3 position,
                            Vector3 prevPosition,
                            float lastDT)
        {
            this.target = target;
            this.informedObject = informedObject;
            this.facing = facing;
            this.velocity = velocity;
            this.position = position;
            this.prevPosition = prevPosition;
            this.lastDT = lastDT; 
        }
    }
}
