using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Engine.Chart.RayTrace
{
    public class Ray
    {
	    private Point3 _origin;
	    private Vector3 _direction;

        public Ray() { _origin = new Point3(0, 0, 0); _direction = new Vector3(0, 0, 0); }
        public Ray(Point3 origin, Vector3 direction) { _origin = origin; _direction = direction; }

        public Point3 Origin { get { return _origin; } set { _origin = value; } }
        public Vector3 Direction { get { return _direction; } set { _direction = value; } }
    }
}
