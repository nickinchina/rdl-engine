using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Drawing;

namespace Rdl.Engine.Chart
{
    class Title : ChartElement
    {
        public enum PositionEnum
        {
            Center,
            Near,
            Far
        };

        private Expression _caption = null;
        private PositionEnum _position = PositionEnum.Center;

        public Title(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            base.ParseAttribute(attr);
            switch (attr.Name.ToLower())
            {
                case "caption":
                    _caption = new Expression(attr, this);
                    break;
                case "style":
                    _style = new Style(attr, this);
                    break;
                case "position":
                    _position = (PositionEnum)Enum.Parse(typeof(PositionEnum), attr.InnerText, true);
                    break;
                default:
                    break;
            }
        }

        public string Caption(Rdl.Runtime.Context context)
        {
            if (_caption == null)
                return null;
            else
                return _caption.ExecAsString(context);
        }

        public PositionEnum Position
        {
            get { return _position; }
        }

        public void DrawAtBottom(Graphics g, Rdl.Runtime.Context context, 
            ref int l, ref int t, ref int w, ref int h)
        {
            string title = Caption(context);
            if (title != null && title != string.Empty)
            {
                Font titleFont = Style.GetWindowsFont(context);
                SizeF stringSize = g.MeasureString(title, titleFont);
                int xPos = 0;
                if (Position == Title.PositionEnum.Center)
                    xPos = (w / 2) - ((int)stringSize.Width / 2);
                if (Position == Title.PositionEnum.Far)
                    xPos = w - (int)stringSize.Width;
                g.DrawString(title, titleFont,
                    new SolidBrush(Style.W32Color(Style.Color(context))),
                    l + xPos, t + h - (int)stringSize.Height);
                h -= (int)stringSize.Height;
            }
        }

        public void DrawAtTop(Graphics g, Rdl.Runtime.Context context,
            ref int l, ref int t, ref int w, ref int h)
        {
            string title = Caption(context);
            if (title != null && title != string.Empty)
            {
                Font titleFont = Style.GetWindowsFont(context);
                SizeF stringSize = g.MeasureString(title, titleFont);
                int xPos = 0;
                if (Position == Title.PositionEnum.Center)
                    xPos = (w / 2) - ((int)stringSize.Width / 2);
                if (Position == Title.PositionEnum.Far)
                    xPos = w - (int)stringSize.Width;
                g.DrawString(title, titleFont, new SolidBrush(Style.W32Color(Style.Color(context))),
                     l + xPos, t);
                t += (int)stringSize.Height;
                h -= (int)stringSize.Height;
            }
        }

        public void DrawAtLeft(Graphics g, Rdl.Runtime.Context context,
            ref int l, ref int t, ref int w, ref int h)
        {
            string title = Caption(context);
            if (title != null && title != string.Empty)
            {
                Font titleFont = Style.GetWindowsFont(context);
                SizeF stringSize = g.MeasureString(title, titleFont, 0, new StringFormat(StringFormatFlags.DirectionVertical));
                int yPos = 0;
                if (Position == Title.PositionEnum.Center)
                    yPos = (h / 2) - ((int)stringSize.Height / 2);
                if (Position == Title.PositionEnum.Far)
                    yPos = h - (int)stringSize.Height;
                g.DrawString(title, titleFont, new SolidBrush(Style.W32Color(Style.Color(context))),
                    l, t + yPos, new StringFormat(StringFormatFlags.DirectionVertical));
                l += (int)stringSize.Width;
                w -= (int)stringSize.Width;
            }
        }
    }
}
