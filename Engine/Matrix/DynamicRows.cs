using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Rdl.Engine;

namespace Rdl.Engine.Matrix
{
    class DynamicRows : RowGrouping
    {
        Grouping _grouping = null;
        List<SortBy> _sorting = new List<SortBy>();
        Subtotal _subtotal = null;
        ReportItems _reportItems = null;
        Visibility _visibility = Visibility.Visible;

        public DynamicRows(XmlNode node, ReportElement parent)
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
                case "grouping":
                    _grouping = new Grouping(attr, this);
                    break;
                case "sorting":
                    foreach(XmlNode child in attr.ChildNodes)
                        _sorting.Add(new SortBy(child, this));
                    break;
                case "subtotal":
                    _subtotal = new Subtotal(attr, this);
                    break;
                case "reportitems":
                    _reportItems = new ReportItems(attr, this);
                    break;
                case "visibility":
                    _visibility = new Visibility(attr, this);
                    break;
                default:
                    break;
            }
        }

        internal override void Render(Rdl.Render.Container box, Rdl.Runtime.Context context)
        {
            Rdl.Runtime.Context context1 = context.GetChildContext(
                null,
                null,
                _grouping,
                _sorting);

            base.Render(box, context1);

            bool hidden = false;
            if (_visibility != null && _visibility.ToggleItem == null)
                hidden = _visibility.IsHidden(context1);

            if (!hidden)
            {
                TextBox tb = Report.FindToggleItem(_visibility);

                Rdl.Render.FlowContainer dynamicRowsBox = null;
                if (box != null)
                {
                    // The dynamic rows box is a top down box holding all of the rows
                    // in the matrix including the total row.
                    dynamicRowsBox = box.AddFlowContainer(this, Style, context, Rdl.Render.FlowContainer.FlowDirectionEnum.TopDown);
                    dynamicRowsBox.MatchParentWidth = true;
                    dynamicRowsBox.Name = "DynamicRows";
                }

                if ((_subtotal != null && _subtotal.Position == Subtotal.PositionEnum.Before) ||
                    (tb != null && _subtotal == null))
                {
                    RenderSubtotal(dynamicRowsBox, context, hidden, (_subtotal == null) ? tb : null);
                }

                while (context1.GroupIndex < context1.GroupCount)
                {
                    Rdl.Render.FlowContainer rowGroupBox = dynamicRowsBox.AddFlowContainer(this, Style, context1, Rdl.Render.FlowContainer.FlowDirectionEnum.LeftToRight);
                    rowGroupBox.Name = "DynamicRow";

                    if (tb != null)
                        tb.LinkedToggles.Add(new Toggle(rowGroupBox, tb));

                    Rdl.Render.FixedContainer rowGroupHeader = rowGroupBox.AddFixedContainer(this, Style, context1);
                    rowGroupHeader.Name = "DynamicRowHeader";
                    rowGroupHeader.Width = Width.points;
                    rowGroupHeader.MatchParentHeight = true;
                    //rowGroupHeader.Height = FindMatrix(this).Rows.Height;
                    rowGroupHeader.CanGrowHorizonally = false;
                    _reportItems.Render(rowGroupHeader, context1);

                    if (RenderNext != null)
                    {
                        if (RenderNext is RowGrouping)
                        {
                            //Rdl.Render.FlowContainer contentBox = rowGroupBox.AddFlowContainer(this, Style, context1, Rdl.Render.FlowContainer.FlowDirectionEnum.TopDown);
                            //contentBox.Name = "DynamicRowContent";
                            ((RowGrouping)RenderNext).Render(rowGroupBox, context1);
                        }
                        else if (RenderNext is ColumnGrouping)
                            ((ColumnGrouping)RenderNext).Render(rowGroupBox, FindMatrix(this).Context, context1, 0);
                        else
                            RenderNext.Render(rowGroupBox, context1);

                        //// Intersect the rows in the current column grouping with the rows in the 
                        //// row grouping and if there is anything left then render the 
                        //// MatrixRows.
                        //Rdl.Runtime.Context tmpContext = context1.Intersect(columnContext);
                    }

                    context1.LinkToggles();
                    context1.NextGroup();
                }

                if (_subtotal != null && _subtotal.Position == Subtotal.PositionEnum.After)
                    RenderSubtotal(box, context, hidden, null);
            }
        }


        private void RenderSubtotal(Rdl.Render.Container box, Rdl.Runtime.Context context, bool hidden, TextBox tb)
        {
            Rdl.Render.FlowContainer totalBox = box.AddFlowContainer(this, Style, context, Rdl.Render.FlowContainer.FlowDirectionEnum.LeftToRight);
            totalBox.Name = "RowTotal";
            if (tb != null)
                tb.LinkedToggles.Add(new Toggle(totalBox, tb, Enums.ToggleDirectionEnum.negative));

            Rdl.Render.FixedContainer totalHeader = totalBox.AddFixedContainer(this, Style, context);
            totalHeader.Name = "RowTotalHeader";
            totalHeader.Width = TotalWidth;
            totalHeader.MatchParentHeight = true;
            //totalHeader.Height = FindMatrix(this).Rows.Height;
            totalHeader.CanGrowHorizonally = false;

            if (_subtotal != null)
                _subtotal.ReportItems.Render(totalHeader, context);

            // Render the details.
            MatrixElement m = RenderNext;
            while (m is RowGrouping)
                m = ((RowGrouping)m).RenderNext;

            if (m is ColumnGrouping)
                ((ColumnGrouping)m).Render(totalBox, FindMatrix(this).Context, context, 0);
            else
                m.Render(totalBox, context);
        }
    }
}
