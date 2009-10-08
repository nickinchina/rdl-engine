using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RdlEngine
{
    class TableRow : ReportElement
    {
        private List<TableCell> _tableCells = new List<TableCell>();
        private Size _height = Size.ZeroSize;
        private Visibility _visibility = Visibility.Visible;

        public TableRow( XmlNode node, ReportElement parent ) :
            base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "tablecells":
                    Int16 i = 0;
                    foreach (XmlNode child in attr.ChildNodes)
                    {
                        TableCell c = new TableCell(child, this, i);
                        _tableCells.Add(c);
                        i += c.ColSpan;
                    }
                    break;
                case "height":
                    _height = new Size(attr.InnerText);
                    break;
                case "visibility":
                    _visibility = new Visibility(attr, this);
                    break;
                default:
                    break;
            }
        }

        public override void Render(RdlRender.Container box)
        {
            base.Render(box);

            RdlRender.FixedContainer rowBox = null;
            bool visible = true;
            if (_visibility != null && _visibility.IsHidden && _visibility.ToggleItem == null)
                visible = false;
            if (box != null && visible)
            {
                rowBox = box.AddFixedContainer(this, Style);
                rowBox.Name = "TableRow";
                rowBox.Width = box.Width;
                rowBox.Height = _height.points;
            }
            decimal cellPos = 0;
            foreach (TableCell tc in _tableCells)
                tc.Render(rowBox, ref cellPos);
        }
    }
}
