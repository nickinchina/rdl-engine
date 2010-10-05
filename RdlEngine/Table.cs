using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RdlEngine
{
    class Table : DataRegion
    {
        private enum DetailDataElementOutputEnum
        {
            Output,
            NoOutput
        };
        private List<TableColumn> _tableColumns = new List<TableColumn>();
        private TableHeader _header = null;
        private List<TableGroup> _tableGroups = new List<TableGroup>();
        private ReportElement _details = null;
        private TableFooter _footer = null;
        private bool _fillPage = false;
        private string _detailDataElementName = "Details";
        private string _detailDataCollectionName = "Details_Collection";
        private DetailDataElementOutputEnum _detailDataElementOutput = DetailDataElementOutputEnum.Output;

        public Table(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
            if (_width == null)
            {
                decimal width = 0;
                foreach (TableColumn tc in _tableColumns)
                    width += tc.Width.points;
                _width = new Size(width);
            }

            // Set up the hierarchy of table groups
            if (_tableGroups.Count > 0)
            {
                for (int i = 0; i < _tableGroups.Count - 1; i++)
                {
                    _tableGroups[i].Details = _tableGroups[i + 1];
                    _tableGroups[i + 1].Parent = _tableGroups[i];
                }
                _tableGroups[_tableGroups.Count - 1].Details = _details;
                _details.Parent = _tableGroups[_tableGroups.Count - 1];
                _details = _tableGroups[0];
            }
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            base.ParseAttribute(attr);
            switch (attr.Name.ToLower())
            {
                case "tablecolumns":
                    foreach( XmlNode child in attr.ChildNodes)
                        _tableColumns.Add(new TableColumn(child, this));
                    break;
                case "header":
                    _header = new TableHeader(attr, this);
                    break;
                case "tablegroups":
                    ReportElement parnt = this;
                    foreach (XmlNode child in attr.ChildNodes)
                    {
                        TableGroup tg = new TableGroup(child, parnt);
                        _tableGroups.Add(tg);
                        parnt = tg;
                    }
                    break;
                case "details":
                    _details = new TableDetails(attr, this);
                    break;
                case "footer":
                    _footer = new TableFooter(attr, this);
                    break;
                case "fillpage":
                    _fillPage = bool.Parse(attr.InnerText);
                    break;
                case "detaildataelementname":
                    _detailDataElementName = attr.InnerText;
                    break;
                case "detaildatacollectionname":
                    _detailDataCollectionName = attr.InnerText;
                    break;
                case "detaildataelementoutput":
                    _detailDataElementOutput = (DetailDataElementOutputEnum)Enum.Parse(typeof(DetailDataElementOutputEnum), attr.InnerText, true);
                    break;
                default:
                    break;
            }
        }

        public List<TableColumn> TableColumns
        {
            get { return _tableColumns; }
        }

        public override void Render(RdlRender.Container box)
        {
            RdlRender.FixedContainer headerBox = null;
            RdlRender.FlowContainer detailsBox = null;
            RdlRender.FixedContainer footerBox = null;

            base.Render(box);

            bool visible = true;
            if (_visibility != null && _visibility.IsHidden && _visibility.ToggleItem == null)
                visible = false;
            if (box != null && visible)
            {
                _box = box.AddFlowContainer(this, Style);
                _box.Name = _name;
                if (IsInCell)
                {
                    _box.Width = box.Width;
                    _box.MatchParentHeight = true;
                }
                else
                {
                    _box.Top = _top.points;
                    _box.Left = _left.points;
                    _box.Width = (_width == null) ? box.Width : _width.points;
                    _box.Height = _height.points;
                }
                _box.CanGrowHorizonally = false;

                if (_header != null)
                {
                    headerBox = _box.AddFixedContainer(this, _header.Style);
                    headerBox.Name = "TableHeader";
                }
                if (_details != null)
                {
                    detailsBox = _box.AddFlowContainer(this, _details.Style);
                    detailsBox.Name = "TableDetails";
                }
                if (_footer != null)
                {
                    footerBox = _box.AddFixedContainer(this, _footer.Style);
                    footerBox.Name = "TableFooter";
                }
            }

            if (_header != null)
                _header.Render(headerBox);
            if (_details != null)
            {
                if (detailsBox != null)
                    detailsBox.Top = headerBox.Height;
                _details.Render(detailsBox);
            }
            if (_footer != null)
            {
                if (footerBox != null)
                    footerBox.Top = detailsBox.Top + detailsBox.Height;
                _footer.Render(footerBox);
            }

            // If the header or footer are reoeated, then add them to the repeat list of the details.
            if (_header != null && _header.RepeatOnNewPage && _box != null)
                detailsBox.RepeatList.Add(headerBox);
            if (_footer != null && _footer.RepeatOnNewPage && _box != null)
                detailsBox.RepeatList.Add(footerBox);
        }
    }
}
