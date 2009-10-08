using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Rdl.Engine;

namespace Rdl.Engine.Table
{
    class Group : TableElement
    {
        private Grouping _grouping = null;
        private List<SortBy> _sortBy = null;
        private Header _header = null;
        private Footer _footer = null;
        private Visibility _visibility = Visibility.Visible;
        private TableElement _details = null;
        private Rdl.Render.FlowContainer _box = null;

        public Group(XmlNode node, ReportElement parent)
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
                    _header = new Header(attr, this);
                    break;
                case "footer":
                    _footer = new Footer(attr, this);
                    break;
                case "visibility":
                    _visibility = new Visibility(attr, this);
                    break;
                default:
                    break;
            }
        }

        public TableElement Details
        {
            get { return _details; }
            set { _details = value; }
        }

        internal override void Render(Rdl.Render.Container box, Rdl.Runtime.Context context)
        {
            Rdl.Render.FlowContainer headerBox = null;
            Rdl.Render.FlowContainer detailsBox = null;
            Rdl.Render.FlowContainer footerBox = null;
            Rdl.Render.FlowContainer groupRow = null;

            context = new Rdl.Runtime.Context(
                context,
                null,
                null,
                _grouping,
                _sortBy);

            TextBox tb = Report.FindToggleItem(_visibility);

            if (box != null && !_visibility.IsHidden(context))
            {
                _box = box.AddFlowContainer(this, Style, context, Rdl.Render.FlowContainer.FlowDirectionEnum.TopDown);
                _box.Width = box.Width;
                _box.Name = "TableGroup";

                if (tb != null)
                    tb.LinkedToggles.Add(new Toggle(_box, tb));
            }

            // Render the header
            decimal groupTop = 0;
            while (context.GroupIndex < context.GroupCount)
            {
                if (_box != null)
                {
                    groupRow = _box.AddFlowContainer(this, Style, context, Rdl.Render.FlowContainer.FlowDirectionEnum.TopDown);
                    groupRow.Width = _box.Width;
                    groupRow.Top = groupTop;
                    groupRow.Name = "GroupRrow";
                    groupRow.ContextBase = true;
                }

                if (_header != null)
                {
                    if (_box != null)
                    {
                        headerBox = groupRow.AddFlowContainer(this, _header.Style, context, Rdl.Render.FlowContainer.FlowDirectionEnum.TopDown);
                        headerBox.Top = 0;
                        headerBox.Width = groupRow.Width;
                        headerBox.Name = "GroupHeader";
                    }

                    _header.Render(headerBox, context);
                }

                // Create a box to hold the details and tie that
                // box to any repeat lists referencing these details.
                if (_details != null && groupRow != null)
                {
                    detailsBox = groupRow.AddFlowContainer(this, _details.Style, context, Rdl.Render.FlowContainer.FlowDirectionEnum.TopDown);
                    detailsBox.Top = (headerBox == null) ? 0 : headerBox.Height;
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
                    _details.Render(detailsBox, context);

                // Render the footer.
                if (_footer != null)
                {
                    if (groupRow != null)
                    {
                        footerBox = groupRow.AddFlowContainer(this, _footer.Style, context, Rdl.Render.FlowContainer.FlowDirectionEnum.TopDown);
                        footerBox.Name = "GroupFooter";
                        footerBox.Top = ((headerBox == null) ? 0 : headerBox.Height) +
                            ((detailsBox == null) ? 0 : detailsBox.Height);
                        footerBox.Width = _box.Width;
                    }
                    context.RowIndex = 0;
                    _footer.Render(footerBox, context);
                }

                if (groupRow != null)
                    groupTop += groupRow.Height;

                context.LinkToggles();
                context.NextGroup();
            }
        }
    }
}
