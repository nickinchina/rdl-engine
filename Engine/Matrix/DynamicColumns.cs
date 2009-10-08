using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Rdl.Engine;

namespace Rdl.Engine.Matrix
{
    class DynamicColumns : ColumnGrouping
    {
        Grouping _grouping = null;
        List<SortBy> _sorting = new List<SortBy>();
        Subtotal _subtotal = null;
        ReportItems _reportItems = null;
        Visibility _visibility = Visibility.Visible;
        Dictionary<int, List<Rdl.Render.TextElement>> _headerTextElements = new Dictionary<int, List<Rdl.Render.TextElement>>();

        public DynamicColumns(XmlNode node, ReportElement parent)
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


        internal override int RenderHeader(Rdl.Render.Container box, Rdl.Runtime.Context context, int column)
        {
            Rdl.Runtime.Context context1 = context.GetChildContext(
                null,
                null,
                _grouping,
                _sorting);

            bool hidden = false;
            if (_visibility != null && _visibility.ToggleItem == null)
                hidden = _visibility.IsHidden(context1);

            TextBox tb = Report.FindToggleItem(_visibility);

            if ((_subtotal != null && _subtotal.Position == Subtotal.PositionEnum.Before) ||
                (tb != null && _subtotal == null))
                column = RenderSubTotalHeader(box, context, hidden, (_subtotal == null) ? tb : null, column);

            while (context1.GroupIndex < context1.GroupCount && !hidden)
            {
                Rdl.Render.FlowContainer cell = box.AddFlowContainer(this, Style, context1, Rdl.Render.FlowContainer.FlowDirectionEnum.TopDown);
                cell.Name = "DynamicColumnHeader";
                cell.MatchParentHeight = true;

                Rdl.Render.FixedContainer contentBox = cell.AddFixedContainer(this, Style, context1);
                contentBox.Name = "DynamicColumnContent";
                contentBox.Width = FindMatrix(this).ColumnWidth;
                contentBox.Height = _height.points;
                contentBox.MatchParentHeight = true;
                _reportItems.Render(contentBox, context1);

                if (tb != null)
                    tb.LinkedToggles.Add(new Toggle(cell, tb));

                // Save the text elements so we can link toggles in Render
                foreach (TextBox tb2 in context1.TextBoxes.Values)
                {
                    if (!_headerTextElements.ContainsKey(column))
                        _headerTextElements[column] = new List<Rdl.Render.TextElement>();
                    _headerTextElements[column].Add(tb2.TextElement);
                }

                if (RenderNext != null && RenderNext is ColumnGrouping)
                {
                    Rdl.Render.FlowContainer columnHeader = cell.AddFlowContainer(this, Style, context1, Rdl.Render.FlowContainer.FlowDirectionEnum.LeftToRight);
                    columnHeader.Name = "ColumnGrouping";
                    columnHeader.ContextBase = true;

                    column = ((ColumnGrouping)RenderNext).RenderHeader(columnHeader, context1, column);
                }
                else
                {
                    contentBox.MatchParentHeight = true;
                    column++;
                }

                context1.LinkToggles();
                context1.NextGroup();
            }

            if (_subtotal != null && _subtotal.Position == Subtotal.PositionEnum.After)
                column = RenderSubTotalHeader(box, context, hidden, tb, column);

            return column;
        }

