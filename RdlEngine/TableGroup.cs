using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RdlEngine
{
    class TableGroup : ReportElement
    {
        private Grouping _grouping = null;
        private List<SortBy> _sortBy = null;
        private TableHeader _header = null;
        private TableFooter _footer = null;
        private Visibility _visibility = Visibility.Visible;
        private ReportElement _details = null;
        private RdlRender.FlowContainer _box = null;

        public TableGroup(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "grouping":
                    _grouping = new Grouping(attr, this);
                    break;
                case "sorting":
                    _sortBy = new List<SortBy>();
                    foreach(XmlNode child in attr.ChildNodes)
                        _sortBy.Add(new SortBy(child, this));
                    break;
                case "header":
                    _header = new TableHeader(attr, this);
                    break;
                case "footer":
                    _footer = new TableFooter(attr, this);
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

        public ReportElement Details
        {
            get { return _details; }
            set { _details = value; }
        }

        public override void Render(RdlRender.Container box)
        {
            RdlRender.FixedContainer headerBox = null;
            RdlRender.FlowContainer detailsBox = null;
            RdlRender.FixedContainer footerBox = null;
            RdlRender.FlowContainer groupRow = null;

            _context = new RdlRuntime.Context(
                ParentContext(null),
                null,
                null,
                _grouping,
                _sortBy);

            base.Render(box);

            if (box != null)
            {
                _box = box.AddFlowContainer(this, Style);
                _box.CanGrowVertically = true;
                _box.Width = box.Width;
                _box.Name = "TableGroup";
            }

            // Render the header
            decimal groupTop = 0;
            while (_context.GroupIndex < _context.GroupCount)
            {
                if (_box != null)
                {
                    groupRow = _box.AddFlowContainer(this, Style);
                    groupRow.CanGrowVertically = true;
                    groupRow.Width = _box.Width;
                    groupRow.Top = groupTop;
                    groupRow.Name = "GroupRrow";
                    groupRow.ContextBase = true;
                }

                if (_header != null)
                {
                    if (_box != null)
                    {
                        headerBox = groupRow.AddFixedContainer(this, _header.Style);
                        headerBox.Top = 0;
                        headerBox.CanGrowVertically = true;
                        headerBox.Width = groupRow.Width;
                        headerBox.Name = "GroupHeader";
                    }

                    _header.Render(headerBox);
                }

                // Create a box to hold the details and tie that
                // box to any repeat lists referencing these details.
                if (_details != null && groupRow != null)
                {
                    detailsBox = groupRow.AddFlowContainer(this, _details.Style);
                    detailsBox.Top = (headerBox==null)?0:headerBox.Height;
                    detailsBox.CanGrowVertically = true;
                    detailsBox.Width = groupRow.Width;
                    detailsBox.Name = "GroupDetails";

                    // If the header or footer are repeated, then add them to the repeat list of the details.
                    if (_header != null && _header.RepeatOnNewPage)
                        detailsBox.RepeatList.Add(headerBox);
                    if (_footer != null && _footer.RepeatOnNewPage)
                        detailsBox.RepeatList.Add(footerBox);
                }

                // Render the details.
                if (_details != null)
                    _details.Render(detailsBox);

                // Render the footer.
                if (_footer != null)
                {
                    if (groupRow != null)
                    {
                        footerBox = groupRow.AddFixedContainer(this, _footer.Style);
                        footerBox.Name = "GroupFooter";
                        footerBox.CanGrowVertically = true;
                        footerBox.Top = ((headerBox == null) ? 0 : headerBox.Height) +
                            ((detailsBox == null) ? 0 : detailsBox.Height);
                        footerBox.Width = _box.Width;
                    }
                    _context.RowIndex = 0;
                    _footer.Render(footerBox);
                }

                if (groupRow != null)
                    groupTop += groupRow.Height;

                _context.NextGroup();
            }
        }
    }
}
