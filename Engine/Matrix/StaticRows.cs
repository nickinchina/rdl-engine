using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine.Matrix
{
    class StaticRows : RowGrouping
    {
        List<ReportItems> _staticRows = new List<ReportItems>();

        public StaticRows(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
            _container = true;
            _cell = true;
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            base.ParseAttribute(attr);
            switch (attr.Name.ToLower())
            {
                case "staticrow":
                    _staticRows.Add(new ReportItems(attr.ChildNodes[0], this));
                    break;
                default:
                    break;
            }
        }

        internal override void Render(Rdl.Render.Container box, Rdl.Runtime.Context context)
        {
            if (_staticRows.Count > FindMatrix(this).Rows.Count)
                throw new Exception("There are more static row elements defined in matrix " + FindMatrix(this).Name + " than there are data rows.");

            Rdl.Render.FlowContainer contentBox = null;
            if (box != null)
            {
                contentBox = box.AddFlowContainer(this, Style, context, Rdl.Render.FlowContainer.FlowDirectionEnum.TopDown);
                contentBox.Name = "StaticRowContent";
                contentBox.Width = Width.points;
                contentBox.MatchParentWidth = true;
                contentBox.CanGrowVertically = true;
            }

            for (int i = 0; i < _staticRows.Count; i++)
            {
                Rdl.Render.FixedContainer cellBox = null;
                if (contentBox != null)
                {
                    cellBox = contentBox.AddFixedContainer(this, Style, context);
                    cellBox.Name = "StaticRowCell";
                    cellBox.Height = FindMatrix(this).Rows[i].Height.points;
                    cellBox.MatchParentWidth = true;
                }

                ReportItems item = _staticRows[i];

                item.Render(cellBox, context);
            }

            if (RenderNext != null)
                if (RenderNext is ColumnGrouping)
                    ((ColumnGrouping)RenderNext).Render(box, FindMatrix(this).Context, context, 0);
                else
                    RenderNext.Render(box, context);
        }
    }
}
