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
using SeaBattles.Tests;

namespace SeaBattles
{
    public class MainGameWindow : GameWindow, IMessageHandler
    {
        #region Performance and debug
        private System.Threading.Timer timer = null;
        #endregion

        bool viewport_changed = true;
        int viewport_width, viewport_height;
        bool exit = false;
        Thread rendering_thread;
        object update_lock = new object();
        private Camera mainCamera = null;

        Random rand = new Random();

        private InputLayer input = null;
        //---------------------------------
        //private List<GraphicsAspect> graphicsAspects = new List<GraphicsAspect>();
        private Ship ship = null;
        private Ship ship2 = null;
        private TestBoundingObject box = null;
        private TestBoundingObject box2 = null;
        private Dictionary<Type, HandlerMethodDelegate> handlers = new Dictionary<Type, HandlerMethodDelegate>();

        public MainGameWindow()
            : base(800, 600, GraphicsMode.Default, "Sea battles")
        {
            Resize += delegate(object sender, EventArgs e)
            {
                // Note that we cannot call any OpenGL methods directly. What we can do is set
                // a flag and respond to it from the rendering thread.
                lock (update_lock)
                {
                    viewport_changed = true;
                    viewport_width = this.ClientRectangle.Width;
                    viewport_height = this.ClientRectangle.Height;
                }
            };

            handlers.Add(typeof(TraceText), WriteTitle);
            handlers.Add(typeof(ButtonUp), HandleButtonUp);
            mainCamera = new Camera(0, 0, 1, 800, 600);
            input = new InputLayer(this);

            ship = Ship.Create(new PointF(0, 0), 40, 10);
            ship2 = Ship.Create(new PointF(0, 0), 40, 1);

            ship.Name = "player";
            //box = new TestBoundingObject(BoundShape.Ship, new PointF(0, 0), 10, 20, Color.Green, Color.Red, 0);
            //box2 = new TestBoundingObject(BoundShape.Circle, new PointF(0, 0), 20, 40, Color.White, Color.Black, -0.5f);

            //MessageDispatcher.RegisterHandler(typeof(ButtonDown), box);
            //MessageDispatcher.RegisterHandler(typeof(SetPosition), box);
            //MessageDispatcher.RegisterHandler(typeof(SetSpeed), box);
            //MessageDispatcher.RegisterHandler(typeof(GetOwnerPosition), box);
            //MessageDispatcher.RegisterHandler(typeof(InformPosition), box);
            //MessageDispatcher.RegisterHandler(typeof(BoundSetCollision), box);
            //MessageDispatcher.RegisterHandler(typeof(BoundSetNotCollision), box);

            //---------------------------------------------------
            //graphicsAspects.Add(ship.Graphics);

            //только корабль игрока подписывается на приём пользовательского ввода
            MessageDispatcher.RegisterHandler(typeof(ButtonDown), ship);
            MessageDispatcher.RegisterHandler(typeof(ButtonHold), ship);

            //MessageDispatcher.RegisterHandler(typeof(SetPosition), ship);
            //MessageDispatcher.RegisterHandler(typeof(SetSpeed), ship);
            //// нужно для определения координат и скорости корабля в момент выстрела
            //// в данном случае owner-ом является ship
            //MessageDispatcher.RegisterHandler(typeof(GetOwnerPosition), ship);
            //MessageDispatcher.RegisterHandler(typeof(InformPosition), ship);
            //MessageDispatcher.RegisterHandler(typeof(Shoot), ship);
            //MessageDispatcher.RegisterHandler(typeof(BoundSetCollision), ship);
            //MessageDispatcher.RegisterHandler(typeof(BoundSetNotCollision), ship);
            ////MessageDispatcher.RegisterHandler(typeof(SetPosition), anotherShip);

            MessageDispatcher.RegisterHandler(typeof(TraceText), this);
            MessageDispatcher.RegisterHandler(typeof(ButtonUp), this);

            //timer = new System.Threading.Timer(new TimerCallback(timer_Tick), null, 1000, 1000);
        }

