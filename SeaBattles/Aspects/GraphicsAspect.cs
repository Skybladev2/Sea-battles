using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using OpenTK;
using SeaBattles.Messages;

namespace SeaBattles
{
    internal class GraphicsAspect : Aspect
    {
        internal List<Vector3> vertices = null;
        internal Vector3 translation = new Vector3(0, 0, 0);
        internal Vector3 rotationAxis = new Vector3(0, 0, 1);
        internal float rotationAngle = 0;
        internal float uniformScale = 1f;
        internal Vector3 scaling;
        internal float lineWidth = 3;
        internal Color color = Color.White;
        internal Color notCollisionColor = Color.White;
        internal Color collisionColor = Color.Red;
        internal Color killColor = Color.Black;

        public static GraphicsAspect Create(object owner, List<Vector3> vertices, float lineWidth, Color notCollisionColor, Color collisionColor)
        {
            GraphicsAspect aspect = new GraphicsAspect(owner, vertices, lineWidth, notCollisionColor, collisionColor);
            aspect.RegisterAllStuff();
            return aspect;
        }

        public static GraphicsAspect Create(object owner, List<Vector3> vertices, Vector3 position, float lineWidth, Color notCollisionColor, Color collisionColor)
        {
            GraphicsAspect aspect = new GraphicsAspect(owner, vertices, position, lineWidth, notCollisionColor, collisionColor);
            aspect.RegisterAllStuff();
            return aspect;
        }

        private GraphicsAspect(object owner, List<Vector3> vertices, float lineWidth, Color notCollisionColor, Color collisionColor)
            : base(owner)
        {
            this.vertices = vertices;
            this.scaling = new Vector3(uniformScale, uniformScale, 1);
            this.lineWidth = lineWidth;
            this.notCollisionColor = notCollisionColor;
            this.collisionColor = collisionColor;

            messageHandler.Handlers.Add(typeof(SetPosition), HandleUpdatePosition);
            messageHandler.Handlers.Add(typeof(BoundSetCollision), HandleCollision);
            messageHandler.Handlers.Add(typeof(BoundSetNotCollision), HandleNotCollision);
            messageHandler.Handlers.Add(typeof(Kill), HandleKill);
        }

        private GraphicsAspect(object owner, List<Vector3> vertices, Vector3 position, float lineWidth, Color notCollisionColor, Color collisionColor)
            : this(owner, vertices, lineWidth, notCollisionColor, collisionColor)
        {
            this.translation = position;
        }

        private bool HandleUpdatePosition(object message)
        {
            SetPosition setPosition = (SetPosition)message;
            if (this.owner == setPosition.Target)
            {
                this.rotationAngle = setPosition.Angle;
                //float scale = updatePosition.Velocity.LengthFast;
                //this.scaling = new Vector3(1, scale, 1);
                this.translation = setPosition.Position;

                //if (this.owner.GetType() == typeof(Shell))
                //    MessageDispatcher.Post(new TraceText(this.translation.ToString()));
            }

            return true;
        }

        private bool HandleCollision(object message)
        {
            BoundSetCollision collision = (BoundSetCollision)message;
            for (int i = 0; i < collision.Objects.Length; i++)
            {
                if (collision.Objects[i] != null && collision.Objects[i].GetOwner() == this.owner)
                {
                    this.color = collisionColor;
                }
            }

            return true;
        }

        private bool HandleNotCollision(object message)
        {
            BoundSetNotCollision collision = (BoundSetNotCollision)message;
            for (int i = 0; i < collision.Objects.Length; i++)
            {
                if (collision.Objects[i] != null && collision.Objects[i].GetOwner() == this.owner)
                {
                    this.color = notCollisionColor;
                }
            }

            return true;
        }

        private bool HandleKill(object message)
        {
            Kill kill = (Kill)message;
            if (kill.Target == this.owner)
            {
                this.color = killColor;
            }
            return true;
        }
    }
}
