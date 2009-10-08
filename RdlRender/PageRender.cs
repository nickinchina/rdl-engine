using System;
using System.Collections.Generic;
using System.Text;

namespace RdlRender
{
    // The PageRender class taked a linear rendered document
    // and paginates it into individual pages.
    public class PageRender
    {
        private List<Page> _pageList;
        public decimal TopMargin = 0;
        public decimal BottomMargin = 0;
        public decimal LeftMargin = 0;
        public decimal RightMargin = 0;
        public decimal PageWidth = 0;
        public decimal PageHeight = 0;
        private decimal _width = 0;
        private decimal _height = 0;
        private decimal _top = 0;
        private decimal _left = 0;

        public void Render(Container box)
        {
            RdlEngine.Report rpt = box.Report;

            TopMargin = rpt.TopMargin.points;
            BottomMargin = rpt.BottomMargin.points;
            LeftMargin = rpt.LeftMargin.points;
            RightMargin = rpt.RightMargin.points;
            PageWidth = rpt.PageWidth.points;
            PageHeight = rpt.PageHeight.points;
            _top = rpt.TopMargin.points;
            _left = rpt.LeftMargin.points;
            _width = rpt.PageWidth.points - rpt.RightMargin.points - rpt.LeftMargin.points;
            _height = rpt.PageHeight.points - rpt.TopMargin.points - rpt.BottomMargin.points;

            _pageList = new List<Page>();
            Container b = null;
            Page p = new Page(PageWidth - RightMargin - LeftMargin, 
                PageHeight - TopMargin - BottomMargin, 
                0,
                null,
                ref b);
            _pageList.Add(p);
            RecurseRender( box, ref b, ref p, 0, 0);
        }

        // Render element elmt onto the page currentPage.
        // parent refers to the parent element within the current rendered page.  When
        //      a page break occurs a new parent hierarchy is build for the new page
        //      and parent will point to the bottom of the new parent hierarchy.
        // currentPage will refer to the current page being built.  It will be updated
        //      inside this routine if elmt spans pages.
        // top refers to the absolute position in the master rendered document.
        private decimal RecurseRender(Element elmt,
            ref Container parent,
            ref Page currentPage,
            decimal top,
            decimal pos
            )
        {
            decimal elementHeight = CalcHeight(elmt);
            decimal requiredHeight = elementHeight;
            decimal elementTop;

            // If this element has repeated elements associated with it then make sure
            // that there is room on the page for those repeated elements.
            if (elmt is Container)
            {
                // If this is a toggle item, then determine if we want to include it in the
                // rendered page.
                if (((Container)elmt).Toggle != null && ((Container)elmt).Toggle.ToggleState == TextElement.ToggleStateEnum.closed)
                    return 0;

                foreach (Container re in ((Container)elmt).RepeatList)
                {
                    if (re.AbsoluteTop > elmt.AbsoluteTop)
                        requiredHeight += CalcHeight(re);
                }
            }

            // If this element is spanning pages, then calculate the top
            // based on the starting position of this page.
            if (elmt.Parent != null && elmt.Parent.AbsoluteTop < currentPage.RelativeTop)
                elementTop = pos - (currentPage.RelativeTop - elmt.Parent.AbsoluteTop);
            else
                elementTop = pos;

            bool newPage = false;
            if (elmt is Container && ((Container)elmt).PageBreakBefore)
                newPage = true;
            else if (PosOnPage(parent) + elementTop + requiredHeight > currentPage.AvailableHeight)
            {
                // If the element doesn't fit entirely on this page and we should keep it on
                // one page, then create a new page.
                if (!(elmt is Container) || (elmt is Container && ((Container)elmt).KeepTogether))
                    newPage = true;
            }

            if (newPage)
            {
                currentPage.AddFooters(parent);

                currentPage = new Page(PageWidth - RightMargin - LeftMargin,
                    PageHeight - TopMargin - BottomMargin,
                    top, elmt.Parent, ref parent);

                _pageList.Add(currentPage);
            }

            Element newElement = null;
            if (elmt is Container)
            {
                newElement = ((Container)elmt).Copy(false);
                newElement.CanGrowVertically = true;

                // Loop through the repeated elements
                foreach (Container r in ((Container)elmt).RepeatList)
                    // If the repeated element is below the current element then reserve space for it at the bottom.
                    if (r.AbsoluteTop > elmt.AbsoluteTop)
                        currentPage.FooterSpace += (r.Top - pos + elementHeight + CalcHeight(r));
            }
            if (elmt is TextElement)
                newElement = new TextElement((TextElement)elmt);
            if (elmt is ImageElement)
                newElement = new ImageElement((ImageElement)elmt);

            newElement.Parent = parent;
            newElement.Top = 0;
            newElement.Height = 0;
            if (parent != null)
                parent.Children.Add(newElement);

            // Set the height of this element to either the height of the source element
            // or the height of the remaining space on the page.  Whichever is less.
            newElement.Top = elementTop;
            newElement.Height = Math.Min(elementHeight, currentPage.AvailableHeight - PosOnPage(parent));

            if (elmt is Container)
            {
                Container be = newElement as Container;
                decimal pos2 = 0;

                foreach (Element child in ((Container)elmt).Children)
                {
                    if (elmt is FixedContainer)
                        pos2 = child.Top;
                    pos2 += RecurseRender(child, ref be, ref currentPage, top + elmt.Top, pos2);
                    parent = be.Parent;
                }

                // Loop through the repeated elements
                foreach (Container r in be.RepeatList)
                    // If the repeated element is below the current element then release the reserved space
                    if (r.AbsoluteTop > be.AbsoluteTop)
                        currentPage.FooterSpace -= (r.Top - elmt.Top + elementHeight + CalcHeight(r));

                if (be.PageBreakAfter)
                {
                    currentPage.AddFooters(parent);

                    currentPage = new Page(PageWidth - RightMargin - LeftMargin,
                        PageHeight - TopMargin - BottomMargin,
                        top, elmt.Parent, ref parent);

                    _pageList.Add(currentPage);
                }
            }
            return elementHeight;
        }

        private decimal CalcHeight(Element elmt)
        {
            if (elmt is Container)
            {
                Container c = elmt as Container;

                if (c.Toggle != null && c.Toggle.ToggleState == TextElement.ToggleStateEnum.closed)
                    return 0;
            }
            if (elmt is FlowContainer)
            {
                decimal height = 0;

                FlowContainer fc = elmt as FlowContainer;
                foreach (Element child in fc.Children)
                    height += CalcHeight(child);
                return height;
            }
            else
                return elmt.Height;
        }

        private decimal PosOnPage(Element elmt)
        {
            decimal pos = 0;
            while (elmt != null)
            {
                pos += elmt.Top;
                elmt = elmt.Parent;
            }
            return pos;
        }

        public List<Page> Pages
        {
            get { return _pageList; }
        }
    }
}
