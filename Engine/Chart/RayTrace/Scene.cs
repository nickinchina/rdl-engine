using System;
using System.Collections.Generic;
using System.Text;
using Rdl.Engine.Chart.RayTrace.Primitives;

namespace Rdl.Engine.Chart.RayTrace
{
    public class Scene
    {
	    private List<Primitive> _primitives = new List<Primitive>();

        public void InitScene()
        {
            Primitive prim;
            // ground plane
            prim = new Plane(new Vector3(0, -2f, 0));
            prim.Name = "plane";
            prim.Material.Reflection = 0f;
            prim.Material.Diffuse = 1f;
            prim.Material.Color = new Color(0.4f, 0.3f, 0.3f);
            _primitives.Add(prim);
            // big sphere
            prim = new Sphere(new Point3(1, -0.8f, 3), 2.5f);
            prim.Name = "big sphere";
            prim.Material.Reflection = 0.3f;// 0.6f;
            prim.Material.Diffuse = 1;
            prim.Material.Refraction = 0.5f;
            prim.Material.RefrIndex = 1f;
            prim.Material.Color = new Color(0f, 0f, 1f); // new Color(0.7f, 0.7f, 0.7f);
            _primitives.Add(prim);
            // small sphere
            prim = new Sphere(new Point3(-5.5f, -0.5f, 7f), 2);
            prim.Name = "small sphere";
            prim.Material.Reflection = 1.0f;
            prim.Material.Diffuse = 0.1f;
            prim.Material.Color = new Color(0.7f, 0.7f, 1.0f);
            _primitives.Add(prim);
            // Box
            prim = new Box(new Point3(-2.5f, -1f, 4f), new Vector3(2, 0, 0), new Vector3(0, 2, 0), new Vector3(0, 0, 2));
            prim.Name = "box";
            prim.Material.Reflection = 0.0f;
            prim.Material.Diffuse = 1f;
            prim.Material.Color = new Color(0.0f, 0.0f, 1.0f);
            _primitives.Add(prim);
            //// light source 1
            //prim = new Sphere(new Point3(0, 5, 5), 0.1f);
            //prim.Light = true;
            //prim.Material.Color = new Color(0.6f, 0.6f, 0.6f);
            //_primitives.Add(prim);
            // light source 2
            prim = new Sphere(new Point3(2, 5, -1), 0.1f);
            prim.Light = true;
            prim.Material.Color = new Color(1f, 1f, 1f);
            _primitives.Add(prim);
        }

	    public int GetNrPrimitives() 
        { 
            return _primitives.Count; 
        }

	    public Primitive GetPrimitive( int a_Idx ) 
        { 
            return _primitives[a_Idx]; 
        }
    }
}
