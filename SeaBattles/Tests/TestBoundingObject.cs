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
        //private VehicleWithGearboxAspect mechanics;
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

        public TestBoundingObject(BoundShape shape, PointF position, float length, float width, Color notColisionColor, Color collisionColor, float depth)
        {
            List<Vector3> verts = new List<Vector3>();
            BoundsAspect bound = null;

            switch (shape)
            {
                case BoundShape.Rectangle:
                    #region Rectangle
                    verts.Add(new Vector3(-1f * width / 2 + position.X, -1f * length / 2 + position.Y, 0));
                    verts.Add(new Vector3(-1f * width / 2 + position.X, 1f * length / 2 + position.Y, 0));
                    verts.Add(new Vector3(1f * width / 2 + position.X, 1f * length / 2 + position.Y, 0));
                    verts.Add(new Vector3(1f * width / 2 + position.X, -1f * length / 2 + position.Y, 0));
                    #endregion

                    bound = RectangleBoundsAspect.Create(this, width, length, new Vector2(0, 0), 0);
                    bounds = BoundSetAspect.Create(this, null);
                    bounds.AddBound(bound);
                    break;
                case BoundShape.Circle:
                    #region Circle
                    float radius = length;
                    int segments = 20;
                    verts = new List<Vector3>(segments + 1);

                    //float unitradius = (float)Math.Sqrt(8);
                    float angleStep = (float)(2 * Math.PI / segments);
                    for (int i = 0; i < segments; i++)
                    {
                        verts.Add(new Vector3((float)Math.Cos(angleStep * i) * radius, (float)Math.Sin(angleStep * i) * radius, depth));
                    }
                    verts.Add(new Vector3(radius, 0, depth));
                    #endregion

                    bound = CircleBoundsAspect.Create(this, new Vector2(0, 0), radius); //new CircleBoundsAspect(this, new Vector2(0, 0), radius);
                    bounds = BoundSetAspect.Create(this, null);
                    bounds.AddBound(bound);
                    break;
                case BoundShape.Triangle:
                    #region Triangle
                    verts = new List<Vector3>(3);
                    verts.Add(new Vector3(-width / 2, -length / 2, depth));
                    verts.Add(new Vector3(0, length / 2, depth));
                    verts.Add(new Vector3(width / 2, -length / 2, depth));
                    #endregion

                    bound = TriangleBoundsAspect.Create(this, verts[0].Xy, verts[1].Xy, verts[2].Xy);
                    bounds = BoundSetAspect.Create(this, null);
                    bounds.AddBound(bound);
                    break;

                case BoundShape.Ship:
                    // вершины формы корабля
                    verts.Add(new Vector3(-1f * width / 2 + position.X, -1f * length / 2 + position.Y, depth));
                    verts.Add(new Vector3(-1f * width / 2 + position.X, 0.25f * length / 2 + position.Y, depth));
                    verts.Add(new Vector3(0 * width / 2 + position.X, 1f * length / 2 + position.Y, depth));
                    verts.Add(new Vector3(1f * width / 2 + position.X, 0.25f * length / 2 + position.Y, depth));
                    verts.Add(new Vector3(1f * width / 2 + position.X, -1f * length / 2 + position.Y, depth));
                    verts.Add(new Vector3(-1f * width / 2 + position.X, -1f * length / 2 + position.Y, depth));
                    bounds = BoundSetAspect.Create(this, verts);
                    break;
                default:
                    break;
            }

            physics = PhysicsAspect.Create(this);
            graphics = GraphicsAspect.Create(this, verts, 1, notColisionColor, collisionColor);

            //bounds = new BoundSetAspect(this, verts);

            RegisterAllStuff();
        }
    }
}
