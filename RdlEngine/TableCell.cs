using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RdlEngine
{
    class TableCell : ReportElement
    {
        private ReportItem _reportItem = null;
        private Int16 _colSpan = 1;
        private Int16 _colIndex = 0;

        public TableCell(XmlNode node, ReportElement parent, Int16 colIndex)
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
                    _reportItem = ReportItem.NewReportItem(attr.ChildNodes[0], this);
                    break;
                case "colspan":
                    _colSpan = Int16.Parse(attr.InnerText);
                    break;
                default:
                    break;
            }
        }

        private Table FindTable(ReportElement elmt)
        {
            if (elmt is Table)
                return (Table)elmt;
            else if (elmt.Parent != null)
                return FindTable(elmt.Parent);
            return null;
        }


        internal override void Parse2()
        {
            base.Parse2();

            Table table = FindTable(this);
            if (table.TableColumns[_colIndex].Visibility.ToggleItem != null)
            {
                ReportItem toggleItem = Report.ReportItems[table.TableColumns[_colIndex].Visibility.ToggleItem];
                if (toggleItem != null)
                {
                    if (toggleItem is TextBox)
                        ((TextBox)toggleItem).ToggleList.Add(this);
                    else
                        throw new Exception("Toggle items are only allowed to refer to text boxes\n");
                }
                else
                    throw new Exception("Toggle item " + table.TableColumns[_colIndex].Visibility.ToggleItem + " not found");
            }
        }

        public void Render(RdlRender.Container box, ref decimal cellPos)
        {
            base.Render(box);

            Table table = FindTable(this);
            RdlRender.FixedContainer cellBox = null;

            bool visible = true;
            if (table.TableColumns[_colIndex].Visibility != null && table.TableColumns[_colIndex].Visibility.IsHidden && table.TableColumns[_colIndex].Visibility.ToggleItem == null)
                visible = false;
            if (box != null && visible)
            {
                cellBox = box.AddFixedContainer(this, Style);
                cellBox.Name = "TableCell";
                cellBox.Left = cellPos;
                for (int i = 0; i < _colSpan; i++)
                    cellBox.Width += table.TableColumns[_colIndex + i].Width.points;

                cellPos += cellBox.Width;
                cellBox.MatchParentHeight = true;
            }

            _reportItem.Render(cellBox);
        }

        public Int16 ColSpan
        {
            get { return _colSpan; }
        }
    }
}
