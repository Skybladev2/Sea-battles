using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using OpenTK;

namespace SeaBattles.Tests
{
    internal class TestBoundingRectangle : Aspect
    {
        private PhysicsAspect physics;
        private VehicleWithGearboxAspect mechanics;
        private GraphicsAspect graphics;
        private RectangleBoundsAspect bounds;

        internal PhysicsAspect Physics
        {
            get { return physics; }
            //set { physics = value; }
        }

        public TestBoundingRectangle(PointF position, float height, float width)
        {
            List<Vector3> verts = new List<Vector3>();
            verts.Add(new Vector3(-1f * width / 2 + position.X, -1f * height / 2 + position.Y, 0));
            verts.Add(new Vector3(-1f * width / 2 + position.X, 1f * height / 2 + position.Y, 0));
            verts.Add(new Vector3(1f * width / 2 + position.X, 1f * height / 2 + position.Y, 0));
            verts.Add(new Vector3(1f * width / 2 + position.X, -1f * height / 2 + position.Y, 0));

            mechanics = new VehicleWithGearboxAspect(this);
            physics = new PhysicsAspect(this);
            graphics = new GraphicsAspect(this, verts, 1);
            bounds = new RectangleBoundsAspect(this, width, height, new Vector2(0, 0), 0);

            RegisterAllStuff();
        }
    }
}
