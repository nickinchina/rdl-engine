using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Engine.Chart.RayTrace
{
    public class Vector3 : Matrix
    {
        public float X { get { return this[0]; } set { this[0] = value; } }
        public float Y { get { return this[1]; } set { this[1] = value; } }
        public float Z { get { return this[2]; } set { this[2] = value; } }

        public Vector3() : base(3) { }
        public Vector3(Matrix v) : base(v) { }
        public Vector3(float x, float y, float z) : base(3) { Set(x, y, z); }
        public void Set(float x, float y, float z) { X = x; Y = y; Z = z; }
        public Vector3 Cross(Vector3 b) { return new Vector3(Y * b.Z - Z * b.Y, Z * b.X - X * b.Z, X * b.Y - Y * b.X); }

        // Normalize will remove the length component from a vector leaving only the direction.
        public Vector3 Normalize()
        {
            float l = Length;
            return new Vector3(X / l, Y / l, Z / l);
        }
        public float Length { get { return (float)Math.Sqrt(Dot(this)); } }
    }
}
