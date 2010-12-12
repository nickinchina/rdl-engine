using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Rdl.Engine;

namespace Rdl.Engine.Table
{
    class Details : TableElement
    {
        private List<Row> _tableRows = new List<Row>();
        private Grouping _grouping = null;
        private List<SortBy> _sortBy = null;
        private Visibility _visibility = Visibility.Visible;

        public Details(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "tablerows":
                    foreach(XmlNode child in attr.ChildNodes)
                        _tableRows.Add(new Row(child, this));
                    break;
                case "grouping":
                    _grouping = new Grouping(attr, this);
                    break;
                case "sorting":
                    _sortBy = new List<SortBy>();
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

        internal override void Render(Rdl.Render.Container box, Rdl.Runtime.Context context)
        {
            bool hidden = false;

            context = new Rdl.Runtime.Context(
                context,
                null,
                null,
                _grouping,
                _sortBy);

            TextBox tb = FindToggleItem(_visibility);
            if (_visibility != null && _visibility.ToggleItem == null)
                hidden = _visibility.IsHidden(context); ;

            // Loop through all of the rows in the data context
            decimal top = 0;
            while (true)
            {
                if (_grouping == null && context.CurrentRow == null)
                    break;
                if (_grouping != null && context.GroupIndex >= context.GroupCount)
                    break;

                foreach (Row tr in _tableRows)
                {
                    Rdl.Render.FixedContainer rowBox = null;
                    if (box != null && !hidden)
                    {
                        rowBox = box.AddFixedContainer(this, Style, context);
                        rowBox.Name = "RowBox";
                        rowBox.Top = top;
                        rowBox.Width = box.Width;
                        rowBox.ContextBase = true;
                    
                        if (tb != null)
                            tb.LinkedToggles.Add(new Toggle(rowBox, tb));
                    }

                    tr.Render(rowBox, context);

                    if (box != null && !hidden)
                    {
                        top += rowBox.Height;
                        box.Height = top;
                    }
                }

                context.LinkToggles();
                if (_grouping == null)
                    context.MoveNext();
                else
                    context.NextGroup();
            }
        }
    }
}
