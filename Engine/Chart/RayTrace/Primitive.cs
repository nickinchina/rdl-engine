using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Engine.Chart.RayTrace
{
    public abstract class Primitive
    {
	    protected Material _material = new Material();
	    protected string _name = string.Empty;
	    protected bool _light = false;

        public enum PrimitiveTypeEnum
	    {
		SPHERE = 1,
		PLANE
	    };

        public enum IntersectResult
        {
            HIT = 1,    // Ray hit primitive
            MISS = 0,   // Ray missed primitive
            INPRIM = -1  // Ray started inside primitive
        };

        public Material Material { get { return _material;} set {_material = value;}}
        public abstract IntersectResult Intersect(Ray ray, out Ray intersect, out float dist);
	    public virtual Color GetColor( Vector3 v ) { return _material.Color; }
	    public virtual bool Light { get { return _light; } set {_light = value;} }
        public string Name { get { return _name; } set {_name = value;} }
    }
}
