using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Rdl.Engine;

namespace Rdl.Engine.Table
{
    class Cell : ReportElement
    {
        private ReportItems _reportItems = null;
        private Int16 _colSpan = 1;
        private Int16 _colIndex = 0;

        public Cell(XmlNode node, ReportElement parent, Int16 colIndex)
            :
            base(node, parent)
        {
            _container = true;
            _cell = true;
            _colIndex = colIndex;
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "reportitems":
                    _reportItems = new ReportItems(attr, this);
                    break;
                case "colspan":
                    _colSpan = Int16.Parse(attr.InnerText);
                    break;
                default:
                    break;
            }
        }

        public void Render(Rdl.Render.Container box, Rdl.Runtime.Context context, ref decimal cellPos)
        {
            Table table = FindTable(this);
            Rdl.Render.FixedContainer cellBox = null;

            bool visible = true;
            if (table.TableColumns[_colIndex].Visibility != null && table.TableColumns[_colIndex].Visibility.IsHidden(context) && table.TableColumns[_colIndex].Visibility.ToggleItem == null)
                visible = false;
            if (box != null && visible)
            {
                cellBox = box.AddFixedContainer(this, Style, context);
                cellBox.Name = "TableCell";
                cellBox.Left = cellPos;
                for (int i = 0; i < _colSpan; i++)
                    cellBox.Width += table.TableColumns[_colIndex + i].Width.points;
                cellBox.Height = box.Height;
                cellBox.MatchParentHeight = true;

                cellPos += cellBox.Width;
            }

            if (_reportItems != null)
                _reportItems.Render(cellBox, context);
        }

        public Int16 ColSpan
        {
            get { return _colSpan; }
        }
    }
}
