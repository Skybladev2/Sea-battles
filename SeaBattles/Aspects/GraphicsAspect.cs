using System;
using System.Collections.Generic;
using System.Linq;
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
        internal float uniformScale = 0.5f;
        internal Vector3 scaling;

        public GraphicsAspect(object owner, List<Vector3> vertices) : base(owner)
        {
            this.vertices = vertices;
            this.scaling = new Vector3(uniformScale, uniformScale, 1);
            handlers.Add(typeof(SetPosition), new HandlerMethodDelegate(HandleUpdatePosition));
        }

        private void HandleUpdatePosition(object message)
        {
            SetPosition setPosition = (SetPosition)message;
            this.rotationAngle = setPosition.Angle;
            //float scale = updatePosition.Velocity.LengthFast;
            //this.scaling = new Vector3(1, scale, 1);
            this.translation = new Vector3(setPosition.Position.X, setPosition.Position.Y, 0);
        }
    }
}
