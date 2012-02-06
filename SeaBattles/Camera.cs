using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SeaBattles.Messages;

namespace SeaBattles
{
    /// <summary>
    /// Based on flipcode Camera class: http://www.flipcode.com/archives/OpenGL_Camera.shtml
    /// </summary>
    class Camera : IMessageHandler
    {
        protected MessageHandler messageHandler = new MessageHandler();

        private float viewport_width;
        private float viewport_height;
        private float zoom = 1;

        //private readonly float right;
        //private readonly float up;
        //private readonly float forward;
        //private float position;
        private float[] transform = null;

        public float ViewportWidth
        {
            get { return viewport_width; }
            set { viewport_width = value; }
        }

        public float ViewportHeight
        {
            get { return viewport_height; }
            set { viewport_height = value; }
        }


        public Camera(float x, float y, float z, int viewport_width, int viewport_height)
        {
            this.viewport_width = viewport_width;
            this.viewport_height = viewport_height;

            transform = new float[16];
            transform[0] = 1.0f;
            transform[5] = 1.0f;
            transform[10] = -1.0f;
            transform[15] = 1.0f;
            transform[12] = x; transform[13] = y; transform[14] = z;

            //right = transform[0];
            //up = transform[4];
            //forward = transform[8];
            //position = transform[12];

            messageHandler.Handlers.Add(typeof(ButtonHold), HandleButtonHold); // регистрируем потенциальный обработчик
            MessageDispatcher.RegisterHandler(typeof(ButtonHold), messageHandler); // говорим, кто будет принимать сообщения данного типа
        }

        internal void SetView()
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            Matrix4 mat = Matrix4.CreateOrthographicOffCenter(  -this.viewport_width / 2 * zoom,
                                                                this.viewport_width / 2 * zoom,
                                                                -this.viewport_height / 2 * zoom,
                                                                this.viewport_height / 2 * zoom,
                                                                -2,
                                                                2);
            GL.LoadMatrix(ref mat);
            //Matrix4.CreatePerspectiveFieldOfView((float)(Math.PI * 45f / 180f), viewport_width / (float)viewport_height, 0.1f, 100f);
            //Glu.gluPerspective(45, 800.0f / 600.0f, 0.001f, 500);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            float[] viewmatrix = new float[]{//Remove the three - for non-inverted z-axis
						  transform[0], transform[4], -transform[8], 0,
						  transform[1], transform[5], -transform[9], 0,
						  transform[2], transform[6], -transform[10], 0,

						  -(transform[0]*transform[12] +
						  transform[1]*transform[13] +
						  transform[2]*transform[14]),

						  -(transform[4]*transform[12] +
						  transform[5]*transform[13] +
						  transform[6]*transform[14]),

						  //add a - like above for non-inverted z-axis
						  (transform[8]*transform[12] +
						  transform[9]*transform[13] +
						  transform[10]*transform[14]), 1};


            //Gl.glMultMatrixf(viewmatrix);
            GL.LoadMatrix(viewmatrix);
        }

        internal void MoveLoc(float x, float y, float z, float distance)
        {
            float dx = x * transform[0] + y * transform[4] + z * transform[8];
            float dy = x * transform[1] + y * transform[5] + z * transform[9];
            float dz = x * transform[2] + y * transform[6] + z * transform[10];
            transform[12] += dx * distance;
            transform[13] += dy * distance;
            transform[14] += dz * distance;
        }

        internal void MoveGlob(float x, float y, float z, float distance)
        {
            transform[12] += x * distance;
            transform[13] += y * distance;
            transform[14] += z * distance;
        }

        internal void RotateLoc(float deg, float x, float y, float z)
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadMatrix(transform);
            GL.Rotate(deg, x, y, z);
            GL.GetFloat(GetPName.ModelviewMatrix, transform);
            GL.PopMatrix();
        }

        internal void RotateGlob(float deg, float x, float y, float z)
        {
            float dx = x * transform[0] + y * transform[1] + z * transform[2];
            float dy = x * transform[4] + y * transform[5] + z * transform[6];
            float dz = x * transform[8] + y * transform[9] + z * transform[10];
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadMatrix(transform);
            GL.Rotate(deg, dx, dy, dz);
            GL.GetFloat(GetPName.ModelviewMatrix, transform);
            GL.PopMatrix();
        }

        internal float[] GetModelViewMatrix()
        {
            float[] matrix = new float[16];
            GL.GetFloat(GetPName.ModelviewMatrix, matrix);
            return matrix;
        }

        internal void ZoomIn(float speed)
        {
            zoom /= speed;
        }

        internal void ZoomOut(float speed)
        {
            zoom *= speed;
        }

        public void HandleButtonHold(object message)
        {
            ButtonHold buttonHold = (ButtonHold)message;
            switch (buttonHold.Button)
            {
                case InputVirtualKey.Unknown:
                    break;
                case InputVirtualKey.AxisLeft:
                    break;
                case InputVirtualKey.AxisRight:
                    break;
                case InputVirtualKey.AxisUp:
                    break;
                case InputVirtualKey.AxisDown:
                    break;
                case InputVirtualKey.Action1:
                    break;
                case InputVirtualKey.Action2:
                    break;
                case InputVirtualKey.Action3:
                    break;
                case InputVirtualKey.Action4:
                    break;
                case InputVirtualKey.Action5:
                    break;
                case InputVirtualKey.Action6:
                    break;
                case InputVirtualKey.Action7:
                    ZoomIn(1.01f);
                    break;
                case InputVirtualKey.Action8:
                    ZoomOut(1.01f);
                    break;
                case InputVirtualKey.Action9:
                    break;
                case InputVirtualKey.Action10:
                    break;
                case InputVirtualKey.Action11:
                    break;
                case InputVirtualKey.Action12:
                    break;
                case InputVirtualKey.Action13:
                    break;
                case InputVirtualKey.Action14:
                    break;
                case InputVirtualKey.Action15:
                    break;
                case InputVirtualKey.Action16:
                    break;
                default:
                    break;
            }
        }

        #region IMessageHandler Members

        public void ProcessMessage(object message)
        {
            messageHandler.ProcessMessage(message);
        }

        #endregion
    }
}
