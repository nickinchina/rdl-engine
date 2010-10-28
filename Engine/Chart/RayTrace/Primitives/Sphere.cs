using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Engine.Chart.RayTrace.Primitives
{
    public class Sphere : Primitive
    {
        private Point3 _centre;
	    private float _sqRadius, _radius, _rRadius;

        public Sphere( Point3 centre, float radius ) 
        { 
		    _centre = centre;
            _sqRadius = radius * radius;
		    _radius = radius;
            _rRadius = 1.0f / radius;
        }

	    public Point3 GetCentre() { return _centre; }
	    public float GetSqRadius() { return _sqRadius; }

        //public override Vector3 GetNormal(Point3 pos) { return new Vector3((pos - _centre) * _rRadius); }

        public override IntersectResult Intersect(Ray ray, out Ray intersect, out float dist)
        {
            IntersectResult ret = IntersectResult.MISS;
            dist = 0;
            intersect = null;
            Vector3 v = new Vector3(ray.Origin - _centre);
            float b = -v.Dot(ray.Direction);
	        float det = (b * b) - v.Dot(v) + _sqRadius;
	        if (det > 0)
	        {
		        det = (float)Math.Sqrt( det );
		        float i1 = b - det;
		        float i2 = b + det;
		        if (i2 > 0)
		        {
                    if (i1 < 0) 
			        {
				        dist = i2;
                        ret = IntersectResult.INPRIM;
			        }
			        else
			        {
				        dist = i1;
                        ret = IntersectResult.HIT;
			        }
                    Point3 pt = new Point3(ray.Origin + ray.Direction * dist);
                    intersect = new Ray(pt,
                        new Vector3((pt - _centre) * _rRadius));
                }
	        }
            return ret;
        }
    }
}
