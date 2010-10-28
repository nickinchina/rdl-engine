using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Rdl.Engine;

namespace Rdl.Engine.Matrix
{
    abstract class ColumnGrouping : MatrixElement
    {
        protected Size _height = Size.ZeroSize;
        protected bool _fixedHeader = false;

        public static ColumnGrouping GetColumnGrouping(XmlNode node, ReportElement parent)
        {
            XmlNamespaceManager ns = new XmlNamespaceManager(node.OwnerDocument.NameTable);
            ns.AddNamespace("rdl", parent.Report.XmlNameSpace);

            if (node.SelectSingleNode("rdl:DynamicColumns", ns) != null)
                return new DynamicColumns(node, parent);
            else
                return new StaticColumns(node, parent);
        }

        public ColumnGrouping(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "height":
                    _height = new Size(attr.InnerText);
                    break;
                case "dynamiccolumns":
                    foreach (XmlNode child in attr.ChildNodes)
                        ParseAttribute(child);
                    break;
                case "staticcolumns":
                    foreach (XmlNode child in attr.ChildNodes)
                        ParseAttribute(child);
                    break;
                case "fixedheader":
                    _fixedHeader = bool.Parse(attr.InnerText);
                    break;
                default:
                    break;
            }
        }

        public Size Height
        {
            get { return _height; }
        }

        public decimal TotalHeight
        {
            get
            {
                MatrixElement next = RenderNext;
                decimal height = Height.points;
                while (next != null && next is ColumnGrouping)
                {
                    height += ((ColumnGrouping)next).Height.points;
                    next = ((ColumnGrouping)next).RenderNext;
                }
                return height;
            }

        }

        internal abstract int RenderHeader(Rdl.Render.Container box, Rdl.Runtime.Context context, int column);
        internal abstract int Render(Rdl.Render.Container box, Rdl.Runtime.Context context, Rdl.Runtime.Context rowContext, int column);
    }
}
