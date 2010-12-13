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
using System.Collections;
using System.Text;

namespace Rdl.Render
{
    public class Toggle
    {
        public TextElement Element;
        public Rdl.Engine.Enums.ToggleDirectionEnum Direction = Rdl.Engine.Enums.ToggleDirectionEnum.positive;

        public Toggle(TextElement te)
        {
            Element = te;
        }

        public Toggle(TextElement te, Rdl.Engine.Enums.ToggleDirectionEnum direction)
        {
            Element = te;
            Direction = direction;
        }
    }

    public class Toggles : IEnumerable
    {
        List<Toggle> _toggleList = null;
        private Element _elmt = null;

        public Toggles(Element elmt)
        {
            _elmt = elmt;
        }

        public Toggles(Toggles t)
        {
            if (t._toggleList != null)
            {
                _toggleList = new List<Toggle>();
                foreach (Toggle tog in t._toggleList)
                    _toggleList.Add(tog);
            }
        }

        public void AddToggle(TextElement toggle)
        {
            if (_toggleList == null)
                _toggleList = new List<Toggle>();
            _toggleList.Add(new Toggle(toggle));
        }

        public void AddToggle(TextElement toggle, Rdl.Engine.Enums.ToggleDirectionEnum direction)
        {
            if (_toggleList == null)
                _toggleList = new List<Toggle>();
            _toggleList.Add(new Toggle(toggle, direction) );
        }

        public int Count
        {
            get
            {
                if (_toggleList == null)
                    return 0;
                else
                    return _toggleList.Count;
            }
        }

        public bool IsVisible
        {
            get
            {
                if (_toggleList != null)
                    foreach (Toggle tog in _toggleList)
                    {
                        if (!tog.Element.IsVisible)
                            return false;
                        if ((tog.Element.ToggleState == TextElement.ToggleStateEnum.closed && tog.Direction == Rdl.Engine.Enums.ToggleDirectionEnum.positive) ||
                            (tog.Element.ToggleState == TextElement.ToggleStateEnum.open && tog.Direction == Rdl.Engine.Enums.ToggleDirectionEnum.negative))
                            return false;
                    }
                return true;
            }
        }

        public Element Element
        {
            get { return _elmt; }
        }

        public override string ToString()
        {
            string ret = string.Empty;

            if (_toggleList != null && _toggleList.Count > 0)
            {
                ret = " Toggles on ( ";
                foreach (Toggle tog in _toggleList)
                    ret += tog.Element.Name + " ";
                ret += ")";
            }
            return ret;
        }

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return _toggleList.GetEnumerator();
        }

        #endregion
    }
}