        void timer_Tick(object sender)
        {
            //MessageDispatcher.Post(new TraceText(MessageDispatcher.GetHandlersCount(typeof(DestroyChildrenOf)).ToString()));
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
            Thread.Sleep(1);
        }

        #endregion

        #region RenderLoop

        void RenderLoop()
        {
            MakeCurrent(); // The context now belongs to this thread. No other thread may use it!

            VSync = VSyncMode.On;
            double lastUpdateDt = 0;
            double lastRenderDt = 0;

            // Since we don't use OpenTK's timing mechanism, we need to keep time ourselves;
            Stopwatch render_watch = new Stopwatch();
            Stopwatch update_watch = new Stopwatch();

            // таймер для измерения времени обновления
            Stopwatch updateTime = new Stopwatch();
            // таймер для измерения времени рендеринга
            Stopwatch renderTime = new Stopwatch();
            update_watch.Start();
            render_watch.Start();

            //GL.Enable(EnableCap.PolygonOffsetFill);
            //GL.PolygonOffset(1, 1);
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            GL.ClearColor(Color.MidnightBlue);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.AlphaTest);
            //GL.Enable(EnableCap.PointSmooth);
            //GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);
            GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            //GL.BlendFunc(BlendingFactorSrc.SrcAlphaSaturate, BlendingFactorDest.One);
            //GL.Enable(EnableCap.LineSmooth);
            GL.Enable(EnableCap.PolygonSmooth);
            GL.PointSize(1);
            GL.DepthFunc(DepthFunction.Lequal);

            bool result = GL.IsEnabled(EnableCap.DepthTest);
            Console.WriteLine("Depth test is {0}", result);

            float res = 0;
            GL.GetFloat(GetPName.DepthClearValue, out res);
            Console.WriteLine("Depth Clear Value is {0}", res);

            GL.GetBoolean(GetPName.DepthWritemask, out result);
            Console.WriteLine("Depth mask is {0}", result);

            int resint = 0;
            GL.GetInteger(GetPName.DepthFunc, out resint);
            Console.WriteLine("Depth comparsion is {0}", (DepthFunction)resint);

            GL.GetInteger(GetPName.DepthBits, out resint);
            Console.WriteLine("Depth bits is {0}", resint);

            float[] range = new float[2];
            GL.GetFloat(GetPName.DepthRange, range); // returns an array
            Console.WriteLine("Depth range is {0} - {1}", range[0], range[1]);

            //GL.ClearDepth(1);

            while (!exit)
            {
                lastUpdateDt = update_watch.Elapsed.TotalSeconds + updateTime.Elapsed.TotalSeconds;
                updateTime.Reset();
                updateTime.Start();
                Update(lastUpdateDt);
                updateTime.Stop();
                update_watch.Reset();
                update_watch.Start();
                //this.Title = lastdt.ToString();

                lastRenderDt = render_watch.Elapsed.TotalSeconds + renderTime.Elapsed.TotalSeconds;
                renderTime.Reset();
                renderTime.Start();
                Render(lastRenderDt);
                renderTime.Stop();
                render_watch.Reset(); //  Stopwatch may be inaccurate over larger intervals.
                render_watch.Start(); // Plus, timekeeping is easier if we always start counting from 0.

                GL.Flush();
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

                //if (input.Pressed(InputVirtualKey.AxisLeft))
                //    box.Physics.UpdateRotation(InputVirtualKey.AxisLeft, dt);

                //if (input.Pressed(InputVirtualKey.AxisRight))
                //    box.Physics.UpdateRotation(InputVirtualKey.AxisRight, dt);

                // все нажатые кнопки помещаются в связный список
                LinkedListNode<InputVirtualKey> holdingKeyNode = input.FirstHoldingButton;
                while (holdingKeyNode != null)
                {
                    input.ProcessHoldingKey(holdingKeyNode.Value, dt);
                    holdingKeyNode = holdingKeyNode.Next;
                }

                

                foreach (DestroyByTimerAspect d in AspectLists.GetAspects(typeof(DestroyByTimerAspect)))
                {
                    d.Update(dt);
                }

                PhysicsManager.Update(dt);

                //foreach (BoundsAspect bound in AspectLists.GetDerivedAspects(typeof(BoundsAspect)))
                //{
                //    if (bound.GetType() == typeof(TriangleBoundsAspect))
                //    {
                //        if (bound.IntersectsWith((CircleBoundsAspect)box2.Bounds))
                //            MessageDispatcher.Post(new Collision(bound, box2.Bounds));
                //        else
                //            MessageDispatcher.Post(new NotCollision(bound, box2.Bounds));
                //    }
                //}

                foreach (BoundSetAspect boundSet in AspectLists.GetAspects(typeof(BoundSetAspect)))
                {
                    foreach (BoundSetAspect boundSet2 in AspectLists.GetAspects(typeof(BoundSetAspect)))
                    {
                        if (boundSet != boundSet2)
                            CollisionManager.CheckIntersection(boundSet, boundSet2);
                    }
                }
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
                    mainCamera.ViewportWidth = viewport_width;
                    mainCamera.ViewportHeight = viewport_height;
                    viewport_changed = false;
                }
            }

