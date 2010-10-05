using System;
using System.Collections.Generic;
using System.Text;
using Rdl.Engine;
using System.Xml;

namespace Rdl.Engine.Matrix
{
    abstract class MatrixElement : ReportElement
    {
        protected MatrixElement _renderNext = null;

        public MatrixElement(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        public MatrixElement RenderNext
        {
            get 
            {
                MatrixElement m = this;
                while (m._renderNext == null && m.Parent != null && !(m.Parent is Matrix))
                    m = m.Parent as MatrixElement;
                return m._renderNext; 
            }
            set 
            { 
                _renderNext = value;
            }
        }

        internal override void Render(Rdl.Render.Container box, Rdl.Runtime.Context context)
        {
        }
    }
}