        private int RenderSubTotalHeader(Rdl.Render.Container box, Rdl.Runtime.Context context, bool hidden, TextBox tb, int column )
        {
            Rdl.Render.FixedContainer cell = null;

            if (box != null && !hidden)
            {
                cell = box.AddFixedContainer(this, Style, context);
                cell.Name = "SubtotalColumnHeader";
                //cell.Height = TotalHeight;
                cell.Width = FindMatrix(this).ColumnWidth;
                cell.MatchParentHeight = true;
                //cell.CanGrowVertically = false;

                if (tb != null)
                    tb.LinkedToggles.Add(new Toggle(cell, tb, Enums.ToggleDirectionEnum.negative));
            }

            if (_subtotal != null)
                _subtotal.ReportItems.Render(cell, context);
            return column + 1;
        }

        
        internal override int Render(Rdl.Render.Container box, Rdl.Runtime.Context context, Rdl.Runtime.Context rowContext, int column)
        {
            Rdl.Runtime.Context context1 = context.GetChildContext(null, null, _grouping, _sorting);

            bool hidden = false;
            if (_visibility != null && _visibility.ToggleItem == null)
                hidden = _visibility.IsHidden(context1);

            if (!hidden)
            {
                TextBox tb = Report.FindToggleItem(_visibility);

                if ((_subtotal != null && _subtotal.Position == Subtotal.PositionEnum.Before) ||
                    (tb != null && _subtotal == null))
                    column = RenderSubtotal(box, context.Intersect(rowContext), hidden, (_subtotal == null) ? tb : null, column);

                Rdl.Render.FlowContainer groupBox = null;
                if (box != null)
                {
                    groupBox = box.AddFlowContainer(this, Style, context1, Rdl.Render.FlowContainer.FlowDirectionEnum.LeftToRight);
                    groupBox.Name = "DynamicColumnGroup";
                    groupBox.MatchParentHeight = true;
                }

                while (context1.GroupIndex < context1.GroupCount)
                {
                    Rdl.Render.FixedContainer groupEntryBox = null;
                    if (groupBox != null)
                    {
                        groupEntryBox = groupBox.AddFixedContainer(this, Style, context1);
                        groupEntryBox.MatchParentHeight = true;
                        if (tb != null)
                            tb.LinkedToggles.Add(new Toggle(groupEntryBox, tb));
                    }

                    // Put the column header text boxes into the column context and 
                    // set the text element for any textboxes in the header of the column so that
                    // cells toggled on the header columns link to the correct boxes.
                    if (_headerTextElements.ContainsKey(column))
                        foreach (Rdl.Render.TextElement te in _headerTextElements[column])
                        {
                            context1.TextBoxes[((TextBox)te.ReportElement).Name] = (TextBox)te.ReportElement;
                            ((TextBox)te.ReportElement).Box = te;
                        }

                    // Render the details.
                    if (RenderNext != null)
                        if (RenderNext is ColumnGrouping)
                            column = ((ColumnGrouping)RenderNext).Render(groupEntryBox, context1, rowContext, column);
                        else
                        {
                            groupBox.Height = FindMatrix(this).Rows.Height;
                            RenderNext.Render(groupEntryBox, context1.Intersect(rowContext));
                            column++;
                        }

                    // Link the toggles at this context level with the linked elements contained.
                    context1.LinkToggles();
                    context1.NextGroup();
                }

                if (_subtotal != null && _subtotal.Position == Subtotal.PositionEnum.After)
                    column = RenderSubtotal(box, context.Intersect(rowContext), hidden, null, column);
            }

            return column;
        }

        private int RenderSubtotal(Rdl.Render.Container box, Rdl.Runtime.Context context, bool hidden, TextBox tb, int column)
        {
            Rdl.Render.FixedContainer cell = null;

            if (!hidden && box != null)
            {
                cell = box.AddFixedContainer(this, Style, context);
                cell.Name = "DynamicColumnSubtotal";
                //cell.Width = FindMatrix(this).ColumnWidth;
                cell.Height = Height.points;
                cell.MatchParentHeight = true;
                if (tb != null)
                    tb.LinkedToggles.Add(new Toggle(cell, tb, Enums.ToggleDirectionEnum.negative));

                // Render the details.
                MatrixElement m = RenderNext;
                while (m is ColumnGrouping)
                    m = ((ColumnGrouping)m).RenderNext;

                m.Render(cell, context);
            }
            return column + 1;
        }
    }
}
