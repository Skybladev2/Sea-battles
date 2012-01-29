using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;

namespace SeaBattles
{
    /// <summary>
    /// Описание вершины геметрической модели объекта.
    /// </summary>
    internal class VertexDescription
    {
        /// <summary>
        /// Координаты вершины.
        /// </summary>
        internal Vector2 Position { get; set; }

        /// <summary>
        /// Принадлежит ли вершина внешней границе объекта.
        /// </summary>
        internal bool IsOuter { get; set; }
    }
}
