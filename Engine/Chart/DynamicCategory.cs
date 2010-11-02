using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine.Chart
{
    class DynamicCategory : CategoryGrouping
    {
        private Grouping _grouping = null;
        private List<SortBy> _sorting = null;
        private Expression _label = null;

        public DynamicCategory(XmlNode node, ReportElement parent)
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
                    _sorting = new List<SortBy>();
                    foreach (XmlNode child in attr.ChildNodes)
                        _sorting.Add(new SortBy(child, this));
                    break;
                case "label":
                    _label = new Expression(attr, this);
                    break;
                default:
                    break;
            }
        }

        public override Categories GetCategories(Rdl.Runtime.Context context, Category parentCategory)
        {
            List<Category> result = new List<Category>();
            Rdl.Runtime.Context ctxt = context.GetChildContext(null, null, _grouping, _sorting);

            while (ctxt.GroupIndex < ctxt.GroupCount)
            {
                string value;
                if (_label != null && !_label.Empty)
                    value = _label.ExecAsString(ctxt);
                else
                    value = ctxt.CurrentGroupValue;
                result.Add(new Category(ctxt, this, parentCategory, value));

                ctxt.NextGroup();
            }

            return new Categories(result);
        }
    }
}
