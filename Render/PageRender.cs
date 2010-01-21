using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Render
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

        public void Render(Rdl.Engine.Report rpt)
        {
            rpt.SetSizes(false);

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
            Page p = new Page(
                rpt, 
                0,
                rpt.BodyContainer.Width, //PageWidth - RightMargin - LeftMargin, 
                PageHeight - TopMargin - BottomMargin, 
                0,
                null,
                ref b);
            _pageList.Add(p);
            RecurseRender( rpt, rpt.BodyContainer, ref b, ref p, 0, rpt.BodyContainer.Top, 0, 0);

            p.AddFooters(rpt, null, true);
        }

        // Render element elmt onto the page currentPage.
        // parent refers to the parent element within the current rendered page.  When
        //      a page break occurs a new parent hierarchy is build for the new page
        //      and parent will point to the bottom of the new parent hierarchy.
        // currentPage will refer to the current page being built.  It will be updated
        //      inside this routine if elmt spans pages.
        private void RecurseRender(
            Rdl.Engine.Report rpt,
            Element elmt, // The current element from the source document
            ref Container parent,   // The parent element in the page.
            ref Page currentPage,   // The current page.
            decimal parentTop,      // The absolute position of the parent element in the master rendered document.
            decimal top,            // The absolute position in the master rendered document.
            decimal xPos,           // The X position within the parent object
            decimal yPos            // The Y position within the parent object
            )
        {
            //decimal elementHeight = CalcHeight(elmt);
            decimal elementHeight = elmt.Height;
            decimal requiredHeight = elementHeight;
            decimal elementTop;

            // If this element has repeated elements associated with it then make sure
            // that there is room on the page for those repeated elements.
            if (elmt is Container)
            {
                // If this is a toggle item, then determine if we want to include it in the
                // rendered page.
                if (!((Container)elmt).IsVisible)
                    return;

                foreach (Container re in ((Container)elmt).RepeatList)
                {
                    if (re.AbsoluteTop > elmt.AbsoluteTop)
                        requiredHeight += re.Height;
                        //requiredHeight += CalcHeight(re);
                }
            }

            // If this element is spanning pages, then calculate the top
            // based on the starting position of this page.
            if (parentTop < currentPage.RelativeTop)
                elementTop = yPos - (currentPage.RelativeTop - parentTop);
            else
                elementTop = yPos;

            bool newPage = false;
            if (elmt is Container && ((Container)elmt).PageBreakBefore)
                newPage = true;
            else if (top - currentPage.RelativeTop + requiredHeight > currentPage.AvailableHeight)
            {
                // If the element doesn't fit entirely on this page and we should keep it on
                // one page, then create a new page.
                if (!(elmt is Container) || (elmt is Container && ((Container)elmt).KeepTogether))
                    newPage = true;
            }

            if (newPage)
            {
                currentPage.AddFooters(rpt, parent, false);

                currentPage = new Page(
                    rpt,
                    currentPage.PageNumber + 1,
                    rpt.BodyContainer.Width, //PageWidth - RightMargin - LeftMargin,
                    PageHeight - TopMargin - BottomMargin,
                    top, elmt.Parent, ref parent);

                _pageList.Add(currentPage);

                if (parentTop < currentPage.RelativeTop)
                    elementTop = yPos - (currentPage.RelativeTop - parentTop);
                else
                    elementTop = yPos;
            }

            Element newElement = null;
            if (elmt is Container)
            {
                newElement = Copy(((Container)elmt), false);
                newElement.CanGrowVertically = true;

                // Loop through the repeated elements
                foreach (Container r in ((Container)elmt).RepeatList)
                    // If the repeated element is below the current element then reserve space for it at the bottom.
                    if (r.AbsoluteTop > elmt.AbsoluteTop)
                        currentPage.FooterSpace += (r.Top - yPos + elementHeight + r.Height);
                        //currentPage.FooterSpace += (r.Top - yPos + elementHeight + CalcHeight(r));
            }
            if (elmt is TextElement)
                newElement = new TextElement((TextElement)elmt);
            if (elmt is ImageElement)
                newElement = new ImageElement((ImageElement)elmt);
            if (elmt is ChartElement)
                newElement = new ChartElement((ChartElement)elmt);

            newElement.Parent = parent;
            newElement.Top = 0;
            newElement.Height = 0;
            //newElement.Left += xPos;
            if (parent != null)
                parent.Children.Add(newElement);

            // Set the height of this element to either the height of the source element
            // or the height of the remaining space on the page.  Whichever is less.
            newElement.Top = elementTop;
            newElement.Height = Math.Min(elementHeight, currentPage.AvailableHeight - (top - currentPage.RelativeTop));

            if (elmt is Container)
            {
                Container be = newElement as Container;

                // Sort the children by the top values so if the children span a page then
                // they will render in the correct order.
                if (elmt is FixedContainer)
                    ((Container)elmt).Children.Sort(delegate(Element a, Element b) {return a.Top.CompareTo(b.Top);});

                foreach (Element child in ((Container)elmt).Children)
                {
                    RecurseRender(rpt, child, ref be, ref currentPage, top, top + child.Top, child.Left, child.Top);
                    parent = be.Parent;
                }

                // Loop through the repeated elements
                foreach (Container r in be.RepeatList)
                    // If the repeated element is below the current element then release the reserved space
                    if (r.AbsoluteTop > be.AbsoluteTop)
                        currentPage.FooterSpace -= (r.Top - elmt.Top + elementHeight + r.Height);
                        //currentPage.FooterSpace -= (r.Top - elmt.Top + elementHeight + CalcHeight(r));

                if (be.PageBreakAfter)
                {
                    currentPage.AddFooters(rpt, parent, false);

                    currentPage = new Page(
                        rpt,
                        currentPage.PageNumber + 1,
                        rpt.BodyContainer.Width, //PageWidth - RightMargin - LeftMargin,
                        PageHeight - TopMargin - BottomMargin,
                        elmt.Top + elementHeight, elmt.Parent, ref parent);

                    _pageList.Add(currentPage);
                }
            }
        }

        //private decimal CalcHeight(Element elmt)
        //{
        //    decimal height = 0;

        //    if (elmt is Container)
        //    {
        //        Container c = elmt as Container;

        //        if (!c.Toggles.IsVisible)
        //            return 0;

        //        if (elmt is FlowContainer && ((FlowContainer)elmt).FlowDirection == FlowContainer.FlowDirectionEnum.TopDown)
        //        {
        //            FlowContainer fc = elmt as FlowContainer;
        //            foreach (Element child in fc.Children)
        //                height += CalcHeight(child);
        //        }
        //        else
        //            height = elmt.Height;
        //    }
        //    return height;
        //}

        public List<Page> Pages
        {
            get { return _pageList; }
        }

        public static Container Copy(Container c, bool copyChildren)
        {
            if (c is FixedContainer || c is Table.Container)
                return new FixedContainer(c, copyChildren);
            if (c is FlowContainer)
                return new FlowContainer((FlowContainer)c, copyChildren);
            if (c is Table.Cell)
                return new Table.Cell((Table.Cell)c, copyChildren);
            return null;
        }
    }
}
