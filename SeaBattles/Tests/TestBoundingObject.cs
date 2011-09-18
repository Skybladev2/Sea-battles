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
        //private BoundsAspect bounds;
        private BoundSetAspect bounds;

        internal PhysicsAspect Physics
        {
            get { return physics; }
            //set { physics = value; }
        }

        internal BoundSetAspect Bounds
        {
            get { return bounds; }
        }

        public TestBoundingObject(PointF position, float length, float width, Color notColisionColor, Color collisionColor, float depth)
        {
            List<Vector3> vertices = new List<Vector3>();
            vertices.Add(new Vector3(-1f * width / 2 + position.X, -1f * length / 2 + position.Y, depth));
            vertices.Add(new Vector3(-1f * width / 2 + position.X, 0.25f * length / 2 + position.Y, depth));
            vertices.Add(new Vector3(0 * width / 2 + position.X, 1f * length / 2 + position.Y, depth));
            vertices.Add(new Vector3(1f * width / 2 + position.X, 0.25f * length / 2 + position.Y, depth));
            vertices.Add(new Vector3(1f * width / 2 + position.X, -1f * length / 2 + position.Y, depth));
            vertices.Add(new Vector3(-1f * width / 2 + position.X, -1f * length / 2 + position.Y, depth));

            //switch (shape)
            //{
            //    case BoundShape.Rectangle:
            //        #region Rectangle
            //        List<Vector3> verts = new List<Vector3>();
            //        verts.Add(new Vector3(-1f * width / 2 + position.X, -1f * height / 2 + position.Y, 0));
            //        verts.Add(new Vector3(-1f * width / 2 + position.X, 1f * height / 2 + position.Y, 0));
            //        verts.Add(new Vector3(1f * width / 2 + position.X, 1f * height / 2 + position.Y, 0));
            //        verts.Add(new Vector3(1f * width / 2 + position.X, -1f * height / 2 + position.Y, 0));
            //        #endregion

            //        bounds = new RectangleBoundsAspect(this, width, height, new Vector2(0, 0), 0);
            //        break;
            //    case BoundShape.Circle:
            //        #region Circle
            //        float radius = height;
            //        int segments = 20;
            //        vertices = new List<Vector3>(segments + 1);

            //        //float unitradius = (float)Math.Sqrt(8);
            //        float angleStep = (float)(2 * Math.PI / segments);
            //        for (int i = 0; i < segments; i++)
            //        {
            //            vertices.Add(new Vector3((float)Math.Cos(angleStep * i) * radius, (float)Math.Sin(angleStep * i) * radius, depth));
            //        }
            //        vertices.Add(new Vector3(radius, 0, depth));
            //        #endregion

            //        bounds = new CircleBoundsAspect(this, new Vector2(0, 0), radius);
            //        break;
            //    case BoundShape.Triangle:
            //        #region Triangle
            //        vertices = new List<Vector3>(3);
            //        vertices.Add(new Vector3(-width / 2, -height / 2, depth));
            //        vertices.Add(new Vector3(0, height / 2, depth));
            //        vertices.Add(new Vector3(width / 2, -height / 2, depth));
            //        #endregion

            //        bounds = new TriangleBoundsAspect(this, vertices[0].Xy, vertices[1].Xy, vertices[2].Xy);
            //        break;
            //    default:
            //        break;
            //}

            mechanics = new VehicleWithGearboxAspect(this);
            physics = new PhysicsAspect(this);
            graphics = new GraphicsAspect(this, vertices, 1, notColisionColor, collisionColor);
            bounds = new BoundSetAspect(this, vertices);

            RegisterAllStuff();
        }
    }
}
