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
        internal abstract object GetOwner();
    }
}
