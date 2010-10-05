using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Rdl.Engine;

namespace Rdl.Engine.Matrix
{
    class Cell : ReportElement
    {
        ReportItems _reportItems = null;
        int _colIndex = 0;

        public Cell(XmlNode node, ReportElement parent, int colIndex)
            : base(node, parent)
        {
            _colIndex = colIndex;
            _container = true;
            _cell = true;
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "reportitems":
                    _reportItems = new ReportItems(attr, this);
                    break;
                default:
                    break;
            }
        }

        internal override void Render(Rdl.Render.Container box, Rdl.Runtime.Context context)
        {
            if (_reportItems != null)
                _reportItems.Render(box, context);
        }
    }
}
