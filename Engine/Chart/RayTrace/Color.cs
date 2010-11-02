using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Engine.Chart.RayTrace
{
    public class Color : Matrix
    {
        public Color() : base(3) {}
        public Color(Matrix v) : base(v) {}
        public Color(float r, float g, float b) : base(3) { this[0] = r; this[1] = g; this[2] = b; }

        public float R { get { return this[0]; } set { this[0] = value; } }
        public float G { get { return this[1]; } set { this[1] = value; } }
        public float B { get { return this[2]; } set { this[2] = value; } }
    }
}
