using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SeaBattles
{
    /// <summary>
    /// Based on flipcode Camera class: http://www.flipcode.com/archives/OpenGL_Camera.shtml
    /// </summary>
    class Camera
    {
        private int viewport_width;
        private int viewport_height;

        //private readonly float right;
        //private readonly float up;
        //private readonly float forward;
        //private float position;
        private float[] transform = null;

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
        }

        internal void SetView()
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            Matrix4 mat = Matrix4.CreateOrthographicOffCenter(-this.viewport_width / 2, this.viewport_width / 2, -this.viewport_height / 2, this.viewport_height / 2, -1, 1);
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
    }
}
