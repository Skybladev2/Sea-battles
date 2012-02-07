using System;
using System.Collections.Generic;
using OpenTK.Input;
using System.IO;
using System.Windows.Forms;

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
            //RunTests();

            using (MainGameWindow example = new MainGameWindow())
            {
                example.Run(30.0, 0.0);
            }
        }

        private static void RunTests()
        {
            //using (StreamWriter writer = new StreamWriter(Path.Combine(Application.StartupPath, "Controls.txt")))
            //{
            //    foreach (Key key in Enum.GetValues(typeof(Key)))
            //    {
            //        writer.WriteLine(Enum.GetName(typeof(Key), key) + "=");
            //    }
            //}
        }
    }
}
