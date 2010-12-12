using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RdlEngine
{
    public class BackgroundImage : Image
    {
        public enum BackgroundRepeatEnum
        {
            Repeat,
            NoRepeat,
            RepeatX,
            RepeatY
        }

        private Expression _backgroundRepeat = null;

        public BackgroundImage(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
            _sizing = ImageSizingEnum.Clip;
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            base.ParseAttribute(attr);
            switch (attr.Name.ToLower())
            {
                case "backgroundrepeat":
                    _backgroundRepeat = new Expression(attr, this);
                    break;
                default:
                    break;
            }
        }

        public BackgroundRepeatEnum BackgroundRepeat
        {
            get
            {
                if (_backgroundRepeat != null)
                {
                    string val = _backgroundRepeat.ExecAsString();
                    return (BackgroundRepeatEnum)Enum.Parse(typeof(BackgroundRepeatEnum), val, true);
                }
                return BackgroundRepeatEnum.Repeat;
            }
        }
    }
}
