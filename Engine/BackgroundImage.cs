/*-----------------------------------------------------------------------------------
This file is part of the SawikiSoft RDL Engine.
The SawikiSoft RDL Engine is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

The SawikiSoft RDL Engine is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
-----------------------------------------------------------------------------------*/
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
