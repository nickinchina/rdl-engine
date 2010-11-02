using System;
using System.Collections.Generic;
using System.Text;

namespace RdlRender
{
    public class Page
    {
        private PageElement _pageBox;
        private FixedContainer _pageHeaderBox;
        private FixedContainer _pageDetailsBox;
        private FixedContainer _pageFooterBox;
        // The RelativeTop value represents the position in the master rendered document
        // of the top of the body of this page.
        private decimal _relativeTop = 0;

        internal Page(decimal width, 
            decimal height,
            decimal relativeTop,
            Container breakAt,
            ref Container currentPos)
        {
            _pageBox = new PageElement();
            _pageBox.Width = width;
            _pageBox.Height = height;
            _pageBox.CanGrowVertically = false;
            _pageBox.Name = "Page";
            if (breakAt != null)
                _pageBox.BaseStyleList = breakAt.BaseStyleList;
            _relativeTop = relativeTop;

            _pageHeaderBox = _pageBox.AddFixedContainer();
            _pageHeaderBox.Width = _pageBox.Width;
            _pageHeaderBox.Name = "PageHeader";
            if (breakAt != null)
                _pageHeaderBox.BaseStyleList = breakAt.BaseStyleList;
            _pageHeaderBox.CanGrowVertically = false;

            _pageDetailsBox = _pageBox.AddFixedContainer();
            _pageDetailsBox.Width = _pageBox.Width;
            _pageDetailsBox.Height = _pageBox.Height;
            _pageDetailsBox.Name = "PageDetails";
            if (breakAt != null)
                _pageDetailsBox.BaseStyleList = breakAt.BaseStyleList;
            _pageDetailsBox.CanGrowVertically = false;

            _pageFooterBox = _pageBox.AddFixedContainer();
            _pageFooterBox.Width = _pageBox.Width;
            _pageFooterBox.Name = "PageFooter";
            _pageFooterBox.CanGrowVertically = false;

            if (breakAt != null)
            {
                AddHeaders(breakAt);
                _pageDetailsBox.Height = _pageBox.Height - _pageHeaderBox.Height - _pageFooterBox.Height;
                _pageDetailsBox.Top = _pageHeaderBox.Height;
            }
            currentPos = AddParentBoxes(breakAt, _pageDetailsBox);
        }

        private void AddHeaders(Container b)
        {
            decimal pos = _pageHeaderBox.Height;

            if (b.Parent != null)
                AddHeaders(b.Parent);

            // Loop through the repeated elements
            foreach (Container r in b.RepeatList)
            {
                // If the repeated element is above the current element then put it at the top
                if (r.AbsoluteTop < b.AbsoluteTop)
                {
                    // Copy the header element along with all children.
                    Container re = r.Copy(true);
                    re.Parent = _pageHeaderBox;
                    re.Top += pos;
                    re.Left = r.AbsoluteLeft;
                    _pageHeaderBox.Children.Add(re);

                    _pageHeaderBox.Height = Math.Max(_pageHeaderBox.Height, pos + re.Top + re.Height);
                }
                else
                    // If the repeated element is below the current element then reserve space
                    // for it at the bottom.
                    _pageFooterBox.Height += (r.Top - b.Top - b.Height + r.Height);
            }
        }

        internal void AddFooters(Container b)
        {
            decimal top = 0;
            _pageFooterBox.Top = _pageHeaderBox.Height + _pageDetailsBox.Height;
            RecurseAddFooters(b, ref top);
        }

        private void RecurseAddFooters(Container b, ref decimal top)
        {
            decimal height = 0;

            // Loop through the repeated elements
            foreach (Container r in b.RepeatList)
            {
                // If the repeated element is above the current element then put it at the top
                if (r.AbsoluteTop > b.AbsoluteTop)
                {
                    // Copy the header element along with all children.
                    Container re = r.Copy(true);
                    re.Parent = _pageFooterBox;
                    re.Top = top + re.Top - b.Top - b.Height;
                    re.Left = r.AbsoluteLeft;
                    _pageFooterBox.Children.Add(re);

                    height = Math.Max(re.Top + re.Height, height);
                }
            }

            top += height;

            if (b.Parent != null)
                AddFooters(b.Parent);
        }

        internal Container AddParentBoxes(Container b, Container newParent)
        {
            if (b == null)
                return newParent;
            if (b.Parent != null)
                newParent = AddParentBoxes(b.Parent, newParent);

            Container b1 = b.Copy(false);
            b1.Parent = newParent;
            newParent.Children.Add(b1);

            if (b1.AbsoluteTop > RelativeTop)
                b1.Top -= (b1.AbsoluteTop - RelativeTop);
            else
            {
                if (b1.AbsoluteTop < _relativeTop)
                    b1.Height -= (_relativeTop - b1.AbsoluteTop);
                b1.Top = 0;
            }
            if (b1.Height > newParent.Height)
                b1.Height = _pageDetailsBox.Height;

            return b1;
        }

        internal decimal AvailableHeight
        {
            get { return _pageDetailsBox.Height; }
        }

        internal decimal RelativeTop
        {
            get { return _relativeTop; }
        }

        internal decimal FooterSpace
        {
            get { return _pageFooterBox.Height; }
            set { _pageFooterBox.Height = value; }
        }

        public PageElement Box
        {
            get { return _pageBox; }
        }

        public override string ToString()
        {
            string ret = string.Empty;
            foreach (Element e in _pageBox.Children)
                ret += e.ToString() + "\r\n";
            return ret;
        }
    }
}
