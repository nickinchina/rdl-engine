using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Rdl.Engine.Chart.RayTrace.Primitives;

namespace Rdl.Engine.Chart.RayTrace
{
    public class Engine
    {
        private const int TRACEDEPTH = 5;
        private const float EPSILON = 0.0001f;

	    // renderer data
	    private float _wX1, _wY1, _wX2, _wY2, _dX, _dY, _sX, _sY;
	    private Scene _scene = new Scene();
        private Bitmap _bm;
	    private int _currLine;
	    //protected Primitive[] _lastRow;

	    public void SetTarget( Bitmap bm )
        {
            _bm = bm;
        }

        public Scene GetScene() 
        { 
            return _scene; 
        }

	    public Primitive Raytrace( Ray ray, ref Color acc, int depth, float rIndex, out float dist )
        {
            // trace primary ray
            dist = 1000000.0f;
            if (depth > TRACEDEPTH) return null;
            Primitive prim = null;
            Ray primIntersect = null;
            Primitive.IntersectResult result = Primitive.IntersectResult.MISS;
            // find the nearest intersection
            for (int s = 0; s < _scene.GetNrPrimitives(); s++)
            {
                float dist1;
                Primitive pr = _scene.GetPrimitive(s);
                Ray intersect;
                Primitive.IntersectResult res = pr.Intersect(ray, out intersect, out dist1);
                if (res != Primitive.IntersectResult.MISS)
                    if (dist1 < dist)
                    {
                        prim = pr;
                        primIntersect = intersect;
                        dist = dist1;
                        result = res;
                    }
            }
            // no hit, terminate ray
            if (prim == null) return null;
            // handle intersection
            if (prim.Light)
            {
                // we hit a light, stop tracing
                acc = new Color(1, 1, 1);
            }
            else
            {
		        // trace lights
		        for ( int l = 0; l < _scene.GetNrPrimitives(); l++ )
		        {
			        Primitive p = _scene.GetPrimitive( l );
			        if (p.Light)
			        {
				        Primitive light = p;
                        Vector3 L;
                        // handle point light source
				        float shade = 1.0f;
                        if (light is Primitives.Sphere)
				        {
                            L = new Vector3(((Sphere)light).GetCentre() - primIntersect.Origin);
					        float tdist = L.Length;
                            L = L.Normalize();
                            float dot = L.Dot(primIntersect.Direction);

                            Ray r = new Ray(new Point3(primIntersect.Origin + L * EPSILON), L);
					        for ( int s = 0; s < _scene.GetNrPrimitives(); s++ )
					        {
						        Primitive pr = _scene.GetPrimitive( s );
                                Ray intersect;
						        if ((pr != light) && (pr.Intersect( r, out intersect, out tdist ) != Primitive.IntersectResult.MISS))
						        {
                                    if (intersect.Direction.Dot(L) < 0)
						                shade = 0;
							        break;
						        }
					        }

                            // calculate diffuse shading
				            if (prim.Material.Diffuse > 0)
				            {
					            if (dot > 0)
					            {
						            float diff = dot * prim.Material.Diffuse * shade;
						            // add diffuse component to ray color
						            acc = new Color(acc + (diff * light.Material.Color * prim.Material.Color));
					            }
				            }

				            // determine specular component
				            if (prim.Material.Specular > 0)
				            {
					            // point light source: sample once for specular highlight
					            Vector3 V = ray.Direction;
                                Vector3 R = new Vector3(L - 2.0f * L.Dot(primIntersect.Direction) * primIntersect.Direction);
					            dot = V.Dot( R );
					            if (dot > 0)
					            {
						            float spec = (float)Math.Pow( dot, 20 ) * prim.Material.Specular * shade;
						            // add specular component to ray color
						            acc = new Color(acc + (spec * light.Material.Color));
					            }
				            }
				        }
			        }
		        }
                // calculate reflection
                float refl = prim.Material.Reflection;
                if (refl > 0.0f)
                {
                    Vector3 R = new Vector3(ray.Direction - 2.0f * ray.Direction.Dot(primIntersect.Direction) * primIntersect.Direction);
                    if (depth < TRACEDEPTH)
                    {
                        Color rcol = new Color(0, 0, 0);
                        float dist1;
                        Raytrace(new Ray(new Point3(primIntersect.Origin + R * EPSILON), R), ref rcol, depth + 1, rIndex, out dist1);
                        acc = new Color(acc + (refl * rcol * prim.Material.Color));
                    }
                }
                // calculate refraction
                float refr = prim.Material.Refraction;
                if ((refr > 0) && (depth < TRACEDEPTH))
                {
                    float rindex = prim.Material.RefrIndex;
                    float n = rIndex / rindex;
                    Vector3 N = new Vector3(primIntersect.Direction * (float)result);
                    float cosI = -N.Dot(ray.Direction);
                    float cosT2 = 1.0f - n * n * (1.0f - cosI * cosI);
                    if (cosT2 > 0.0f)
                    {
                        Vector3 T = new Vector3((n * ray.Direction) + (n * cosI - (float)Math.Sqrt(cosT2)) * N);
                        Color rcol = new Color(0, 0, 0);
                        float dist1;
                        Raytrace(new Ray(new Point3(primIntersect.Origin + T * EPSILON), T), ref rcol, depth + 1, rindex, out dist1);
                        // apply Beer's law
                        Color absorbance = new Color(prim.Material.Color * 0.15f * dist1);
                        Color transparency = new Color(absorbance.R, absorbance.G, absorbance.B);
                        acc = new Color(acc + rcol * transparency);
                    }
                }
            }
            // return pointer to primitive hit by primary ray
            return prim;
        }

        public void InitRender()
        {
	        // set firts line to draw to
	        _currLine = 0;
	        // screen plane in world space coordinates
            _wX1 = -4; _wX2 = 4; _wY1 = _sY = 3; _wY2 = -3;
	        // calculate deltas for interpolation
	        _dX = (_wX2 - _wX1) / _bm.Width;
            _dY = (_wY2 - _wY1) / _bm.Height;
	        _sY += 20 * _dY;
	        // allocate space to store pointers to primitives for previous line
	        //_lastRow = new Primitive[_bm.Width];
        }

        public bool Render()
        {
	        // render scene
	        Point3 o = new Point3( 0, 0, -5 );
	        // render remaining lines
	        for ( int y = _currLine; y < _bm.Height; y++ )
	        {
		        _sX = _wX1;
		        // render pixels for current line
		        for ( int x = 0; x < _bm.Width; x++ )
		        {
			        // fire primary ray
			        Color acc = new Color( 0, 0, 0 );
			        Vector3 dir = (new Vector3(new Vector3( _sX, _sY, 0 ) - o)).Normalize();
			        Ray r = new Ray( o, dir );
			        float dist;
			        Primitive prim = Raytrace( r, ref acc, 1, 1.0f, out dist );
			        int red = (int)(acc.R * 256);
			        int green = (int)(acc.G * 256);
			        int blue = (int)(acc.B * 256);
			        if (red > 255) red = 255;
			        if (green > 255) green = 255;
			        if (blue > 255) blue = 255;
                    _bm.SetPixel(x, y, System.Drawing.Color.FromArgb(red, green, blue));
			        _sX += _dX;
		        }
		        _sY += _dY;
                //// see if we've been working to long already
                //if ((GetTickCount() - msecs) > 100) 
                //{
                //    // return control to windows so the screen gets updated
                //    _currLine = y + 1;
                //    return false;
                //}
	        }
	        // all done
	        return true;
        }
    }
}
