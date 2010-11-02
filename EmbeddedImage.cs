using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine
{
    class EmbeddedImage : ReportElement
    {
        private string _name = string.Empty;
        private string _mimeType = null;
        private string _imageData = null;

        public EmbeddedImage(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "name":
                    _name = attr.InnerText;
                    break;
                case "mimetype":
                    _mimeType = attr.InnerText;
                    break;
                case "imagedata":
                    _imageData = attr.InnerText;
                    break;
                default:
                    break;
            }
        }

        public string Name
        {
            get { return _name; }
        }

        public string MIMEType
        {
            get { return _mimeType; }
        }

        public byte[] ImageData
        {
            get { return Convert.FromBase64String(_imageData); }
        }
    }
}
