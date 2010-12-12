using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine.Chart
{
    class StaticCategory : CategoryGrouping
    {
        private List<Expression> _labels = new List<Expression>();

        public StaticCategory(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            base.ParseAttribute(attr);
            switch (attr.Name.ToLower())
            {
                case "staticmember":
                    foreach (XmlNode child in attr.ChildNodes)
                        _labels.Add(new Expression(child.FirstChild, this));
                    break;
                default:
                    break;
            }
        }

        public override Categories GetCategories(Rdl.Runtime.Context context, Category parentCategory)
        {
            List<Category> result = new List<Category>();

            int i = 0;
            foreach (Expression exp in _labels)
            {
                string value = exp.ExecAsString(context);
               result.Add(new Category(context, this, parentCategory, value, i++));
            }

            return new Categories(result);
        }
    }
}
