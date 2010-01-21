using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine.Chart
{
    class CategoryGrouping : ChartElement
    {
        protected CategoryGrouping _nextGrouping = null;

        public static CategoryGrouping GetCategoryGrouping(XmlNode node, ReportElement parent)
        {
            XmlNamespaceManager ns = new XmlNamespaceManager(node.OwnerDocument.NameTable);
            ns.AddNamespace("rdl", parent.Report.XmlNameSpace);

            if (node.SelectSingleNode("rdl:DynamicCategories", ns) != null)
                return new DynamicCategory(node, parent);
            else
                return new StaticCategory(node, parent);
        }

        public CategoryGrouping(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            base.ParseAttribute(attr);
            switch (attr.Name.ToLower())
            {
                case "dynamiccategories":
                case "staticcategories":
                    foreach (XmlNode child in attr.ChildNodes)
                        ParseAttribute(child);
                    break;
                default:
                    break;
            }
        }

        public CategoryGrouping NextGrouping
        {
            get { return _nextGrouping; }
            set { _nextGrouping = value; }
        }

        public virtual Categories GetCategories(Rdl.Runtime.Context context, Category parentCategory)
        {
            return null;
        }
    }


    class Category
    {
        private string _value;
        private Categories _categories = null;
        private Rdl.Runtime.Context _context = null;
        private Rdl.Runtime.ContextState _contextState = null;
        private Category _parentCategory = null;
        private int _categoryIndex = 0;

        public Category(Rdl.Runtime.Context context, CategoryGrouping grouping, Category parentCategory, string value)
        {
            _context = context;
            _contextState = context.ContextState;
            _parentCategory = parentCategory;
            _value = value;
            if (grouping.NextGrouping != null)
                _categories = grouping.NextGrouping.GetCategories(context, this);
        }

        public Category(Rdl.Runtime.Context context, CategoryGrouping grouping, Category parentCategory, string value, int index)
            :
            this(context, grouping, parentCategory, value)
        {
            _categoryIndex = index;
        }


        public Categories Categories
        {
            get { return _categories; }
        }

        public string Value
        {
            get { return _value; }
        }

        public Rdl.Runtime.Context Context
        {
            get
            {
                _context.ContextState = _contextState;
                return _context;
            }
        }

        public virtual int CategoryIndex
        {
            get
            {
                if (_categoryIndex > 0 || _parentCategory == null)
                    return _categoryIndex;
                else
                    return _parentCategory.CategoryIndex;
            }
        }
    }

    class Categories
    {
        private List<Category> _values = null;

        public Categories(List<Category> values)
        {
            _values = values;
        }

        public IEnumerator<Category> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        public int Count
        {
            get { return _values.Count; }
        }

        public Categories LeafCategories
        {
            get
            {
                List<Category> leafCategories = new List<Category>();
                BuildLeafCategories(this, leafCategories);
                return new Categories(leafCategories);
            }
        }

        private void BuildLeafCategories(Categories categories, List<Category> leafCategories)
        {
            foreach (Category cat in categories)
                if (cat.Categories == null)
                    leafCategories.Add(cat);
                else
                    BuildLeafCategories(cat.Categories, leafCategories);
        }
    }
}
