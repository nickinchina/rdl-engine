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
using Rdl.Render;

namespace Rdl.Engine
{
    public abstract class ReportItem : ReportElement
    {
        protected string _name = string.Empty;
        protected Action _action = null;
        protected Size _top = Size.ZeroSize;
        protected Size _left = Size.ZeroSize;
        protected Size _height = null;
        protected Size _width = null;
        protected Int16 _zindex = 0;
        protected Visibility _visibility = Visibility.Visible;
        protected Expression _toolTop = null;
        protected Expression _label = null;
        protected string _linkToChild = null;
        protected Expression _bookmark = null;
        protected string _repeatWith = null;
        protected string _dataElementName = string.Empty;
        protected Enums.DataElementOutputEnum _dataElementOutput = Enums.DataElementOutputEnum.Auto;
        protected Rdl.Render.Element _box = null;

        public static ReportItem NewReportItem(XmlNode node, ReportElement parent)
        {
            switch (node.Name.ToLower())
            {
                case "line":
                    return new Line(node, parent);
                case "rectangle":
                    return new Rectangle(node, parent);
                case "textbox":
                    return new TextBox(node, parent);
                case "image":
                    return new Image(node, parent);
                case "subreport":
                    return new SubReport(node, parent);
                case "customreportitem":
                    break;
                case "list":
                    return new List(node, parent);
                case "matrix":
                    return new Matrix.Matrix(node, parent);
                case "table":
                    return new Table.Table(node, parent);
                case "chart":
                    return new Chart.Chart(node, parent);
            }
            return null;
        }

        public ReportItem(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
            if (_name != string.Empty)
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
                    _bookmark = new Expression(attr, this);
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

        public string Name
        {
            get { return _name; }
        }

        public string RepeatWith
        {
            get { return _repeatWith; }
        }

        public Action Action
        {
            get { return _action; }
        }

        // For ReportItems rendering is broken into 2 parts.  The first part generates
        // the contianing box and the second part is called after.  This allows
        // for base type operations to be performed on the containing box.
        protected abstract void Render1(Rdl.Render.Container parentBox, Rdl.Runtime.Context context, bool visible);
        protected abstract void Render2(Rdl.Runtime.Context context);

        public Rdl.Render.Element Box
        {
            get { return _box; }
            set { _box = value; }
        }

        internal override void Render(Rdl.Render.Container box, Rdl.Runtime.Context context)
        {
            bool visible = true;
            if (_visibility != null && _visibility.IsHidden(context) && _visibility.ToggleItem == null)
                visible = false;

            Render1(box, context, visible);

            if (_action != null)
                _action.Render(box, context);

            if (_box != null && visible)
            {
                if (IsInCell)
                {
                    _box.MatchParentHeight = true;
                    _box.MatchParentWidth = true;
                    // Cell widths don't change.
                    _box.Width = _box.Parent.Width;
                    _box.Height = _box.Parent.Height;
                }
                else
                {
                    _box.Top = _top.points;
                    _box.Left = _left.points;
                    _box.Width = (_width == null) ? box.Width - _left.points : _width.points;
                    _box.Height = (_height == null) ? box.Height - _top.points : _height.points;
                }
            }

            Render2(context);

            TextBox tb = Report.FindToggleItem(_visibility);
            if (tb != null)
                tb.LinkedToggles.Add(new Toggle(_box, tb));
        }

        public string Value
        {
            get
            {
                return "~--" + Name + "--~";
            }
        }
    }
}