            //Matrix4 perspective =
            //    Matrix4.CreatePerspectiveFieldOfView((float)(Math.PI * 45f / 180f), viewport_width / (float)viewport_height, 0.1f, 100f);
            ////Matrix4.CreateOrthographic(2, 2, -1, 1);
            //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadMatrix(ref perspective);

            //GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            mainCamera.SetView();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //GL.Clear(ClearBufferMask.ColorBufferBit);
            //GL.LineWidth(4);

            foreach (GraphicsAspect g in AspectLists.GetAspects(typeof(GraphicsAspect)))
            {
                GL.PushAttrib(AttribMask.EnableBit | AttribMask.LightingBit | AttribMask.CurrentBit);
                GL.Disable(EnableCap.Lighting);
                GL.Disable(EnableCap.Texture2D);
                GL.Color3(g.color);

                GL.LineWidth(g.lineWidth);
                GL.PushMatrix();
                //GL.LoadIdentity();
                GL.Translate(g.translation);
                GL.Rotate(g.rotationAngle, g.rotationAxis);
                //GL.Scale(g.scaling);

                GL.Begin(BeginMode.TriangleFan);
                //GL.Begin(BeginMode.LineStrip);
                //GL.Begin(BeginMode.Lines);

                foreach (Vector3 vertex in g.vertices)
                {
                    GL.Vertex3(vertex);
                }

                GL.End();

                GL.PopMatrix();
                GL.PopAttrib();
            }

            //GL.Begin(BeginMode.LineLoop);

            //GL.Vertex3(-0.1, -0.3, -1); // back left
            //GL.Vertex3(-0.1, 0.1, -1); // front left
            //GL.Vertex3(0, 0.3, -1); // font
            //GL.Vertex3(0.1, 0.1, -1); // front right
            //GL.Vertex3(0.1, -0.3, -1); // back right

            //GL.End();

            //GL.Color3(Color.Green);
            //GL.Begin(BeginMode.Points);
            //GL.Vertex3(0, 0, 1f);
            //GL.End();

            //float[] pixels = new float[1];
            //GL.ReadPixels(400, 300, 1, 1, PixelFormat.DepthComponent, PixelType.Float, pixels);
            //this.Title = pixels[0].ToString();
            //Console.WriteLine("Depth read pixels is {0}", pixels[0]);
            GL.Flush();
        }

        #endregion

        #region IMessageHandler Members

        public void ProcessMessage(object message)
        {
            Type type = message.GetType();
            handlers[type](message);
        }

        #endregion

        private bool WriteTitle(object message)
        {
            TraceText text = (TraceText)message;
            this.Title = text.Text;
            return true;
        }

        private bool HandleButtonUp(object message)
        {
            ButtonUp buttonUp = (ButtonUp)message;
            if (buttonUp.Button == InputVirtualKey.Action17)
                Exit();

            return true;
        }
    }
}
