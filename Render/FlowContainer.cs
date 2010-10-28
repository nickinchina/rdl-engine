using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Render
{
    // A FlowContainer flows elements one after another.
    // Elements are always positioned immediately following
    // the previous element and flush left.
    internal class FlowContainer : Container
    {
        public enum FlowDirectionEnum
        {
            TopDown,
            LeftToRight
        };

        public FlowDirectionEnum FlowDirection = FlowDirectionEnum.TopDown;
        public Element FillElement = null;

        internal FlowContainer(Container parent, Rdl.Engine.ReportElement reportElement, BoxStyle style)
            : base(parent, reportElement, style)
        {
        }

        public FlowContainer(FlowContainer b) : this(b, false)
        {
        }

        public FlowContainer(FlowContainer b, bool copyChildren)
            : base(b, copyChildren)
        {
            FlowDirection = b.FlowDirection;
        }

        internal override void SetParentSizes(bool ignoreVisibility)
        {
            _renderedHeight = 0;
            _renderedWidth = 0;

            if (!ignoreVisibility && !IsVisible)
                return;

            foreach (Element child in _childElements)
            {
                // Set the sizes of all of the children first.
                child.SetParentSizes(ignoreVisibility);

                // Set the size of this element based on the children.
                if (FlowDirection == FlowDirectionEnum.TopDown)
                    _renderedHeight += child.Height;
                else
                    _renderedHeight = Math.Max(_renderedHeight, child.Top + child.Height);
                if (FlowDirection == FlowDirectionEnum.LeftToRight)
                    _renderedWidth += child.Width;
                else
                    _renderedWidth = Math.Max(_renderedWidth, child.Left + child.Width);
            }
        }

        internal override void SetChildSizes(bool ignoreVisibility)
        {
            if (!ignoreVisibility && !IsVisible)
                return;

            FillElement = null;
            base.SetChildSizes(ignoreVisibility);

            decimal height;
            decimal width;
            if (FillElement != null)
            {
                height = 0;
                width = 0;
                foreach (Element child in _childElements)
                    if (child != FillElement)
                    {
                        height += child.Height;
                        width += child.Width;
                    }
                if (FlowDirection == FlowDirectionEnum.TopDown)
                    FillElement._renderedHeight = _renderedHeight - height;
                else
                    FillElement._renderedWidth = _renderedWidth - width;
                FillElement.SetChildSizes(ignoreVisibility);
            }

            height = 0;
            width = 0;
            foreach (Element child in _childElements)
            {
                if (FlowDirection == FlowDirectionEnum.TopDown)
                    child._renderedTop = height;
                else
                    child._renderedLeft = width;
                height += child.Height;
                width += child.Width;
            }
        }

        public override decimal TotalHeight(bool ignoreVisibility)
        {
            decimal height = 0;

            if (FlowDirection == FlowDirectionEnum.TopDown)
                foreach (Element child in _childElements)
                    height += child.TotalHeight(ignoreVisibility);
            else
                height = base.TotalHeight(ignoreVisibility);
            return Math.Max(height, _height);
        }

        public override decimal TotalWidth(bool ignoreVisibility)
        {
            decimal width = 0;

            if (FlowDirection == FlowDirectionEnum.LeftToRight)
                foreach (Element child in _childElements)
                    width += child.TotalWidth(ignoreVisibility);
            else
                width = base.TotalWidth(ignoreVisibility);
            return Math.Max(width, _width);
        }
    }
}
