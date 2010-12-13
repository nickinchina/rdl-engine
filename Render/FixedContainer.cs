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

namespace Rdl.Render
{
    // A FixedCOntainer positions child elements at fixed
    // positions.
    public class FixedContainer : Container
    {
        internal FixedContainer(Container parent, Rdl.Engine.ReportElement reportElement, BoxStyle style)
            : base(parent, reportElement, style)
        {
        }

        public FixedContainer(Container b) : base(b)
        {
        }

        public FixedContainer(Container b, bool copyChildren)
            : base(b, copyChildren)
        {
        }

        internal override void SetParentSizes(bool ignoreVisibility)
        {
            decimal diff;
            base.SetParentSizes(ignoreVisibility);

            if (!ignoreVisibility && !IsVisible)
                return;

            // The purpose of this code is to adjust the relative positions of the child items
            // of a fixed container to match how they are layed out prior to expanding 
            // when rendered.  The width and height of the container is also adjusted after
            // the children are positioned.

            // The elements of the fixed container need to be rearanged according to the 
            // rendered sizes of the contained elements.
            List<Element> childElements = new List<Element>();
            foreach (Element child in _childElements)
                if (ignoreVisibility || child.IsVisible)
                    childElements.Add(child);

            // First sort the child list by the tops.
            childElements.Sort(delegate(Element a, Element b) {return a._top.CompareTo(b._top);});
            // Then adjust the _renderedTop value
            for (int i = 0; i < childElements.Count; i++)
            {
                for (int j = i + 1; j < childElements.Count; j++)
                {
                    // diff is the space between the bottom of element i and the top of element j.
                    diff = childElements[j]._top - (childElements[i]._top + childElements[i]._height);
                    if (diff >= 0 && childElements[j].Height > 0 &&
                            childElements[j]._renderedTop < childElements[i].Top + childElements[i].Height + diff)
                        childElements[j]._renderedTop = childElements[i].Top + childElements[i].Height + diff;
                }
                if (childElements[i].Height > 0)
                    _renderedHeight = Math.Max(_renderedHeight, childElements[i].Top + childElements[i].Height);
            }

            // Next sort the child list by the lefts.
            childElements.Sort(delegate(Element a, Element b) { return a._left.CompareTo(b._left); });
            // Then adjust the _renderedLeft value
            for (int i = 0; i < childElements.Count; i++)
            {
                for (int j = i + 1; j < childElements.Count; j++)
                {
                    // diff is the space between the bottom of element i and the top of element j.
                    diff = childElements[j]._left - (childElements[i]._left + childElements[i]._width);
                    if (diff >= 0 && childElements[j].Width > 0 &&
                            childElements[j]._renderedLeft < childElements[i].Left + childElements[i].Width + diff)
                        childElements[j]._renderedLeft = childElements[i].Left + childElements[i].Width + diff;
                }
                if (childElements[i].Width > 0)
                    _renderedWidth = Math.Max(_renderedWidth, childElements[i].Left + childElements[i].Width);
            }
        }

    }
}
