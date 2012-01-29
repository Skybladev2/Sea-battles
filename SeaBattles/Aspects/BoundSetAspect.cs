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
    internal class BoundSetAspect : Aspect, IReflectionAttributes
    {
        private bool IAmCircle { get; set; }
        private ReflectionAttributes ReflectionAttributes { get; set; }

        /// <summary>
        /// Список вершин по часовой стрелке, который описывает внешний контур объекта.
        /// Ега на каждом шаге вытягивают вдоль направления движения.
        /// </summary>
        protected IList<Vector2> outerContour = new List<Vector2>();

        /// <summary>
        /// Список вершин по часовой стрелке, который описывает внешний контур объекта на предыдущем шаге.
        /// </summary>
        protected IList<Vector2> previousContour = null;

        /// <summary>
        /// Неупорядоченное множество треугольников, которые получились путём вытягивания рёбер внешенго контура вдоль направления движения.
        /// </summary>
        protected LinkedList<Triangle<Vector2>> stretchedOutline = new LinkedList<Triangle<Vector2>>();
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

        private LinkedList<Triangle<Vector2>> Triangulated
        {
            get
            {
                LinkedList<Triangle<Vector2>> triangles = new LinkedList<Triangle<Vector2>>();
                foreach (BoundsAspect bound in this.bounds)
                {
                    triangles.AddLast(new Triangle<Vector2>(bound.Triangulate()));
                }

                return triangles;
            }
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
            ReflectionAttributes = new ReflectionAttributes();
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

        /// <summary>
        /// Пересчитывает положение и размер ограничивающей окружности.
        /// </summary>
        /// <remarks>http://cg.scs.carleton.ca/~zarrabi/files/papers/CCCG06/meb.pdf</remarks>
        protected void RecomputeBounds()
        {
            CheckSingleCicle();

            if (this.bounds.Count == 1 && this.bounds.Last.Value.GetType() == typeof(CircleBoundsAspect))
            {
                this.position = this.bounds.Last.Value.Position;
                this.radius = this.bounds.Last.Value.Radius;
            }
            else
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
            }
            // устанаваливаем новые координаты родителя (BoundSetAspect) для всех детей (BoundsAspect)
            foreach (BoundsAspect bound in bounds)
            {
                bound.SetParentPosition(this.position);
            }
        }

        /// <summary>
        /// Проверяет, состоит ли BoundSetAspect из единственного круга.
        /// </summary>
        private void CheckSingleCicle()
        {
            if (this.bounds.Count == 1 && typeof(CircleBoundsAspect) == bounds.First.Value.GetType())
                IAmCircle = true;
            else
                IAmCircle = false;
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
            // первый тик - стандартное определение столкновения
            if (previousContour == null)
            {
                return DiscreteCollisionDetection(boundSet);
            }
            else
            {
                string otherCdSpeedType = null;
                string thisCdSpeedType = null;
                BoundSetAspect fastBoundSet = null;
                BoundSetAspect slowBoundSet = null;

                if (boundSet.ReflectionAttributes.GetAttribute(Strings.CollisionDetectionSpeedType, out otherCdSpeedType)
                    &&
                    this.ReflectionAttributes.GetAttribute(Strings.CollisionDetectionSpeedType, out thisCdSpeedType))
                {
                    // медленный - быстрый
                    //
                    if (otherCdSpeedType == Strings.CollisionDetectionSpeedTypeFast
                        &&
                        thisCdSpeedType == Strings.CollisionDetectionSpeedTypeSlowOrStatic)
                    {
                        slowBoundSet = this;
                        fastBoundSet = boundSet;

                        return IntersectSlowAndFastBounds(slowBoundSet, fastBoundSet);
                    }

                    // быстрый - медленный 
                    //
                    if (otherCdSpeedType == Strings.CollisionDetectionSpeedTypeSlowOrStatic
                        &&
                        thisCdSpeedType == Strings.CollisionDetectionSpeedTypeFast)
                    {
                        slowBoundSet = boundSet;
                        fastBoundSet = this;

                        return IntersectSlowAndFastBounds(slowBoundSet, fastBoundSet);
                    }

                    // медленный - медленный
                    //
                    if (otherCdSpeedType == Strings.CollisionDetectionSpeedTypeSlowOrStatic
                        &&
                        thisCdSpeedType == Strings.CollisionDetectionSpeedTypeSlowOrStatic)
                    {
                        return DiscreteCollisionDetection(boundSet);
                    }

                    // быстрый - быстрый
                    //
                    return IntersectFastAndFastBounds(this, boundSet);

                }
                else // непонятно, какой тип - считаем обычными дискретными объектами
                {
                    return DiscreteCollisionDetection(boundSet);
                }

                #region Old
                //ICollection<ICollection<Vector2>> myNotExcludedParts = new LinkedList<ICollection<Vector2>>();
                //ICollection<ICollection<Vector2>> otherNotExcludedParts = new LinkedList<ICollection<Vector2>>();

                //foreach (Triangle<Vector2> stretchedOutlinePart in stretchedOutline)
                //{
                //    // каждый раз получаем массив контуров, из которых состоит разница нового и старого контуров
                //    // stretchedOutline - это набор треугольников, поэтому их разность с исходным контуром не должна, по идее, давать полигоны с дырками
                //    // поэтому каждую полученную часть можно триангулировать

                //    foreach (var triangle in this.Triangulated)
                //    {
                //        // возвращается коллекция полигонов
                //        ICollection<ICollection<Vector2>> differencePart =
                //            WeilerAtherton.Process(
                //                new CircularLinkedList<Vector2>(stretchedOutlinePart),
                //                new CircularLinkedList<Vector2>(previousContour, true),
                //                Operation.Difference);

                //        if (differencePart.Count != 0)
                //            foreach (var part in differencePart)
                //                myNotExcludedParts.Add(part);
                //    }
                //}

                //foreach (Triangle<Vector2> stretchedOutlinePart in boundSet.stretchedOutline)
                //{
                //    // каждый раз получаем массив контуров, из которых состоит разница нового и старого контуров
                //    // stretchedOutline - это набор треугольников, поэтому их разность с исходным контуром не должна, по идее, давать полигоны с дырками
                //    // поэтому каждую полученную часть можно триангулировать

                //    // возвращается коллекция полигонов
                //    ICollection<ICollection<Vector2>> differencePart =
                //        WeilerAtherton.Process(
                //            new CircularLinkedList<Vector2>(stretchedOutlinePart),
                //            new CircularLinkedList<Vector2>(boundSet.previousContour, true),
                //            Operation.Difference);

                //    if (differencePart.Count != 0)
                //        foreach (var part in differencePart)
                //            otherNotExcludedParts.Add(part);

                //}

                ////stretchedOutline - previousContour
                #endregion
            }

            return false;
        }

        protected bool DiscreteCollisionDetection(BoundSetAspect boundSet)
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

        protected bool IntersectSlowAndFastBounds(BoundSetAspect slowBoundSet, BoundSetAspect fastBoundSet)
        {
            // для круглых быстрых объектов границами является вытянутая часть + круг
            if (fastBoundSet.IAmCircle)
            {
                // сначала проверяем вытянутый след
                if (SlowWithFastCollisionDetection(slowBoundSet, fastBoundSet))
                    return true;

                // потом - новое положение круга
                foreach (BoundsAspect bound in slowBoundSet.bounds)
                {
                    foreach (BoundsAspect bound2 in fastBoundSet.bounds)
                    {
                        if (bound != bound2)
                            if (bound.IntersectsWith(bound2))
                                return true;
                    }
                }

                return false;
            }
            else
            {
                return SlowWithFastCollisionDetection(slowBoundSet, fastBoundSet);
            }
        }

        protected bool IntersectFastAndFastBounds(BoundSetAspect fastBoundSet1, BoundSetAspect fastBoundSet2)
        {
            // проверка вытянутых частей 
            //
            foreach (Triangle<Vector2> stretchedOutlinePart1 in fastBoundSet1.stretchedOutline)
            {
                foreach (Triangle<Vector2> stretchedOutlinePart2 in fastBoundSet2.stretchedOutline)
                {
                    if (Misc.TriangleIntersectsWithTriangle(stretchedOutlinePart1, stretchedOutlinePart2))
                        return true;
                }
            }

            // для круглых быстрых объектов границами является вытянутая часть + круг
            if (fastBoundSet1.IAmCircle)
            {
                // в данной ситуации у fastBoundSet1 проверяется только его круг со следом fastBoundSet2
                if (SlowWithFastCollisionDetection(fastBoundSet1, fastBoundSet2))
                    return true;
            }

            if (fastBoundSet2.IAmCircle)
            {
                // в данной ситуации у fastBoundSet2 проверяется только его круг со следом fastBoundSet1
                if (SlowWithFastCollisionDetection(fastBoundSet2, fastBoundSet1))
                    return true;
            }

            // прроверка столкновения кругов у обоих объектов
            //
            if (fastBoundSet1.IAmCircle && fastBoundSet2.IAmCircle)
            {
                // ! всегда считаем, что первый параметр - это this
                return DiscreteCollisionDetection(fastBoundSet2);
            }

            return false;
        }

        private static bool SlowWithFastCollisionDetection(BoundSetAspect slowBoundSet, BoundSetAspect fastBoundSet)
        {
            foreach (BoundsAspect bound in slowBoundSet.bounds)
            {
                foreach (Triangle<Vector2> stretchedOutlinePart in fastBoundSet.stretchedOutline)
                {
                    // где-то тут должна быть проверка на пересечение с самим собой, но это вроде бы не возможно
                    if (bound.IntersectsWith(stretchedOutlinePart))
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
                //bool a = setPosition.Target.GetType() == typeof(Shell);
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
                Vector2 prevPosition = this.position;

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

                // сначала нужно повернуть контур, а потом вытягивать
                RotateContour(prevPosition, setPosition.Angle);
                // вытягиваем контур по направлению движения
                StretchContour(rotatedPosition);

                this.previousContour = this.outerContour;
                // строим новый контур исходя из новых координат
                //BuildOuterContour();
            }
        }

        /// <summary>
        /// Поворачивает контур на заданный угол
        /// </summary>
        /// <param name="prevPosition">Центр BoundSetAspect до обновления положения.</param>
        /// <param name="angle">Угол, на которой будет повёрнута модель на текущем шаге.</param>
        private void RotateContour(Vector2 prevPosition, float angle)
        {
            Vector2[] centerToPoints = new Vector2[outerContour.Count];

            for (int i = 0; i < outerContour.Count; i++)
            {
                outerContour[i] = Misc.RotateVector(outerContour[i] - prevPosition, angle) + prevPosition;
            }
        }

        protected override void Cleanup()
        {
            base.Cleanup();

            // нужно удалить аспект из CollisionManager, если в момент уничтожения объекта
            // он с чем-то сталкивается
            CollisionManager.RemoveAspect(this);
        }

        /// <summary>
        /// Получает внешний контур объекта.
        /// </summary>
        [Obsolete("Алгоритму определения столкновений больше не требуется внушний контур объекта, достаточно вытянуть все его треугольники вдоль траектории")]
        internal void BuildOuterContour()
        {
            // в данном случае порядок имеет значение, но мы будем стравнивать элементы поодиночке,
            // так что можно использовать этот класс-контейнер
            List<Pair<Vector2>> contourEdges = new List<Pair<Vector2>>();

            foreach (BoundsAspect bound in this.bounds)
            {
                Vector2[] boundPoints = bound.Triangulate();
                for (int i = 0; i < 3; i++)
                {
                    Vector2 firstPoint = boundPoints[i];
                    Vector2 secondPoint = boundPoints[(i + 1) % 3];

                    bool edgeIsOuter = true;

                    foreach (BoundsAspect bound2 in this.bounds)
                    {
                        if (bound == bound2)
                            continue;

                        int firstPointCount = 0;
                        int secondPointCount = 0;

                        for (int j = 0; j < 3; j++)
                        {
                            if (bound2.Triangulate()[j] == firstPoint)
                                firstPointCount++;

                            if (bound2.Triangulate()[j] == secondPoint)
                                secondPointCount++;

                            if (firstPointCount > 0 && secondPointCount > 0)
                                edgeIsOuter = false;
                        }

                        // если обнаружили, что ребро не внешнее, то прекращаем дальнейшую проверку
                        if (!edgeIsOuter)
                            break;
                    }

                    if (edgeIsOuter)
                    {
                        // теперь нужно пометить вершины внешнего контура флагом, что они внешние
                        bound.SetVertexAsOuter(firstPoint);
                        bound.SetVertexAsOuter(secondPoint);

                        contourEdges.Add(new Pair<Vector2>(firstPoint, secondPoint));
                        //if (!contourPoints.Contains(firstPoint))
                        //contourPoints.Add(firstPoint);

                        //if (!contourPoints.Contains(secondPoint))
                        //contourPoints.Add(secondPoint);
                    }
                }
            }

            this.outerContour = TraverseContour(contourEdges);
        }
        /// <summary>
        /// На вход подаются неупорядоченные внешние рёбра.
        /// </summary>
        /// <param name="contourEdges"></param>
        /// <returns>Точки внешних рёбер, упорядоченные так, чтобы образовывать замкнутый контур.</returns>
        private IList<Vector2> TraverseContour(IList<Pair<Vector2>> contourEdges)
        {
            List<Vector2> contourPoints = new List<Vector2>(contourEdges.Count / 2);
            foreach (Pair<Vector2> edge1 in contourEdges)
            {
                foreach (Pair<Vector2> edge2 in contourEdges)
                {
                    // тут можно применять условие равенства, потому что 2 одинаковых ребра с противоположными направлениями встретиться не может
                    if (!edge1.Equals(edge2))
                    {
                        if (contourPoints.Count == 0)
                            contourPoints.Add(edge1.First);

                        if (edge1.Second == edge2.First)
                            contourPoints.Add(edge1.Second);
                    }
                }
            }

            //Console.WriteLine((contourPoints[0] - contourPoints[contourPoints.Count - 1]).LengthSquared);

            // последняя точка всегда должна быть равна первой, иначе контур неправильный
            // в этом месте должна стоять проверка, но из-за ошибок округления это сделать невозможно
            // поэтому просто надеемся на правильное построение модели в редакторе

            //if ((contourPoints[0] != contourPoints[contourPoints.Count - 1])
            //    throw new ArgumentException("Edges are in inconsistent state.", "contourEdges");
            //else
            contourPoints.RemoveAt(contourPoints.Count - 1);

            return contourPoints;
        }

        /// <summary>
        /// Вытягивает внешний контур объекта в указанном направлении.
        /// </summary>
        /// <remarks>Уникальная форма вытянутого контура 
        /// определяется исходным углом поворота BoundSetAspect и вектором переноса.</remarks>
        /// <param name="transfer">Вектор переноса.</param>
        internal void StretchContour(Vector2 transfer)
        {
            stretchedOutline.Clear();

            // для круга особая процедура
            if (IAmCircle)
            {
                CircleBoundsAspect circle = (CircleBoundsAspect)this.bounds.First.Value;

                Vector2 firstPoint = transfer.PerpendicularRight;
                firstPoint.Normalize();
                Vector2 firstTriangleFirstPoint = Vector2.Multiply(firstPoint, circle.Radius);

                Vector2 secondPoint = transfer.PerpendicularLeft;
                secondPoint.Normalize();
                Vector2 firstTriangleSecondPoint = Vector2.Multiply(secondPoint, circle.Radius);

                Vector2 firstTriangleThirdPoint = firstTriangleSecondPoint + transfer;

                Vector2 secondTriangleSecondPoint = firstTriangleFirstPoint + transfer;

                stretchedOutline.AddLast(new Triangle<Vector2>(firstTriangleFirstPoint, firstTriangleSecondPoint, firstTriangleThirdPoint));
                stretchedOutline.AddLast(new Triangle<Vector2>(firstTriangleThirdPoint, secondTriangleSecondPoint, firstTriangleFirstPoint));
            }
            else
                for (int i = 0; i < this.outerContour.Count; i++)
                {
                    Vector2 firstPoint = this.outerContour[i];
                    Vector2 secondPoint = this.outerContour[(i + 1) % this.outerContour.Count];

                    Vector2[] firstTriangle = null;
                    Vector2[] secondTriangle = null;

                    ExtrudeEdge(firstPoint, secondPoint, transfer, out firstTriangle, out secondTriangle);

                    if (firstTriangle != null)
                    {
                        stretchedOutline.AddLast(new Triangle<Vector2>(firstTriangle));
                        stretchedOutline.AddLast(new Triangle<Vector2>(secondTriangle));
                    }
                }
        }

        /// <summary>
        /// Получает 2 ориентированных треугольника из 2 точек ребра и вектора переноса.
        /// </summary>
        /// <param name="firstPoint"></param>
        /// <param name="secondPoint"></param>
        /// <param name="transfer"></param>
        /// <param name="firstTriangle"></param>
        /// <param name="secondTriangle"></param>
        /// <remarks>Так как треугольника всегда 2, то вместо тяжёлой коллекции используется 2 обычных массива.</remarks>
        private void ExtrudeEdge(Vector2 firstPoint,
                                        Vector2 secondPoint,
                                        Vector2 transfer,
                                        out Vector2[] firstTriangle,
                                        out Vector2[] secondTriangle)
        {

            // если вектор переноса коллинеарен ребру
            if (Vector3.Cross(new Vector3(firstPoint - secondPoint), new Vector3(transfer)) == Vector3.Zero)
            {
                firstTriangle = null;
                secondTriangle = null;
                return;
            }

            firstTriangle = new Vector2[3];
            firstTriangle[0] = firstPoint;
            firstTriangle[1] = secondPoint;
            firstTriangle[2] = secondPoint + transfer;

            secondTriangle = new Vector2[3];
            secondTriangle[0] = firstPoint;
            secondTriangle[1] = firstPoint + transfer;
            secondTriangle[2] = secondPoint + transfer;

            // Построение треугольников таким образом позволяет всегда получать 2 треугольника
            // с противоположными направлениями обхода.
            // Поэтому, если у одного из треугольников направление обхода изменено,
            // то для второго это точно не нужно делать.
            // Это не даёт какого-то выигрыша в скорости,
            // но позволяет сделать нагрузку более предсказуемой в этом месте.
            if (CorrectRotation(firstTriangle))
                return;

            CorrectRotation(secondTriangle);
        }

        private bool CorrectRotation(Vector2[] triangle)
        {
            if (Vector3.Cross(new Vector3(triangle[0]), new Vector3(triangle[1])).Z > 0)
            {
                Vector2 temp = triangle[0];
                triangle[0] = triangle[1];
                triangle[1] = temp;
                return true;
            }

            return false;
        }

        #region IReflectionAttributes Members

        public bool SetAttribute(string attributeName, string attributeValue)
        {
            return ReflectionAttributes.SetAttribute(attributeName, attributeValue);
        }

        public bool GetAttribute(string attributeName, out string attributeValue)
        {
            return ReflectionAttributes.GetAttribute(attributeName, out attributeValue);
        }

        public void RemoveAttribute(string attributeName)
        {
            ReflectionAttributes.RemoveAttribute(attributeName);
        }

        #endregion
    }
}
