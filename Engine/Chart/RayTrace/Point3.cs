using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Engine.Chart.RayTrace
{
    public class Point3 : Matrix
    {
        public float X { get { return this[0]; } set { this[0] = value; } }
        public float Y { get { return this[1]; } set { this[1] = value; } }
        public float Z { get { return this[2]; } set { this[2] = value; } }

        public Point3() : base(3) { }
        public Point3(Matrix v) : base(v) { }
        public Point3(float x, float y, float z) : base(3) { Set(x, y, z); }
        public void Set(float x, float y, float z) { X = x; Y = y; Z = z; }
    }
}
