using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RdlEngine
{
    public class Image : ReportElement
    {
        protected enum ImageSourceEnum
        {
            External,
            Embedded,
            Database
        }
        public enum ImageSizingEnum
        {
            Autosize,
            Fit,
            FitProportional,
            Clip,
        }

        protected ImageSourceEnum _source = ImageSourceEnum.External;
        internal Expression _value = null;
        internal Expression _mIMEType = null;
        protected ImageSizingEnum _sizing = ImageSizingEnum.Autosize;

        public Image(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "source":
                    _source = (ImageSourceEnum)Enum.Parse(typeof(ImageSourceEnum), attr.InnerText, true);
                    break;
                case "value":
                    _value = new Expression(attr, this);
                    break;
                case "mimetype":
                    _mIMEType = new Expression(attr, this);
                    break;
                case "sizing":
                    _sizing = (ImageSizingEnum)Enum.Parse(typeof(ImageSizingEnum), attr.InnerText, true);
                    break;
                default:
                    break;
            }
        }

        public Byte[] Value
        {
            get
            {
                return null;
            }
        }

        public string MIMEType
        {
            get { return (_mIMEType==null) ? string.Empty : _mIMEType.ExecAsString(); }
        }

        public virtual ImageSizingEnum Sizing
        {
            get { return _sizing; }
        }
    }
}
