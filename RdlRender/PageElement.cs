using System;
using System.Collections.Generic;
using System.Text;

namespace RdlRender
{
    public class PageElement : Element
    {
        protected List<Container> _childElements = new List<Container>();

        internal PageElement()
            : base(null, null, null)
        {
        }

        public List<Container> Children
        {
            get { return _childElements; }
        }

        internal FixedContainer AddFixedContainer()
        {
            FixedContainer b = new FixedContainer(null, null, null);
            _childElements.Add(b);
            return b;
        }
    }
}
