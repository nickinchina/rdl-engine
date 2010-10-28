using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine
{
    class List : DataRegion
    {
        Grouping _grouping = null;
        private List<SortBy> _sortBy = null;
        ReportItems _reportItems = null;
        string _dataInstanceName = string.Empty;

        public List(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
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
                    _sortBy = new List<SortBy>();
                    foreach (XmlNode child in attr.ChildNodes)
                        _sortBy.Add(new SortBy(child, this));
                    break;
                case "reportitems":
                    _reportItems = new ReportItems(attr, this);
                    break;
                case "datainstancename":
                    _dataInstanceName = attr.InnerText;
                    break;
                case "datainstanceelementoutput":
                    _dataElementOutput = (Enums.DataElementOutputEnum)Enum.Parse(typeof(Enums.DataElementOutputEnum), attr.InnerText, true);
                    break;
                default:
                    break;
            }
        }

        protected override void Render1(Rdl.Render.Container parentBox, Rdl.Runtime.Context context, bool visible)
        {
            base.Render1(parentBox, context, visible);

            if (visible)
            {
                _box = parentBox.AddFlowContainer(this, Style, _context, Rdl.Render.FlowContainer.FlowDirectionEnum.TopDown);
                _box.Name = _name;
            }

            _context = new Rdl.Runtime.Context(
                _context,
                null,
                null,
                _grouping,
                _sortBy);
        }

        protected override void Render2(Rdl.Runtime.Context context)
        {
            base.Render2(context);
     
            while (_context.GroupIndex < _context.GroupCount)
            {
                Rdl.Render.FixedContainer cell = null;
                if (_box != null && _dataElementOutput == Enums.DataElementOutputEnum.Auto)
                {
                    bool first = (cell == null);
                    cell = ((Rdl.Render.Container)_box).AddFixedContainer(this, Style, context);
                    cell.Name = "ListItem";
                    cell.MatchParentWidth = true;
                    // If this isn't the first group item and PageBreakAtStart then break before the container
                    if (!first && _grouping != null && _grouping.PageBreakAtStart)
                        cell.PageBreakBefore = _grouping.PageBreakAtStart;
                    // If this isn't the last group item and PageBreakAtEnd then break after the container
                    if (_context.GroupIndex < _context.GroupCount - 1 && _grouping != null && _grouping.PageBreakAtEnd)
                        cell.PageBreakAfter = _grouping.PageBreakAtEnd;
                }

                _reportItems.Render(cell, _context);

                _context.LinkToggles();
                _context.NextGroup();
            }
        }
    }
}
