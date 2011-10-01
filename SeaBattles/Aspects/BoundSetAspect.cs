using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using SeaBattles.Messages;

namespace SeaBattles
{
    /// <summary>
    /// Более привычное название - Bounding volume.
    /// Содержит в себе другие BoundingAspect и служит грубым приближением их формы.
    /// Для BoundingAspect внутри одного BoundingSet проверка на столкновения не производится.
    /// </summary>
    internal class BoundSetAspect : Aspect
    {
        //protected LinkedList<CircleBoundsAspect> circles = new LinkedList<CircleBoundsAspect>();
        //protected LinkedList<TriangleBoundsAspect> triangles = new LinkedList<TriangleBoundsAspect>();
        //protected LinkedList<RectangleBoundsAspect> rectangles = new LinkedList<RectangleBoundsAspect>();
        protected LinkedList<BoundsAspect> bounds = new LinkedList<BoundsAspect>();
        protected bool positionSynced = false;
        protected Vector2 physicalPositionDisplacement;
        //protected float angle = 0;

        private float radius;
        private Vector2 position;

        protected Vector2 Position
        {
            get { return position; }
        }

        protected float Radius
        {
            get { return radius; }
        }

        public static BoundSetAspect Create(object owner, IList<Vector3> vertices)
        {
            BoundSetAspect aspect = new BoundSetAspect(owner, vertices);
            aspect.RegisterAllStuff();
            return aspect;
        }

        private BoundSetAspect(object owner, IList<Vector3> vertices)
            : base(owner)
        {
            if (vertices != null)
            {
                this.bounds = TriangulateFan(vertices);
                RecomputeBounds();
            }
            handlers.Add(typeof(SetPosition), new HandlerMethodDelegate(HandleUpdatePosition));
        }

        internal void AddBound(BoundsAspect bound)
        {
            //circles.AddLast(circle);
            bounds.AddLast(bound);
            RecomputeBounds();
        }

        //internal void AddBound(TriangleBoundsAspect triangle)
        //{
        //    triangles.AddLast(triangle);
        //    RecomputeBounds();
        //}

        //internal void AddBound(RectangleBoundsAspect rectangle)
        //{
        //    rectangles.AddLast(rectangle);
        //    RecomputeBounds();
        //}

        /// <summary>
        /// Пересчитывает положение и размер ограничивающей окружности.
        /// </summary>
        /// <remarks>http://cg.scs.carleton.ca/~zarrabi/files/papers/CCCG06/meb.pdf</remarks>
        protected void RecomputeBounds()
        {
            // 1 этап
            // Берём попарно все ограничивающие окружности BoundAspect-ов и
            // проводим через их центры прямые.
            // Затем находим пересечения этих прямых с окружностями (4 точки) и
            // берём самые удалённые друг от друга точки.
            // Скармливаем их алгоритму построения минимального покрывающего круга для точек.
            // 2 этап
            // Проводим от центра покрывающего круга до каждого центра окружностей прямые и
            // проецируем круги на эти прямые.
            // Корректиреум радиус покрывающего круга на расстояние до края самого удалённого круга.

            LinkedList<Vector2> circlesBorderPoints = new LinkedList<Vector2>();

            Vector2 start;
            Vector2 end;

            foreach (BoundsAspect bound1 in bounds)
            {
                foreach (BoundsAspect bound2 in bounds)
                {
                    if (bound1 == bound2)
                        continue;

                    start = bound1.Position;
                    end = bound2.Position;

                    float bound1CenterProjection = Misc.GetProjection(bound1.Position, start, end);
                    float radius1Length = bound1.Radius / (start - end).Length;
                    float bound2CenterProjection = Misc.GetProjection(bound2.Position, start, end);
                    float radius2Length = bound2.Radius / (start - end).Length;
                    float bound1MinProjection = bound1CenterProjection - radius1Length;
                    float bound1MaxProjection = bound1CenterProjection + radius1Length;
                    float bound2MinProjection = bound2CenterProjection - radius2Length;
                    float bound2MaxProjection = bound2CenterProjection + radius2Length;

                    float min = Math.Min(bound1MinProjection, bound2MinProjection);
                    circlesBorderPoints.AddLast(new Vector2(start.X + (end.X - start.X) * min, start.Y + (end.Y - start.Y) * min));

                    float max = Math.Max(bound1MaxProjection, bound2MaxProjection);
                    circlesBorderPoints.AddLast(new Vector2(start.X + (end.X - start.X) * max, start.Y + (end.Y - start.Y) * max));
                }
            }

            // нахождение минимального покрывающего круга 
            //
            Vector2 lastCenter = bounds.First.Value.Position;

            float lastRadius = 0;
            float delta;
            float distanceToNewPoint;
            Vector2 currentCenter = Vector2.Zero;
            float currentRadius = 0;

            foreach (Vector2 vector in circlesBorderPoints)
            {
                distanceToNewPoint = (vector - lastCenter).Length;
                delta = (distanceToNewPoint - lastRadius) / 2;

                currentCenter = lastCenter + delta / distanceToNewPoint * (vector - lastCenter);
                currentRadius = lastRadius + delta;

                lastCenter = currentCenter;
                lastRadius = currentRadius;
            }

            currentRadius = 0;
            // 2 этап
            foreach (BoundsAspect bound in bounds)
            {
                float radius = (lastCenter - bound.Position).Length + bound.Radius;
                if (radius > currentRadius)
                    currentRadius = radius;
            }

            this.position = currentCenter;
            this.radius = currentRadius;

            // устанаваливаем новые координаты родителя (BoundSetAspect) для всех детей (BoundsAspect)
            foreach (BoundsAspect bound in bounds)
            {
                bound.SetParentPosition(this.position);
            }
        }

