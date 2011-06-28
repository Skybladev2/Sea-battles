using System;
using System.Collections.Generic;

namespace SeaBattles
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (MainGameWindow example = new MainGameWindow())
            {
                example.Run(30.0, 0.0);
            }
        }
    }
}
