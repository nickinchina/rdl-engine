using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RdlEngine
{
    class TableDetails : ReportElement
    {
        private List<TableRow> _tableRows = new List<TableRow>();
        private Grouping _grouping = null;
        private List<SortBy> _sortBy = null;
        private Visibility _visibility = Visibility.Visible;

        public TableDetails(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "tablerows":
                    foreach(XmlNode child in attr.ChildNodes)
                        _tableRows.Add(new TableRow(child, this));
                    break;
                case "grouping":
                    _grouping = new Grouping(attr, this);
                    break;
                case "sorting":
                    foreach (XmlNode child in attr.ChildNodes)
                        _sortBy.Add(new SortBy(child, this));
                    break;
                case "visibility":
                    _visibility = new Visibility(attr, this);
                    break;
                default:
                    break;
            }
        }

        internal override void Parse2()
        {
            base.Parse2();

            if (_visibility.ToggleItem != null)
            {
                ReportItem toggleItem = Report.ReportItems[_visibility.ToggleItem];
                if (toggleItem != null)
                {
                    if (toggleItem is TextBox)
                        ((TextBox)toggleItem).ToggleList.Add(this);
                    else
                        throw new Exception("Toggle items are only allowed to refer to text boxes\n");
                }
                else
                    throw new Exception("Toggle item " + _visibility.ToggleItem + " not found");
            }
        }

        public override void Render(RdlRender.Container box)
        {
            bool hidden = false;

            base.Render(box);

            RdlRuntime.Context parentContext = ParentContext(null);
            _context = new RdlRuntime.Context(
                parentContext,
                null,
                null,
                _grouping,
                _sortBy);

            if (_visibility != null && _visibility.ToggleItem == null)
                hidden = _visibility.IsHidden;

            // Loop through all of the rows in the data context
            decimal top = 0;
            while (true)
            {
                if (_grouping == null && _context.CurrentRow == null)
                    break;
                if (_grouping != null && _context.GroupIndex >= _context.GroupCount)
                    break;

                foreach (TableRow tr in _tableRows)
                {
                    RdlRender.FixedContainer rowBox = null;
                    if (box != null && !hidden)
                    {
                        rowBox = box.AddFixedContainer(this, Style);
                        rowBox.Name = "RowBox";
                        rowBox.Top = top;
                        rowBox.Width = box.Width;
                        rowBox.ContextBase = true;
                    }

                    tr.Render(rowBox);

                    if (box != null && !hidden)
                    {
                        top += rowBox.Height;
                        box.Height = top;
                    }
                }

                if (_grouping == null)
                    _context.MoveNext();
                else
                    _context.NextGroup();
            }
        }
    }
}
