using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Engine.Chart.RayTrace
{
    public class Material
    {
	    private Color _color = new Color(0.2f, 0.2f, 0.2f);
	    private float _refl = 0;
	    private float _diff = 0.2f;
        private float _refrIndex = 1.5f;
        private float _refraction = 0;

        public Color Color { get { return _color; } set { _color = value; } }
	    public float Diffuse { get { return _diff; } set {_diff = value;} }
	    public float Reflection { get { return _refl; }  set { _refl = value;} }
        public float Specular { get { return 1.0f - _diff; } }
        public float Refraction { get { return _refraction; } set { _refraction = value; } }
        public float RefrIndex { get { return _refrIndex; } set { _refrIndex = value; } }
    }
}
