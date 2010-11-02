using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Engine.Chart.RayTrace.Primitives
{
    class Box : Primitive
    {
        private Point3 _corner1, _corner2;
        private Vector3 _width, _height, _depth;
        private Vector3 _width_normal, _height_normal, _depth_normal;
        private float _l1, _l2;

        public Box(Point3 corner, Vector3 width, Vector3 height, Vector3 depth)
        {
            _corner1 = corner;
            _corner2 = new Point3(corner + width + height + depth);
            _width = width;
            _width_normal = width.Normalize();
            _height = height;
            _height_normal = _height.Normalize();
            _depth = depth;
            _depth_normal = _depth.Normalize();
            _l1 = (new Vector3(_corner1)).Length;
            _l2 = (new Vector3(_corner2)).Length;

            float v = _corner1.Dot(_height_normal);
            float f2 = _corner1.Dot(_depth_normal);
            float f3 = _corner2.Dot(_width_normal);
        }

        public override IntersectResult Intersect(Ray ray, out Ray intersect, out float dist)
        {
            dist = float.MaxValue;
            intersect = null;

            CheckFace(ray, new Vector3(_height_normal * -1), _corner1, new Point3(_corner1 + _width), new Point3(_corner1 + _width + _depth), new Point3(_corner1 + _depth), ref dist, ref intersect);
            CheckFace(ray, _height_normal, _corner2, new Point3(_corner2 - _width), new Point3(_corner2 - _width - _depth), new Point3(_corner2 - _depth), ref dist, ref intersect);
            CheckFace(ray, new Vector3(_width_normal * -1),_corner1, new Point3(_corner1 + _height), new Point3(_corner1 + _height + _depth), new Point3(_corner1 + _depth), ref dist, ref intersect);
            CheckFace(ray, _width_normal,_corner2, new Point3(_corner2 - _height), new Point3(_corner2 - _height - _depth), new Point3(_corner2 - _depth), ref dist, ref intersect);
            CheckFace(ray, new Vector3(_depth_normal * -1), _corner1, new Point3(_corner1 + _width), new Point3(_corner1 + _width + _height), new Point3(_corner1 + _height), ref dist, ref intersect);
            CheckFace(ray, _depth_normal, _corner2, new Point3(_corner2 - _width), new Point3(_corner2 - _width - _height), new Point3(_corner2 - _height), ref dist, ref intersect);

            return (intersect == null) ? IntersectResult.MISS : IntersectResult.HIT;
        }

        private void CheckFace(Ray ray, Vector3 normal, Point3 vertex1, Point3 vertex2, Point3 vertex3, Point3 vertex4,
            ref float dist, ref Ray intersect)
        {
            // Find the cos to the angle between the normal to the plane and the ray.
            float d = normal.Dot(ray.Direction);

            // If the ray is parallel to the plane then the angle with be 90
            if (d != 0)
            {
                Point3 pt = new Point3(ray.Origin + (normal.Dot(vertex1 - ray.Origin) / d) * ray.Direction);

                // We know that pt is on the plane of the side of the box, the only question is 
                // whether the point is inside the rectangle
                // Check the dot product between _corner1 to p1 and _corner1 to 
                Vector3 side1 = (new Vector3(vertex2 - vertex1)).Normalize();
                Vector3 side2 = (new Vector3(vertex4 - vertex1)).Normalize();
                //Vector3 side3 = (new Vector3(vertex3 - vertex2)).Normalize();
                //Vector3 side4 = (new Vector3(vertex3 - vertex4)).Normalize();
                Vector3 v1 = (new Vector3(pt - vertex1)).Normalize();
                Vector3 v2 = (new Vector3(pt - vertex3)).Normalize();
                if (side1.Dot(v1) > 0 && side2.Dot(v1) > 0 && side1.Dot(v2) < 0 && side2.Dot(v2) < 0)
                {
                    float dist1 = new Vector3(ray.Origin - pt).Length;

                    if (dist1 < dist)
                    {
                        intersect = new Ray(pt, normal);
                        dist = dist1;
                    }
                }
            }
        }
    }
}
