using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;

namespace SeaBattles
{
    internal abstract class BoundsAspect : Aspect
    {
        protected Vector2 displacementFromParent;

        public BoundsAspect(object owner)
            : base(owner)
        {

        }

        public abstract bool IntersectsWith(Vector2 point);
        public abstract bool IntersectsWith(CircleBoundsAspect circle);
        public abstract bool IntersectsWith(TriangleBoundsAspect triangle);
        public abstract bool IntersectsWith(BoundsAspect bound);
        internal abstract object GetOwner();

        internal abstract Vector2 Position { get; }
        internal abstract float Radius { get; }

        internal void SetParentPosition(Vector2 parentPosition)
        {
            this.displacementFromParent = this.Position - parentPosition;
        }

        internal abstract void UpdatePosition(Vector2 parentPosition, float angle);
    }
}
