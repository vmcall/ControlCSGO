using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Control
{
    // Credits: Zat
    public static class MathUtils
    {
        public static Vector2 Vector2EUCtoSDX(this Vector2 vec) => new Vector2(vec.X, vec.Y);
        public static Vector2[] Vector2EUCtoSDX(this Vector2[] vec)
        {
            Vector2[] vecs = new Vector2[vec.Length];
            for (int i = 0; i < vecs.Length; i++)
                vecs[i] = Vector2EUCtoSDX(vec[i]);
            return vecs;
        }
        public static Vector2[] WorldToScreen(this Matrix viewMatrix, Rectangle screenSize, params Vector3[] points)
        {
            Vector2[] worlds = new Vector2[points.Length];
            for (int i = 0; i < worlds.Length; i++)
                worlds[i] = viewMatrix.WorldToScreen(screenSize, points[i]);
            return worlds;
        }
        public static Vector2 WorldToScreen(this Matrix viewMatrix, Rectangle screenSize, Vector3 point3D)
        {
            Vector2 returnVector = Vector2.Zero;
            float w = viewMatrix[3, 0] * point3D.X + viewMatrix[3, 1] * point3D.Y + viewMatrix[3, 2] * point3D.Z + viewMatrix[3, 3];
            if (w >= 0.01f)
            {
                float inverseX = 1f / w;
                returnVector.X =
                    (screenSize.Width / 2f) +
                    (0.5f * (
                    (viewMatrix[0, 0] * point3D.X + viewMatrix[0, 1] * point3D.Y + viewMatrix[0, 2] * point3D.Z + viewMatrix[0, 3])
                    * inverseX)
                    * screenSize.Width + 0.5f);
                returnVector.Y =
                    (screenSize.Height / 2f) -
                    (0.5f * (
                    (viewMatrix[1, 0] * point3D.X + viewMatrix[1, 1] * point3D.Y + viewMatrix[1, 2] * point3D.Z + viewMatrix[1, 3])
                    * inverseX)
                    * screenSize.Height + 0.5f);
            }
            return returnVector;
        }
        
        public static float ToRadians(this float deg) => deg * (float)(Math.PI / 180f);
        public static float ToDegrees(this float rad) => rad * (float)(180f / Math.PI);

    }

    // Credits: Zat
    public class Matrix
    {
        #region VARIABLES
        private float[] data;
        private int rows, columns;
        #endregion

        #region CONSTRUCTOR
        public Matrix(int rows, int columns)
        {
            this.rows = rows;
            this.columns = columns;
            this.data = new float[rows * columns];
        }
        #endregion

        #region METHODS
        public void Read(byte[] data)
        {
            for (int y = 0; y < rows; y++)
                for (int x = 0; x < columns; x++)
                    this[y, x] = BitConverter.ToSingle(data, sizeof(float) * ((y * columns) + x));
        }
        public byte[] ToByteArray()
        {
            int sof = sizeof(float);
            byte[] data = new byte[this.data.Length * sof];
            for (int i = 0; i < this.data.Length; i++)
                Array.Copy(BitConverter.GetBytes(this.data[i]), 0, data, i * sof, sof);
            return data;
        }
        #endregion

        #region OPERANDS
        public float this[int i]
        {
            get { return data[i]; }
            set { data[i] = value; }
        }
        public float this[int row, int column]
        {
            get { return data[row * columns + column]; }
            set { data[row * columns + column] = value; }
        }
        #endregion
    }
}
