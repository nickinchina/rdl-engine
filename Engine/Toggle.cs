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

namespace Rdl.Engine
{
    class Toggle
    {
        public Rdl.Render.Element Element;
        public TextBox TextBox;
        public Enums.ToggleDirectionEnum Direction;

        public Toggle(Rdl.Render.Element elmt, TextBox textBox) : this(elmt, textBox, Rdl.Engine.Enums.ToggleDirectionEnum.positive)
        {
        }

        public Toggle(Rdl.Render.Element elmt, TextBox textBox, Enums.ToggleDirectionEnum direction)
        {
            TextBox = textBox;
            Direction = direction;
            Element = elmt;
        }

        public Toggle(Toggle t)
        {
            TextBox = t.TextBox;
            Direction = t.Direction;
        }
    }
}
