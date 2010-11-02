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
