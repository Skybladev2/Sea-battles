using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenTK.Platform;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using SeaBattles.Messages;
using System.IO;

namespace SeaBattles
{
    public class MainGameWindow : GameWindow, IMessageHandler
    {
        #region Performance and debug
        //private int updateCount = 0;
        //private List<double> dts = new List<double>(15000);
        #endregion

        bool viewport_changed = true;
        int viewport_width, viewport_height;
        bool exit = false;
        Thread rendering_thread;
        object update_lock = new object();

        Random rand = new Random();

        private InputLayer input = null;
        private List<GraphicsAspect> graphicsAspects = new List<GraphicsAspect>();
        private Ship ship = null;
        private Ship anotherShip = null;
        private Dictionary<Type, HandlerMethodDelegate> handlers = new Dictionary<Type, HandlerMethodDelegate>();

        public MainGameWindow()
            : base(800, 600)
        {
            Resize += delegate(object sender, EventArgs e)
            {
                // Note that we cannot call any OpenGL methods directly. What we can do is set
                // a flag and respond to it from the rendering thread.
                lock (update_lock)
                {
                    viewport_changed = true;
                    viewport_width = Width;
                    viewport_height = Height;
                }
            };

            handlers.Add(typeof(TraceText), new HandlerMethodDelegate(WriteTitle));
            input = new InputLayer(this);

            //shipVerts.Add(new Vector3(0, 0, -1.1f)); // низ
            //shipVerts.Add(new Vector3(0, 0.1f, -1.09f)); // снизу вверх
            //shipVerts.Add(new Vector3(-0.02f, 0.08f, -1.08f)); // сверху в левый угол стрелки
            //shipVerts.Add(new Vector3(0.02f, 0.08f, -1.07f)); // из левого угла стрелки в правый
            //shipVerts.Add(new Vector3(0, 0.1f, -1.06f)); // из правого угла стрелки вверх

            ship = new Ship(new PointF(0, 0));
            //anotherShip = new Ship(new PointF(0.5f, 0));

            //ship.Graphics = new GraphicsAspect(shipVerts);
            graphicsAspects.Add(ship.Graphics);
            //graphicsAspects.Add(anotherShip.Graphics);

            //только корабль игрока подписывается на приём пользовательского ввода
            MessageDispatcher.RegisterHandler(typeof(ButtonDown), ship);
            MessageDispatcher.RegisterHandler(typeof(SetPosition), ship);
            //MessageDispatcher.RegisterHandler(typeof(SetPosition), anotherShip);
            MessageDispatcher.RegisterHandler(typeof(TraceText), this);
        }

        #region OnLoad

        /// <summary>
        /// Setup OpenGL and load resources here.
        /// </summary>
        /// <param name="e">Not used.</param>
        protected override void OnLoad(EventArgs e)
        {
            Context.MakeCurrent(null); // Release the OpenGL context so it can be used on the new thread.

            rendering_thread = new Thread(RenderLoop);
            rendering_thread.IsBackground = true;
            rendering_thread.Start();
        }

        #endregion

        #region OnUnload

        /// <summary>
        /// Release resources here.
        /// </summary>
        /// <param name="e">Not used.</param>
        protected override void OnUnload(EventArgs e)
        {
            exit = true; // Set a flag that the rendering thread should stop running.
            rendering_thread.Join();

            base.OnUnload(e);
        }

        #endregion

        #region OnUpdateFrame

        /// <summary>
        /// Add your game logic here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            this.InputDriver.Poll();
        }

        #endregion

        #region OnRenderFrame

        /// <summary>
        /// Ignored. All rendering is performed on our own rendering function.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            // Nothing to do. Release the CPU to other threads.
            //Thread.Sleep(1);
        }

        #endregion

        #region RenderLoop

        void RenderLoop()
        {
            MakeCurrent(); // The context now belongs to this thread. No other thread may use it!

            VSync = VSyncMode.On;

            // Since we don't use OpenTK's timing mechanism, we need to keep time ourselves;
            Stopwatch render_watch = new Stopwatch();
            Stopwatch update_watch = new Stopwatch();
            update_watch.Start();
            render_watch.Start();

            GL.ClearColor(Color.MidnightBlue);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.PointSmooth);
            GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            //GL.BlendFunc((BlendingFactorSrc)0x0300, BlendingFactorDest.OneMinusSrcColor);
            GL.Enable(EnableCap.LineSmooth);
            GL.PointSize(16);

            while (!exit)
            {
                Update(update_watch.Elapsed.TotalSeconds);
                update_watch.Reset();
                update_watch.Start();

                Render(render_watch.Elapsed.TotalSeconds);
                render_watch.Reset(); //  Stopwatch may be inaccurate over larger intervals.
                render_watch.Start(); // Plus, timekeeping is easier if we always start counting from 0.

                SwapBuffers();
            }

            Context.MakeCurrent(null);

            //foreach (double dt in dts)
            //{
            //    File.AppendAllText(Path.Combine(
            //        Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            //        "SeaBattlesLog.txt"), dt.ToString() + Environment.NewLine);    
            //}
            
            //Console.WriteLine("Update count " + updateCount);
        }

        #endregion

        #region Update

        void Update(double dt)
        {
            lock (update_lock)
            {
                //dts.Add(dt);

                if (input.Pressed(InputVirtualKey.AxisLeft))
                    ship.Physics.UpdateRotation(InputVirtualKey.AxisLeft, dt);

                if (input.Pressed(InputVirtualKey.AxisRight))
                    ship.Physics.UpdateRotation(InputVirtualKey.AxisRight, dt);

                ship.Physics.Update(dt);
            }
        }

        #endregion

        #region Render

        /// <summary>
        /// This is our main rendering function, which executes on the rendering thread.
        /// </summary>
        public void Render(double time)
        {
            lock (update_lock)
            {
                if (viewport_changed)
                {
                    GL.Viewport(0, 0, viewport_width, viewport_height);
                    viewport_changed = false;
                }
            }

            Matrix4 perspective =
                Matrix4.CreatePerspectiveFieldOfView((float)(Math.PI * 45f / 180f), viewport_width / (float)viewport_height, 0.1f, 100f);
            //Matrix4.CreateOrthographic(2, 2, -1, 1);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspective);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.LineWidth(4);

            foreach (GraphicsAspect g in graphicsAspects)
            {
                GL.PushMatrix();
                GL.LoadIdentity();
                GL.Translate(g.translation);
                GL.Rotate(g.rotationAngle, g.rotationAxis);
                GL.Scale(g.scaling);

                GL.Begin(BeginMode.LineStrip);
                //GL.Begin(BeginMode.Lines);

                foreach (Vector3 vertex in g.vertices)
                {
                    GL.Vertex3(vertex);
                }

                GL.End();

                GL.PopMatrix();
            }

            //GL.Begin(BeginMode.LineLoop);

            //GL.Vertex3(-0.1, -0.3, -1); // back left
            //GL.Vertex3(-0.1, 0.1, -1); // front left
            //GL.Vertex3(0, 0.3, -1); // font
            //GL.Vertex3(0.1, 0.1, -1); // front right
            //GL.Vertex3(0.1, -0.3, -1); // back right

            //GL.End();

            GL.Begin(BeginMode.Points);
            GL.Vertex3(0, 0, -1);
            GL.End();
        }

        #endregion

        #region IMessageHandler Members

        public void ProcessMessage(object message)
        {
            Type type = message.GetType();
            handlers[type](message);
        }

        #endregion

        private void WriteTitle(object message)
        {
            TraceText text = (TraceText)message;
            this.Title = text.Text;
        }
    }
}
