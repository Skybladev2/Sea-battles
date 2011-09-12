using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;

namespace SeaBattles
{
    internal abstract class BoundsAspect : Aspect
    {
        public BoundsAspect(object owner)
            : base(owner)
        {

        }

        public abstract bool IntersectsWith(Vector2 point);
        public abstract bool IntersectsWith(CircleBoundsAspect circle);
        public abstract bool IntersectsWith(TriangleBoundsAspect triangle);
        internal abstract object GetOwner();

        internal abstract Vector2 Position { get; }
        internal abstract float Radius { get; }
    }
}
