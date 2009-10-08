using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using RdlRender;

namespace RdlEngine
{
    abstract class ReportItem : ReportElement
    {
        protected string _name = string.Empty;
        protected Action _action = null;
        protected Size _top = Size.ZeroSize;
        protected Size _left = Size.ZeroSize;
        protected Size _height = Size.ZeroSize;
        protected Size _width = null;
        protected Int16 _zindex = 0;
        protected Visibility _visibility = Visibility.Visible;
        protected Expression _toolTop = null;
        protected Expression _label = null;
        protected string _linkToChild = null;
        protected Expression _boolmark = null;
        protected string _repeatWith = null;
        protected string _dataElementName = string.Empty;
        protected Enums.DataElementOutputEnum _dataElementOutput = Enums.DataElementOutputEnum.Auto;
        protected RdlRender.Container _box = null;

        public static ReportItem NewReportItem(XmlNode node, ReportElement parent)
        {
            switch (node.Name.ToLower())
            {
                case "line":
                    break;
                case "rectangle":
                    return new Rectangle(node, parent);
                case "textbox":
                    return new TextBox(node, parent);
                case "image":
                    break;
                case "subreport":
                    break;
                case "customreportitem":
                    break;
                case "list":
                    break;
                case "matrix":
                    break;
                case "table":
                    return new Table(node, parent);
                case "chart":
                    break;
            }
            return null;
        }

        public ReportItem(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
            Report.ReportItems.Add(_name, this);
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "name":
                    _name = attr.InnerText;
                    break;
                case "style":
                    _style = new Style(attr, this);
                    break;
                case "action":
                    _action = new Action(attr, this);
                    break;
                case "top":
                    _top = new Size(attr.InnerText);
                    break;
                case "left":
                    _left = new Size(attr.InnerText);
                    break;
                case "height":
                    _height = new Size(attr.InnerText);
                    break;
                case "width":
                    _width = new Size(attr.InnerText);
                    break;
                case "zindex":
                    _zindex = Int16.Parse(attr.InnerText);
                    break;
                case "visibility":
                    _visibility = new Visibility(attr, this);
                    break;
                case "tooltip":
                    _toolTop = new Expression(attr, this);
                    break;
                case "label":
                    _label = new Expression(attr, this);
                    break;
                case "linktochild":
                    _linkToChild = attr.InnerText;
                    break;
                case "bookmark":
                    _boolmark = new Expression(attr, this);
                    break;
                case "repeatwith":
                    _repeatWith = attr.InnerText;
                    break;
                case "dataelementname":
                    _dataElementName = attr.InnerText;
                    break;
                case "dataelementoutput":
                    _dataElementOutput = (Enums.DataElementOutputEnum)Enum.Parse(typeof(Enums.DataElementOutputEnum), attr.InnerText,true);
                    break;
                default:
                    break;
            }
        }

        internal override void Parse2()
        {
            base.Parse2();

            if (_visibility.ToggleItem != null)
            {
                ReportItem toggleItem = Report.ReportItems[_visibility.ToggleItem];
                if (toggleItem != null)
                {
                    if (toggleItem is TextBox)
                        ((TextBox)toggleItem).ToggleList.Add(this);
                    else
                        throw new Exception("Toggle items are only allowed to refer to text boxes\n" );
                }
                else
                    throw new Exception("Toggle item " + _visibility.ToggleItem + " not found");
            }
        }

        public string Name
        {
            get { return _name; }
        }

        public string RepeatWith
        {
            get { return _repeatWith; }
        }

        public RdlRender.Container Box
        {
            get { return _box; }
        }
    }
}
