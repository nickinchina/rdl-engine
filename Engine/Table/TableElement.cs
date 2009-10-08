using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine.Table
{
    abstract class TableElement : ReportElement
    {
        public TableElement(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        internal override void Render(Rdl.Render.Container box, Rdl.Runtime.Context context)
        {
            throw new Exception("Not Implemented");
        }
    }
}
