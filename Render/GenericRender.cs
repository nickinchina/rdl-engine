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
//using System.Linq;
using System.Text;

namespace Rdl.Render
{
    /// <summary>
    /// The Render class is the generic form of the rendered report.  It is used to then
    /// render the report to a specific format.
    /// </summary>
    public class GenericRender
    {
        private List<BoxStyle> _styleList = new List<BoxStyle>();
        private List<ImageData> _imageList = new List<ImageData>();

        public Container BodyContainer = null;
        public Container PageHeaderContainer = null;
        public Container PageFooterContainer = null;
        public Rdl.Engine.Report _report = null;

        public GenericRender(Rdl.Engine.Report rpt, Rdl.Runtime.Context context)
        {
            _report = rpt;

            BodyContainer = new FixedContainer(null, rpt, null);
            BodyContainer.Name = "ReportBody";
            BodyContainer.Width = rpt.Width.points;
            BodyContainer.ContextBase = true;
            BodyContainer.SetGenericRender(this);
            BodyContainer.StyleIndex = AddStyle(new BoxStyle(Rdl.Engine.Style.DefaultStyle, context));

            PageHeaderContainer = new FixedContainer(null, rpt, null);
            PageHeaderContainer.Name = "PageHeader";
            PageHeaderContainer.Width = rpt.Width.points;
            PageHeaderContainer.ContextBase = true;
            PageHeaderContainer.SetGenericRender(this);
            PageHeaderContainer.StyleIndex = AddStyle(new BoxStyle(Rdl.Engine.Style.DefaultStyle, context));

            PageFooterContainer = new FixedContainer(null, rpt, null);
            PageFooterContainer.Name = "PageFooter";
            PageFooterContainer.Width = rpt.Width.points;
            PageFooterContainer.ContextBase = true;
            PageFooterContainer.SetGenericRender(this);
            PageFooterContainer.StyleIndex = AddStyle(new BoxStyle(Rdl.Engine.Style.DefaultStyle, context));
        }

        public Rdl.Engine.Report Report
        {
            get { return _report; }
        }

        /// <exclude/>
        public void SetSizes(bool ignoreVisibility)
        {
            PageHeaderContainer.SetSizes(ignoreVisibility);
            PageFooterContainer.SetSizes(ignoreVisibility);
            BodyContainer.SetSizes(ignoreVisibility);
        }

        internal int AddStyle(BoxStyle style)
        {
            for (int i = 0; i < _styleList.Count; i++)
                if (_styleList[i].GetType() == style.GetType() && _styleList[i].Equals(style))
                    return i;

            _styleList.Add(style);
            return _styleList.Count - 1;
        }

        public List<Rdl.Render.BoxStyle> StyleList
        {
            get { return _styleList; }
        }

        internal int AddImage(Rdl.Engine.Image image, Rdl.Runtime.Context context)
        {
            if (image._imageIndex >= 0)
                return image._imageIndex;

            System.Drawing.Image img = image.GetImage(context);

            // Search for the image in the image list.
            for (int i = 0; i < _imageList.Count; i++)
                if (_imageList[i].CompareTo(img))
                    return i;

            ImageData id = new ImageData();
            id.imageData = img;
            id.MimeType = image.MIMEType(context);
            if (image is Rdl.Engine.BackgroundImage)
                id.ImageRepeat = ((Rdl.Engine.BackgroundImage)image).ImageRepeat(context);
            id.Sizing = image.Sizing;

            _imageList.Add(id);
            return _imageList.Count - 1;
        }

        /// <exclude/>
        public List<Rdl.Render.ImageData> ImageList
        {
            get { return _imageList; }
        }

    }
}
