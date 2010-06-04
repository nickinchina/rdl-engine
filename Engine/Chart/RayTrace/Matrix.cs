using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Engine.Chart.RayTrace
{
    public class Matrix
    {
        private float[] values;

        public Matrix(int size) { values = new float[size]; }
        public Matrix(float[] f) { values = new float[f.Length]; for (int i = 0; i < f.Length; i++) values[i] = f[i]; }
        public Matrix(Matrix v) { values = new float[v.Size]; for (int i = 0; i < v.Size; i++) values[i] = v[i]; }

        public float this[int index] { get { return values[index]; } set { values[index] = value; } }
        public int Size { get { return values.Length; } }

        public float Dot(Matrix v) 
        {
            float f = 0;
            for (int i = 0; i < Size; i++)
                f += values[i] * v[i];
            return f;
        }
        public static Matrix operator -(Matrix v) 
        { 
            float[] f = new float[v.Size];
            for (int i = 0; i < v.Size; i++)
                f[i] = -v[i];
            return new Matrix(f);
        }
        public static Matrix operator +(Matrix v1, Matrix v2) 
        {
            float[] f = new float[v1.Size];
            for (int i = 0; i < v1.Size; i++)
                f[i] = v1[i] + v2[i];
            return new Matrix(f);
        }
        public static Matrix operator -(Matrix v1, Matrix v2) 
        {
            float[] f = new float[v1.Size];
            for (int i = 0; i < v1.Size; i++)
                f[i] = v1[i] - v2[i];
            return new Matrix(f);
        }
        public static Matrix operator *(Matrix v1, Matrix v2) 
        {
            float[] f = new float[v1.Size];
            for (int i = 0; i < v1.Size; i++)
                f[i] = v1[i] * v2[i];
            return new Matrix(f);
        }
        public static Matrix operator *(Matrix v, float f) 
        {
            float[] fa = new float[v.Size];
            for (int i = 0; i < v.Size; i++)
                fa[i] = v[i] * f;
            return new Matrix(fa);
        }
        public static Matrix operator *(float f, Matrix v) 
        {
            float[] fa = new float[v.Size];
            for (int i = 0; i < v.Size; i++)
                fa[i] = v[i] * f;
            return new Matrix(fa);
        }

    }
}
