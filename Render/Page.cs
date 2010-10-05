using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Rdl.Render
{
    public class Page : Container
    {
        private FixedContainer _pageHeaderBox;
        private FixedContainer _pageDetailsBox;
        private FixedContainer _pageFooterBox;
        // The RelativeTop value represents the position in the master rendered document
        // of the top of the body of this page.
        private decimal _relativeTop = 0;
        private int _pageNumber = 0;
        public Dictionary<string, string> elementValues = new Dictionary<string, string>();

        internal Page(Rdl.Render.GenericRender rpt,
            int pageNumber,
            decimal width, 
            decimal height,
            decimal relativeTop,
            Container breakAt,
            ref Container currentPos) : base(null, null, null)
        {
            Width = width;
            Height = height;
            CanGrowVertically = false;
            Name = "Page";
            _pageNumber = pageNumber;
            _relativeTop = relativeTop;

            _pageHeaderBox = AddFixedContainer(rpt.Report, rpt.Report.Style, null);
            _pageHeaderBox.Width = Width;
            _pageHeaderBox.Name = "PageHeader";
            _pageHeaderBox.CanGrowVertically = true;

            // Add in the page header if appropriate
            if (rpt.Report.PageHeader != null)
                if (pageNumber > 0 || rpt.Report.PageHeader.PrintOnFirstPage)
                    _pageHeaderBox.Children.Add(PageRender.Copy(rpt.PageHeaderContainer, true));
            _pageHeaderBox.SetSizes(false);

            _pageDetailsBox = AddFixedContainer(rpt.Report, rpt.Report.Style, null);
            _pageDetailsBox.Width = Width;
            _pageDetailsBox.Height = Height;
            _pageDetailsBox.Name = "PageDetails";
            _pageDetailsBox.CanGrowVertically = false;

            _pageFooterBox = AddFixedContainer(rpt.Report, rpt.Report.Style, null);
            _pageFooterBox.Width = Width;
            _pageFooterBox.Name = "PageFooter";
            _pageFooterBox.CanGrowVertically = true;

            if (breakAt != null)
                AddHeaders(breakAt);
            AddFooters(rpt, _pageFooterBox);

            _pageDetailsBox.Height = Height - _pageHeaderBox.Height - _pageFooterBox.Height;
            _pageDetailsBox.Top = _pageHeaderBox.Height;

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
                    Container re = PageRender.Copy(r, true);
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

        internal void AddFooters(Rdl.Render.GenericRender rpt, Container b)
        {
            decimal top = 0;
            //_pageFooterBox.Top = _pageHeaderBox.Height + _pageDetailsBox.Height;
            if (b != null)
                RecurseAddFooters(b, ref top);

            if (rpt.Report.PageFooter != null)
            {
                Container pageFooter = PageRender.Copy(rpt.PageFooterContainer, true);
                pageFooter.Top = top;
                _pageFooterBox.Children.Add(pageFooter);
                top += pageFooter.Height;
            }
            _pageFooterBox.Height = top;
            _pageFooterBox.Top = Height - top;        }

        public void RemoveLastPageHeadersAndFooters(Rdl.Render.GenericRender rpt)
        {
            if (rpt.Report.PageHeader != null)
                if (!rpt.Report.PageHeader.PrintOnLastPage)
                    _childElements.Remove(_pageHeaderBox);
            if (rpt.Report.PageFooter != null)
                if (!rpt.Report.PageFooter.PrintOnLastPage)
                    _childElements.Remove(_pageFooterBox);
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
                    Container re = PageRender.Copy(r, true);
                    re.Parent = _pageFooterBox;
                    re.Top = top + re.Top - b.Top - b.Height;
                    re.Left = r.AbsoluteLeft;
                    _pageFooterBox.Children.Add(re);

                    height = Math.Max(re.Top + re.Height, height);
                }
            }

            top += height;

            if (b.Parent != null)
                RecurseAddFooters(b.Parent, ref top);
        }

        internal Container AddParentBoxes(Container b, Container newParent)
        {
            if (b == null)
                return newParent;
            if (b.Parent != null)
                newParent = AddParentBoxes(b.Parent, newParent);

            Container b1 = PageRender.Copy(b, false);
            b1.Parent = newParent;
            newParent.Children.Add(b1);

            if (b1.AbsoluteTop >= RelativeTop)
                b1.Top -= (b1.AbsoluteTop - RelativeTop);
            else
            {
                if (b.AbsoluteTop < _relativeTop)
                    b1.Height -= (_relativeTop - b.AbsoluteTop);
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

        public int PageNumber
        {
            get { return _pageNumber; }
        }

        public override string ToString()
        {
            string ret = string.Empty;
            ret += _pageHeaderBox.ToString() + "\r\n";
            ret += _pageDetailsBox.ToString() + "\r\n";
            ret += _pageFooterBox.ToString() + "\r\n";
            ret += "Page " + _pageNumber.ToString() + "\r\n";            ret += "Width:" + Width.ToString() + " Height:" + Height.ToString() + "\r\n";            return ret;
        }

        // Resolve the values of the ReportItem references on this page.
        public void ResolveReportItemReferences()
        {
            RecurseResolveReportItemReferences(_pageHeaderBox);
            RecurseResolveReportItemReferences(_pageFooterBox);
            RecurseResolveReportItemReferences(_pageDetailsBox);
        }

        private string MatchEvaluatorMethod(Match match)
        {
            if (elementValues.ContainsKey(match.Groups["name"].Value))
                return elementValues[match.Groups["name"].Value];
            else
                return string.Empty;
        }

        private void RecurseResolveReportItemReferences(Element elmt)
        {
            if (elmt is TextElement)
            {
                TextElement te = (TextElement)elmt;

                Regex regEx = new Regex("~--(?<name>.*)--~");
                te.Text = regEx.Replace(te.Text, MatchEvaluatorMethod);
            }
            if (elmt is Container)
            {
                foreach( Element child in ((Container)elmt).Children)
                    RecurseResolveReportItemReferences(child);
            }
        }
    }
}
