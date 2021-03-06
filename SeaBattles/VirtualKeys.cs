﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattles
{
    /// <summary>
    /// Универсальный enum для перевода нажатия кнопок на клавиатуре и джойстике к одинаковым параметрам.
    /// </summary>
    internal enum InputVirtualKey
    {
        Unknown,
        AxisLeft,
        AxisRight,
        AxisUp,
        AxisDown,
        /// <summary>
        /// Shoot left
        /// </summary>
        Action1,
        /// <summary>
        /// Shoot right
        /// </summary>
        Action2,
        /// <summary>
        /// Shoot back
        /// </summary>
        Action3,
        /// <summary>
        /// Execute
        /// </summary>
        Action4,
        Action5,
        Action6,
        /// <summary>
        /// Zoom in
        /// </summary>
        Action7,
        /// <summary>
        /// Zoom out
        /// </summary>
        Action8,
        Action9,
        Action10,
        Action11,
        Action12,
        Action13,
        Action14,
        Action15,
        Action16,
        /// <summary>
        /// Pause/Escape
        /// </summary>
        Action17
    }
}
