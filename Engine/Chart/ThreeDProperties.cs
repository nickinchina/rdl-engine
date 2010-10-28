using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine.Chart
{
    class ThreeDProperties : ChartElement
    {
        public enum ProjectionModeEnum
        {
            Perspective,
            Orthographic
        };

        public enum ShadingEnum
        {
            None,
            Simple,
            Real
        };

        public enum DrawingStyleEnum
        {
            Cylinder,
            Cube
        };

        private bool _enabled = false;
        private ProjectionModeEnum _projectionMode = ProjectionModeEnum.Perspective;
        private int _perspective = 0;
        private int _rotation = 0;
        private int _inclination = 0;
        private ShadingEnum _shading = ShadingEnum.None;
        private int _wallThickness = 0;
        private DrawingStyleEnum _drawingStyle = DrawingStyleEnum.Cube;
        private bool _clustered = false;

        public ThreeDProperties(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            base.ParseAttribute(attr);
            switch (attr.Name.ToLower())
            {
                case "enabled":
                    _enabled = bool.Parse(attr.InnerText);
                    break;
                case "projectionmode":
                    _projectionMode = (ProjectionModeEnum)Enum.Parse(typeof(ProjectionModeEnum), attr.InnerText, true);
                    break;
                case "perspective":
                    _perspective = int.Parse(attr.InnerText);
                    break;
                case "rotation":
                    _rotation = int.Parse(attr.InnerText);
                    break;
                case "inclination":
                    _inclination = int.Parse(attr.InnerText);
                    break;
                case "shading":
                    _shading = (ShadingEnum)Enum.Parse(typeof(ShadingEnum), attr.InnerText, true);
                    break;
                case "wallthickness":
                    _wallThickness = int.Parse(attr.InnerText);
                    break;
                case "drawingstyle":
                    _drawingStyle = (DrawingStyleEnum)Enum.Parse(typeof(DrawingStyleEnum), attr.InnerText, true);
                    break;
                case "clustered":
                    _clustered = bool.Parse(attr.InnerText);
                    break;
                default:
                    break;
            }
        }
    }
}
