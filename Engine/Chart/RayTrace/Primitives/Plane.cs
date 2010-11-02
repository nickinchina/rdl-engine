using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Engine.Chart.RayTrace.Primitives
{
    public class Plane : Primitive
    {
        private Vector3 _normal;
        private Vector3 _n;
        private Vector3 _revN;
        private float _l;

	    public Plane( Vector3 normal ) 
        {
            _normal = normal;
            _n = (new Vector3(normal)).Normalize();
            _revN = new Vector3(_n * -1);
            _l = _normal.Length;
        }
        //public override Vector3 GetNormal(Point3 v) 
        //{
        //    float f = v.Dot(_normal);
        //    return (f < 0) ? _n : _revN; 
        //}

        public override IntersectResult Intersect(Ray ray, out Ray intersect, out float dist)
        {
            dist = 0;
            intersect = null;
            // Find the cos to the angle between the normal to the plane and the ray.
            float d = _n.Dot(ray.Direction);
            // If the ray is parallel to the plane then the angle with be 90
            if (d != 0)
            {
                float angleToCenter = _n.Dot(ray.Origin);
                dist = (angleToCenter + _l) / d;
                if (dist > 0)
                {
                    intersect = new Ray(new Point3(ray.Origin + ray.Direction * dist),
                        (angleToCenter < 0) ? _n : _revN);
                    return IntersectResult.HIT;
                }
            }
            return IntersectResult.MISS;
        }
    }
}