        /// <summary>
        /// Триангулирует веер из вершин треугольников в отдельные треугольники.
        /// </summary>
        /// <param name="verticesFan"></param>
        /// <returns></returns>
        protected LinkedList<BoundsAspect> TriangulateFan(IList<Vector3> verticesFan)
        {
            LinkedList<BoundsAspect> triangles = new LinkedList<BoundsAspect>();
            Vector2 firstPoint = verticesFan[0].Xy;
            Vector2 secondPoint;
            Vector2 thirdPoint;
            for (int i = 2; i < verticesFan.Count - 1; i++)
            {
                secondPoint = verticesFan[i - 1].Xy;
                thirdPoint = verticesFan[i].Xy;
                triangles.AddLast(TriangleBoundsAspect.Create(this.owner, firstPoint, secondPoint, thirdPoint));
            }
            return triangles;
        }

        public bool IntersectsWith(BoundSetAspect boundSet)
        {
            if ((this.position - boundSet.position).LengthFast > this.radius + boundSet.radius)
                return false;

            foreach (BoundsAspect bound in this.bounds)
            {
                foreach (BoundsAspect bound2 in boundSet.bounds)
                {
                    if (bound != bound2)
                        if (bound.IntersectsWith(bound2))
                            return true;
                }
            }

            return false;
        }

        internal object GetOwner()
        {
            return this.owner;
        }

        private void HandleUpdatePosition(object message)
        {
            SetPosition setPosition = (SetPosition)message;
            if (this.owner == setPosition.Target)
            {
                if (!positionSynced)
                {
                    // вектор от физической позиции до геометрического центра покрывающего круга
                    this.physicalPositionDisplacement = this.position -
                                                            (setPosition.Position.Xy +
                                                            Vector2.Multiply(
                                                                Misc.RotateVector(setPosition.Velocity,
                                                                                    setPosition.Angle),
                                                                setPosition.Dt));

                    positionSynced = true;
                }

                // поворачиваем вектор от физической позиции до центра покрывающего круга на угол, на который повернулась физика
                Vector2 rotatedPosition = Misc.RotateVector(this.physicalPositionDisplacement, setPosition.Angle);
                // новая позиция будет считаться от физической позиции, смещённой на предыдущий повернутый вектор
                this.position = setPosition.Position.Xy + rotatedPosition;
                // угол поворота совпадает с углом повторота физики

                // теперь можно перемещать и поворачивать все части BoundSetAspect вокруг нового центра
                LinkedListNode<BoundsAspect> boundNode = this.bounds.First;
                BoundsAspect bound = null;
                while (boundNode != null)
                {
                    bound = boundNode.Value;
                    bound.UpdatePosition(this.position, setPosition.Angle);
                    boundNode = boundNode.Next;
                }
            }
        }
    }
}
