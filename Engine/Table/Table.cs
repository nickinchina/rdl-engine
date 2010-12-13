/*-----------------------------------------------------------------------------------
This file is part of the SawikiSoft RDL Engine.
The SawikiSoft RDL Engine is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

The SawikiSoft RDL Engine is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
-----------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Rdl.Engine;

namespace Rdl.Engine.Table
{
    class Table : DataRegion
    {
        private enum DetailDataElementOutputEnum
        {
            Output,
            NoOutput
        };
        private List<Column> _tableColumns = new List<Column>();
        private Header _header = null;
        private List<Group> _tableGroups = new List<Group>();
        private TableElement _details = null;
        private Footer _footer = null;
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
                foreach (Column tc in _tableColumns)
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
                if (_details != null)
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
                        _tableColumns.Add(new Column(child, this));
                    break;
                case "header":
                    _header = new Header(attr, this);
                    break;
                case "tablegroups":
                    ReportElement parnt = this;
                    foreach (XmlNode child in attr.ChildNodes)
                    {
                        Group tg = new Group(child, parnt);
                        _tableGroups.Add(tg);
                        parnt = tg;
                    }
                    break;
                case "details":
                    _details = new Details(attr, this);
                    break;
                case "footer":
                    _footer = new Footer(attr, this);
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

        public List<Column> TableColumns
        {
            get { return _tableColumns; }
        }

        protected override void Render1(Rdl.Render.Container parentBox, Rdl.Runtime.Context context, bool visible)
        {
            base.Render1(parentBox, context, visible);
            if (visible && _context.Rows.Count > 0)
            {
                _box = parentBox.AddFlowContainer(this, Style, _context, Rdl.Render.FlowContainer.FlowDirectionEnum.TopDown);
                _box.Name = _name;
                _box.CanGrowHorizonally = false;
            }
        }

        protected override void Render2(Rdl.Runtime.Context context)
        {
            base.Render2(context);
            if (_box != null)
            {
                Rdl.Render.Container headerBox = null;
                Rdl.Render.Container detailsBox = null;
                Rdl.Render.Container footerBox = null;

                if (_header != null)
                {
                    headerBox = ((Rdl.Render.Container)_box).AddFlowContainer(this, _header.Style, _context, Rdl.Render.FlowContainer.FlowDirectionEnum.TopDown);
                    headerBox.Name = "TableHeader";
                }
                if (_details != null)
                {
                    detailsBox = ((Rdl.Render.Container)_box).AddFlowContainer(this, _details.Style, _context, Rdl.Render.FlowContainer.FlowDirectionEnum.TopDown);
                    detailsBox.Name = "TableDetails";
                }
                if (_footer != null)
                {
                    footerBox = ((Rdl.Render.Container)_box).AddFlowContainer(this, _footer.Style, _context, Rdl.Render.FlowContainer.FlowDirectionEnum.TopDown);
                    footerBox.Name = "TableFooter";
                }

                if (_header != null)
                    _header.Render(headerBox, _context);
                if (_details != null)
                {
                    if (detailsBox != null && headerBox != null)
                        detailsBox.Top = headerBox.Height;
                    _details.Render(detailsBox, _context);
                }
                if (_footer != null)
                {
                    if (footerBox != null)
                        if (detailsBox != null)
                            footerBox.Top = detailsBox.Top + detailsBox.Height;
                        else if (headerBox != null)
                            footerBox.Top = headerBox.Height;
                    _footer.Render(footerBox, _context);
                }

                // If the header or footer are reoeated, then add them to the repeat list of the details.
                if (_header != null && _header.RepeatOnNewPage && _box != null)
                    detailsBox.RepeatList.Add(headerBox);
                if (_footer != null && _footer.RepeatOnNewPage && _box != null)
                    detailsBox.RepeatList.Add(footerBox);
            }
        }
    }
}
