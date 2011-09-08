using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using OpenTK;

namespace SeaBattles.Tests
{
    internal class TestBoundingObject : Aspect
    {
        private PhysicsAspect physics;
        private VehicleWithGearboxAspect mechanics;
        private GraphicsAspect graphics;
        private BoundsAspect bounds;

        internal PhysicsAspect Physics
        {
            get { return physics; }
            //set { physics = value; }
        }

        internal BoundsAspect Bounds
        {
            get { return bounds; }
        }

        public TestBoundingObject(PointF position, float height, float width, Color notColisionColor, Color collisionColor, float depth)
        {
            #region Rectangle
            //List<Vector3> verts = new List<Vector3>();
            //verts.Add(new Vector3(-1f * width / 2 + position.X, -1f * height / 2 + position.Y, 0));
            //verts.Add(new Vector3(-1f * width / 2 + position.X, 1f * height / 2 + position.Y, 0));
            //verts.Add(new Vector3(1f * width / 2 + position.X, 1f * height / 2 + position.Y, 0));
            //verts.Add(new Vector3(1f * width / 2 + position.X, -1f * height / 2 + position.Y, 0));
            #endregion

            #region Circle
            //float radius = height;
            //int segments = 20;
            //List<Vector3> vertices = new List<Vector3>(segments + 1);

            ////float unitradius = (float)Math.Sqrt(8);
            //float angleStep = (float)(2 * Math.PI / segments);
            //for (int i = 0; i < segments; i++)
            //{
            //    vertices.Add(new Vector3((float)Math.Cos(angleStep * i) * radius, (float)Math.Sin(angleStep * i) * radius, 0));
            //}
            //vertices.Add(new Vector3(radius, 0, 0));
            #endregion

            #region Triangle
            List<Vector3> vertices = new List<Vector3>(3);
            vertices.Add(new Vector3(-width / 2, -height / 2, depth));
            vertices.Add(new Vector3(0, height / 2, depth));
            vertices.Add(new Vector3(width / 2, -height / 2, depth));
            #endregion

            mechanics = new VehicleWithGearboxAspect(this);
            physics = new PhysicsAspect(this);
            graphics = new GraphicsAspect(this, vertices, 1, notColisionColor, collisionColor);
            //bounds = new RectangleBoundsAspect(this, width, height, new Vector2(0, 0), 0);
            //bounds = new CircleBoundsAspect(this, new Vector2(0, 0), radius);
            bounds = new TriangleBoundsAspect(this, vertices[0].Xy, vertices[1].Xy, vertices[2].Xy);

            RegisterAllStuff();
        }
    }
}
