using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine
{
    public class BackgroundImage : Image
    {
        public enum ImageRepeatEnum
        {
            Repeat,
            NoRepeat,
            RepeatX,
            RepeatY
        }

        private Expression _backgroundRepeat = null;
        private static int _backgroundImageIndex = 0;

        public BackgroundImage(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
            //_sizing = ImageSizingEnum.Clip;

            // Add the background image to the ReportItem list so that we can find it later by name.
            if (_name == string.Empty)
                _name = "BackgroundImage_" + (_backgroundImageIndex++).ToString();
            Report.ReportItems.Add(_name, this);
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

        public ImageRepeatEnum ImageRepeat(Rdl.Runtime.Context context)
        {
            if (_backgroundRepeat != null)
            {
                string val = _backgroundRepeat.ExecAsString(context);
                return (ImageRepeatEnum)Enum.Parse(typeof(ImageRepeatEnum), val, true);
            }
            return ImageRepeatEnum.Repeat;
        }
    }
}
