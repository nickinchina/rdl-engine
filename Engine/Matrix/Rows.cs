using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Rdl.Engine;

namespace Rdl.Engine.Matrix
{
    class Rows : MatrixElement
    {
        List<Row> _rows = new List<Row>();

        public Rows(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "matrixrow":
                    _rows.Add(new Row(attr, this));
                    break;
                default:
                    break;
            }
        }

        public int Count
        {
            get { return _rows.Count; }
        }

        public Row this[int index]
        {
            get { return _rows[index]; }
        }

        public decimal Height
        {
            get
            {
                decimal height = 0;
                foreach (Row r in _rows)
                    height += r.Height.points;
                return height;
            }
        }

        internal override void Render(Rdl.Render.Container box, Rdl.Runtime.Context context)
        {
            Rdl.Render.FlowContainer rowsBox = null;
            if (box != null)
            {
                rowsBox = box.AddFlowContainer(this, Style, context, Rdl.Render.FlowContainer.FlowDirectionEnum.TopDown);
                rowsBox.Name = "RowsBox";
                rowsBox.MatchParentHeight = true;
                rowsBox.MatchParentWidth = true;
            }

            foreach (Row r in _rows)
            {
                r.Render(rowsBox, context);
            }
        }
    }
}
