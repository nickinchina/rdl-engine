using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Drawing;

namespace Rdl.Engine.Chart
{
    class CategoryAxis : Axis
    {
        private int _totalCategories = 1;
        private int _categoryLabelWidth = 0;

        public CategoryAxis(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        internal void DrawHorizontal(Chart chart,
            Rdl.Runtime.Context context,
            Graphics g,
            bool centered,
            int l, int t, int w, ref int h)
        {
            if (chart.LeafCategories != null)
                _totalCategories = chart.LeafCategories.Count;

            if (Scalar)
                DrawScalarHorizontal(chart, context, g, centered, l, t, w, ref h);
            else
                DrawNonScalarHorizontal(chart, context, g, centered, l, t, w, ref h);
        }

        internal void DrawVertical(Chart chart,
            Rdl.Runtime.Context context,
            Graphics g,
            bool centered,
            ref int l, int t, ref int w, int h)
        {
            if (chart.LeafCategories != null)
                _totalCategories = chart.LeafCategories.Count;

            // Find the tick mark sizes and axis position for the axis.
            int majorTickStart, majorTickWidth, minorTickStart, minorTickWidth, axisPos, axisWidth;
            AdjustAxisPosition(GetAxisThickness(context), out majorTickStart, out majorTickWidth,
                out minorTickStart, out minorTickWidth,
                out axisPos, out axisWidth);

            // If we are showing the grid lines then adjust the tick width to include the whole chart area.
            if (MajorGridLines.ShowGridLines)
                majorTickWidth = w - _categoryLabelWidth - axisPos - majorTickStart;
            if (MinorGridLines.ShowGridLines)
                minorTickWidth = w - _categoryLabelWidth - axisPos - minorTickStart;

            // Draw the axis line 
            Color color = Style.W32Color(chart.ValueAxis.Style.Color(context));
            g.DrawLine(new Pen(new SolidBrush(color), GetAxisThickness(context)),
                new Point(l + _categoryLabelWidth + axisPos, t), new Point(l + _categoryLabelWidth + axisPos, t + h));

            Font labelFont = Style.GetWindowsFont(context);
            Brush brush = new SolidBrush(Style.W32Color(Style.Color(context)));

            Pen majorGridPen = new Pen( color, 1);
            if (chart.CategoryAxis.MajorGridLines != null)
                majorGridPen = new Pen( Style.W32Color(MajorGridLines.Style.Color(context)),
                    (int)MajorGridLines.Style.BorderWidth.Top(context).points);
            Pen minorGridPen = new Pen( color, 1);
            if (chart.CategoryAxis.MinorGridLines != null)
                minorGridPen = new Pen( Style.W32Color(MinorGridLines.Style.Color(context)),
                    (int)MinorGridLines.Style.BorderWidth.Top(context).points);

            if (Scalar)
            {
                if (chart.CategoryGrouping == null || chart.CategoryGrouping.NextGrouping != null)
                    throw new Exception("There must be exactly one grouping on a scalar axis.");

                // Find the min and max values from the data.
                decimal min = 0;
                decimal max = 0;
                foreach (Category cat in chart.Categories)
                {
                    min = Math.Min(min, decimal.Parse(cat.Value));
                    max = Math.Max(max, decimal.Parse(cat.Value));
                }

                // Adjust the min and max values and find int intervals.
                SetMinMaxValues(context, min, max);

                int majorIntervals = (int)((_maxValue - _minValue) / _majorIntervalValue);
                int minorIntervals = (int)((_maxValue - _minValue) / _minorIntervalValue);

                float majorIntervalSize = (float)h / majorIntervals;
                float minorIntervalSize = (float)h / minorIntervals;

                int i = 0;
                for (decimal dValue = min; dValue <= max; dValue += _majorIntervalValue)
                {
                    // Draw the labels.
                    if (Visible)
                    {
                        SizeF stringSize = g.MeasureString(dValue.ToString(), labelFont);
                        g.DrawString(dValue.ToString(), labelFont, brush,
                            l + _categoryLabelWidth - stringSize.Width,
                            t + h - (int)(i * majorIntervalSize) - (stringSize.Height / 2));
                    }

                    // Draw the major tick mark
                    if (majorTickWidth > 0)
                    {
                        g.DrawLine(majorGridPen, 
                            new Point(l + _categoryLabelWidth + axisPos + majorTickStart,
                                t + h - (int)(i * majorIntervalSize)),
                            new Point(l + _categoryLabelWidth + axisPos + majorTickStart + majorTickWidth,
                                t + h - (int)(i * majorIntervalSize)));
                    }

                    // Draw the minor tick marks
                    if (majorTickWidth > 0)
                    {
                        for( int j=1; j < minorIntervals / majorIntervals; j++)
                            g.DrawLine(minorGridPen, 
                                new Point(l + _categoryLabelWidth + axisPos + minorTickStart,
                                    t + h - (int)(i * majorIntervalSize) - (int)(j * minorIntervalSize)),
                                new Point(l + _categoryLabelWidth + axisPos + minorTickStart + minorTickWidth,
                                    t + h - (int)(i * majorIntervalSize) - (int)(j * minorIntervalSize)));
                    }

                    i++;
                }
            }
            else if (chart.Categories != null && chart.Categories.Count > 0)
            {
                DrawCategoryOntoVerticalAxis(chart.Categories, g, brush, labelFont, context,
                    centered, l, t, h, 0, axisPos,
                    majorGridPen, majorTickStart, majorTickWidth,
                    minorGridPen, minorTickStart, minorTickWidth);
            }

            l += _categoryLabelWidth + axisWidth;
            w -= _categoryLabelWidth + axisWidth;
        }

        public int TotalCategories
        {
            get { return _totalCategories; }
        }

        /// <summary>
        /// If the category axis is being painted on the vertical axis, then we
        /// need to initially find the width that the axis will take up so
        /// the horizontal axis can be drawn in the appripriate place.
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="context"></param>
        /// <param name="g"></param>
        /// <returns></returns>
        internal int Width(Chart chart,
            Rdl.Runtime.Context context,
            Graphics g)
        {
            // Find the maximum string width of the category data.
            _categoryLabelWidth = 0;
            if (Visible)
            {
                // Determine the amount of space required for the axis labels.
                Font labelFont = Style.GetWindowsFont(context);
                if (Scalar)
                {
                    // Find the min and max values from the data.
                    decimal min = 0;
                    decimal max = 0;
                    foreach (Category cat in chart.Categories)
                    {
                        min = Math.Min(min, decimal.Parse(cat.Value));
                        max = Math.Max(max, decimal.Parse(cat.Value));
                    }

                    // Adjust the min and max values and find int intervals.
                    SetMinMaxValues(context, min, max);

                    for (decimal d = _minValue; d <= _maxValue; d += _majorIntervalValue)
                    {
                        SizeF stringSize = g.MeasureString(d.ToString(), labelFont);
                        _categoryLabelWidth = Math.Max(_categoryLabelWidth, (int)stringSize.Width);
                    }
                }
                else if (chart.Categories != null && chart.Categories.Count > 0)
                {
                    _categoryLabelWidth = GetMaxCategoriesWidth(chart.Categories, g, labelFont, 0);
                }
            }

            // Find the tick mark and axis position for the axis.
            int majorTickStart, majorTickWidth, minorTickStart, minorTickWidth, axisPos, axisWidth;
            AdjustAxisPosition(GetAxisThickness(context), out majorTickStart, out majorTickWidth,
                out minorTickStart, out minorTickWidth,
                out axisPos, out axisWidth);

            return _categoryLabelWidth + axisWidth;
        }

        /// <summary>
        /// Find the width of the widest string in this category list
        /// </summary>
        /// <param name="categories"></param>
        /// <param name="g"></param>
        /// <param name="font"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private int GetMaxCategoriesWidth(Categories categories, Graphics g, Font font, int offset)
        {
            SizeF stringSize;
            int maxWidth = 0;

            foreach (Category cat in categories)
            {
                stringSize = g.MeasureString(cat.Value, font);
                maxWidth = Math.Max(maxWidth, (int)stringSize.Width + offset);
                if (cat.Categories != null)
                    Math.Max(maxWidth, GetMaxCategoriesWidth(cat.Categories, g, font, offset + 20));
            }
            return maxWidth;
        }

        private void DrawScalarHorizontal(Chart chart,
            Rdl.Runtime.Context context,
            Graphics g,
            bool centered,
            int l, int t, int w, ref int h)
        {
            if (chart.CategoryGrouping == null || chart.CategoryGrouping.NextGrouping != null)
                throw new Exception("There must be exactly one grouping on a scalar axis.");

            // Find the tick mark sizes and axis position for the axis.
            int majorTickStart, majorTickHeight, minorTickStart, minorTickHeight, axisPos, axisHeight;
            chart.CategoryAxis.AdjustAxisPosition(GetAxisThickness(context), out majorTickStart, out majorTickHeight,
                out minorTickStart, out minorTickHeight,
                out axisPos, out axisHeight);

            // Find the min and max values from the data.
            decimal min = 0;
            decimal max = 0;
            foreach (Category cat in chart.Categories)
            {
                min = Math.Min(min, decimal.Parse(cat.Value));
                max = Math.Max(max, decimal.Parse(cat.Value));
            }

            // Adjust the min and max values and find int intervals.
            SetMinMaxValues(context, min, max);

            int majorIntervals = (int)((_maxValue - _minValue) / _majorIntervalValue);
            int minorIntervals = (int)((_maxValue - _minValue) / _minorIntervalValue);

            Color color = Style.W32Color(chart.CategoryAxis.Style.Color(context));
            Brush brush = new SolidBrush(color);
            int textHeight = 0;
            if (chart.CategoryAxis.Visible)
            {
                Font font = chart.CategoryAxis.Style.GetWindowsFont(context);

                // Determine if the label text needs to be vertical.
                bool vertical = false;
                int textSize = 0;
                for (decimal d = _minValue; d <= _maxValue; d += _majorIntervalValue)
                {
                    SizeF sizeF = g.MeasureString(d.ToString() + " ", font);
                    textSize += (int)sizeF.Width;
                    textHeight = Math.Max(textHeight, (int)sizeF.Height);
                }

                if (textSize > w)
                {
                    // If we are drawing vertically then determine how much height is required.
                    textSize = 0;
                    for (decimal d = _minValue; d <= _maxValue; d += _majorIntervalValue)
                    {
                        SizeF sizeF = g.MeasureString(d.ToString() + " ", font);
                        textHeight = Math.Max(textHeight, (int)sizeF.Height);
                    }

                    vertical = true;
                }

                // Draw in the label text
                decimal value = min;
                for (int i = 0; i < majorIntervals; i++)
                {
                    SizeF sizeF = g.MeasureString(value.ToString(), font, 0, (vertical) ? new StringFormat(StringFormatFlags.DirectionVertical) : StringFormat.GenericDefault);

                    System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                    path.AddString(value.ToString(), font.FontFamily, (int)font.Style, font.Size, new Point(0, 0), StringFormat.GenericDefault);
                    if (vertical)
                    {
                        System.Drawing.Drawing2D.Matrix matrix = new System.Drawing.Drawing2D.Matrix();
                        matrix.Rotate(-30);
                        path.Transform(matrix);
                    }
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.DrawPath(new Pen(brush), path);

                    //g.DrawString(value.ToString(), font, brush,
                    //    l + (i * w / majorIntervals) - ((int)sizeF.Width / 2), h - textHeight,
                    //    (vertical) ? new StringFormat(StringFormatFlags.DirectionVertical) : StringFormat.GenericDefault);
                }

                h -= textHeight;
            }

            // Draw the major tick marks and/or the grid lines.
            // Get the color for the major grid lines.
            Color majorGridColor = color;
            if (chart.CategoryAxis.MajorGridLines != null)
                majorGridColor = Style.W32Color(chart.CategoryAxis.MajorGridLines.Style.Color(context));
            for (int i = 0; i < majorIntervals; i++)
            {
                int tickThickness = 1;
                if (chart.ValueAxis.MajorGridLines != null)
                {
                    if (chart.ValueAxis.MajorGridLines.ShowGridLines)
                        majorTickHeight = h - (axisPos - l) - majorTickStart;
                    tickThickness = (int)chart.CategoryAxis.MajorGridLines.Style.BorderWidth.Left(context).points;
                }
                g.DrawLine(new Pen(new SolidBrush(majorGridColor), tickThickness),
                    new Point(l + (i * w / majorIntervals), t + h - axisPos - majorTickStart),
                    new Point(l + (i * w / majorIntervals), t + h - axisPos - majorTickStart));
            }

            // Draw the minor tick marks and/or the grid lines.
            Color minorGridColor = color;
            if (chart.CategoryAxis.MinorGridLines != null)
                minorGridColor = Style.W32Color(chart.CategoryAxis.MinorGridLines.Style.Color(context));
            for (int i = 0; i < majorIntervals; i++)
            {
                int tickThickness = 1;
                if (chart.ValueAxis.MinorGridLines != null)
                {
                    if (chart.ValueAxis.MinorGridLines.ShowGridLines)
                        minorTickHeight = h - (axisPos - l) - minorTickStart;
                    tickThickness = (int)chart.ValueAxis.MinorGridLines.Style.BorderWidth.Left(context).points;
                }
                g.DrawLine(new Pen(new SolidBrush(minorGridColor), tickThickness),
                    new Point(l + (i * w / majorIntervals), t + h - axisPos - minorTickStart),
                    new Point(l + (i * w / majorIntervals), t + h - axisPos - minorTickStart));
            }

            // Adjust the height by the height of the axis labels and the axis line.
            h -= axisHeight;
        }

        void DrawNonScalarHorizontal(Chart chart,
            Rdl.Runtime.Context context,
            Graphics g,
            bool centered,
            int l, int t, int w, ref int h)
        {
            // Find the tick mark sizes and axis position for the axis.
            int majorTickStart, majorTickHeight, minorTickStart, minorTickHeight, axisPos, axisHeight;
            chart.CategoryAxis.AdjustAxisPosition(GetAxisThickness(context), out majorTickStart, out majorTickHeight,
                out minorTickStart, out minorTickHeight,
                out axisPos, out axisHeight);

            // Get a temporary bitmap to draw the axis onto
            Bitmap bmAxis = new Bitmap(w + l, h);
            Graphics gAxis = Graphics.FromImage(bmAxis);
            Brush brush = new SolidBrush(Style.W32Color(chart.CategoryAxis.Style.Color(context)));

            // Draw the axis line at the top of the bitmap
            Color color = Style.W32Color(chart.ValueAxis.Style.Color(context));
            gAxis.DrawLine(new Pen(new SolidBrush(color), GetAxisThickness(context)),
                new Point(l, 0), new Point(l+w, 0));

            int labelHeight = 0;
            List<int> majorMarks = new List<int>();
            List<int> minorMarks = new List<int>();

            if (chart.CategoryAxis.Visible && chart.CategoryGrouping != null)
            {
                labelHeight = DrawCategoryOntoHorizontalAxis(chart.Categories,
                     gAxis, brush, context, chart.CategoryAxis,
                     centered,
                     0, l, axisHeight, w,
                     majorMarks, minorMarks);
            }
            gAxis.Dispose();

            // Fill in the background of the plot area.
            g.FillRectangle(new SolidBrush(Style.W32Color(chart.PlotAreaStyle.BackgroundColor(context))),
                l, t, w, h - (labelHeight + axisHeight));

            // Copy the temporary bitmap back onto the chart.
            g.DrawImage(bmAxis,
                new System.Drawing.Rectangle(0, t + h - (labelHeight + axisHeight), l+w, labelHeight + axisHeight),
                new System.Drawing.Rectangle(0, 0, l+w, labelHeight + axisHeight),
                GraphicsUnit.Pixel);

            // Draw the major tick marks and/or the grid lines.
            // Get the color for the major grid lines.
            Color majorGridColor = color;
            if (chart.CategoryAxis.MajorGridLines != null)
                majorGridColor = Style.W32Color(chart.ValueAxis.MajorGridLines.Style.Color(context));
            foreach (int markPos in majorMarks)
            {
                int tickThickness = 1;
                if (chart.ValueAxis.MajorGridLines != null)
                {
                    if (chart.ValueAxis.MajorGridLines.ShowGridLines)
                        majorTickHeight = h - labelHeight - (axisPos - l) - majorTickStart;
                    tickThickness = (int)chart.CategoryAxis.MajorGridLines.Style.BorderWidth.Left(context).points;
                }
                g.DrawLine(new Pen(new SolidBrush(majorGridColor), tickThickness),
                    new Point(l + markPos, t + h - labelHeight - axisPos - majorTickStart),
                    new Point(l + markPos, t + h - labelHeight - axisPos - majorTickStart));
            }

            // Draw the minor tick marks and/or the grid lines.
            Color minorGridColor = color;
            if (chart.CategoryAxis.MinorGridLines != null)
                minorGridColor = Style.W32Color(chart.ValueAxis.MinorGridLines.Style.Color(context));
            foreach (int markPos in minorMarks)
            {
                int tickThickness = 1;
                if (chart.ValueAxis.MinorGridLines != null)
                {
                    if (chart.ValueAxis.MinorGridLines.ShowGridLines)
                        minorTickHeight = h - labelHeight - (axisPos - l) - minorTickStart;
                    tickThickness = (int)chart.CategoryAxis.MinorGridLines.Style.BorderWidth.Left(context).points;
                }
                g.DrawLine(new Pen(new SolidBrush(minorGridColor), tickThickness),
                    new Point(l + markPos, t + h - labelHeight - axisPos - minorTickStart),
                    new Point(l + markPos, t + h - labelHeight - axisPos - minorTickStart));
            }

            // Adjust the height by the height of the axis labels and the axis line.
            h -= axisHeight + labelHeight;
        }

        private int DrawCategoryOntoHorizontalAxis(Categories categories,
            Graphics g,
            Brush brush,
            Rdl.Runtime.Context context,
            Axis axis,
            bool centered,
            int level,
            int left, int top, int w,
            List<int> majorMarks, List<int> minorMarks)
        {
            Font font = axis.Style.GetWindowsFont(context);
            float yMult = (float)g.DpiY / 72;

            // Determine if the labels need to be vertical and find the height required for the labels.
            bool vertical = false;
            int labelHeight = (int)(font.Height * yMult);
            int adjustment = (centered) ? 0 : 1;
            foreach (Category cat in categories)
            {
                SizeF size = g.MeasureString(cat.Value, font);

                // Find the total members below this point.
                int membersAtThisPoint = 1;
                if (cat.Categories != null)
                    membersAtThisPoint = cat.Categories.LeafCategories.Count;

                // If we exceed the available width then go to vertical labels.
                if (_totalCategories > 1 &&
                    size.Width > (w * membersAtThisPoint / (_totalCategories - adjustment)))
                {
                    vertical = true;
                    foreach (Category cat2 in categories)
                    {
                        size = g.MeasureString(cat2.Value, font, 0, new StringFormat(StringFormatFlags.DirectionVertical));
                        labelHeight = Math.Max(labelHeight, (int)size.Height);
                    }
                    break;
                }
                labelHeight = Math.Max(labelHeight, (int)size.Height);
            }

            //Draw the labels at this category level.
            int totalHeight = labelHeight;
            foreach (Category cat in categories)
            {
                // Find the total members below this point.
                int membersAtThisPoint = 1;
                if (cat.Categories != null)
                    membersAtThisPoint = cat.Categories.LeafCategories.Count;

                // Find the width of this group entry
                int groupEntryWidth = w;
                if (_totalCategories > 1)
                    groupEntryWidth = (int)(membersAtThisPoint * w / (_totalCategories - adjustment));

                // Measure the space needed for the label
                //SizeF size = g.MeasureString(cat.Value, font, 0, (vertical) ? new StringFormat(StringFormatFlags.DirectionVertical) : StringFormat.GenericDefault);
                SizeF size = g.MeasureString(cat.Value, font, 0, StringFormat.GenericDefault);

                int center = left;
                if (centered)
                    center += groupEntryWidth / 2;
                if (vertical)
                {
                    System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                    path.AddString(cat.Value, font.FontFamily, (int)font.Style, font.Size * yMult,
                        new Point(center - ((int)size.Width / 2), top),
                        StringFormat.GenericDefault);
                    if (vertical)
                    {
                        System.Drawing.Drawing2D.Matrix matrix = new System.Drawing.Drawing2D.Matrix();
                        matrix.Translate(0 - (size.Width / 2), 0);
                        matrix.RotateAt(-30, new PointF(center, top), System.Drawing.Drawing2D.MatrixOrder.Append);
                        path.Transform(matrix);
                    }
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.DrawPath(new Pen(brush), path);
                }
                else
                    g.DrawString(cat.Value, font, brush,
                        center - (int)size.Width / 2, top, StringFormat.GenericDefault);



                // Draw this label.
                //if (vertical)
                //{
                //    g.TranslateTransform(0, size.Width);
                //    g.RotateTransform(-30);
                //}
                //g.DrawString(cat.Value, font, brush, center - size.Width / 2, top, StringFormat.GenericDefault);
                //return totalHeight;
                //g.ResetTransform();

                

                // Add in the tick mark position for this entry.
                if (level == 0)
                    majorMarks.Add(left);
                else
                    minorMarks.Add(left);

                // Recurse down to draw stuff below this group.
                if (cat.Categories != null)
                {
                    int h = DrawCategoryOntoHorizontalAxis(cat.Categories, g, brush, context, axis, centered,
                        level + 1, left, top + labelHeight, groupEntryWidth, majorMarks, minorMarks);
                    totalHeight = Math.Max(totalHeight, labelHeight + h);
                }

                // Adjust the left to one group entry over.
                left += groupEntryWidth;
            }
            return totalHeight;
        }

        private void DrawCategoryOntoVerticalAxis(Categories categories,
            Graphics g,
            Brush brush,
            Font labelFont,
            Rdl.Runtime.Context context,
            bool centered,
            int l, int t, int h, int offset,
            int axisPos,
            Pen majorGridPen, int majorTickStart, int majorTickWidth,
            Pen minorGridPen, int minorTickStart, int minorTickWidth
            )
        {
            //Draw the labels at this category level.
            int i = 0;
            foreach (Category cat in categories)
            {
                // Find the total members below this point.
                int membersAtThisPoint = 1;
                if (cat.Categories != null)
                    membersAtThisPoint = cat.Categories.LeafCategories.Count;

                int top = t + h - (int)(i * ((float)h / _totalCategories));
                if (Visible)
                {
                    int labelTop = top;
                    if (centered)
                        labelTop -= (int)(membersAtThisPoint * ((float)h / _totalCategories) / 2);
                    SizeF stringSize = g.MeasureString(cat.Value, labelFont);
                    g.DrawString(cat.Value, labelFont, brush, l + offset + _categoryLabelWidth - stringSize.Width,
                        labelTop - (stringSize.Height / 2));
                }

                // Draw the tick mark
                if (offset == 0 && majorTickWidth > 0)
                {
                    g.DrawLine(majorGridPen, 
                        new Point(l + _categoryLabelWidth + axisPos + majorTickStart, top),
                        new Point(l + _categoryLabelWidth + axisPos + majorTickStart + majorTickWidth, top));
                }
                else if (offset != 0 && minorTickWidth > 0)
                {
                    g.DrawLine(minorGridPen,
                        new Point(l + _categoryLabelWidth + axisPos + minorTickStart, top),
                        new Point(l + _categoryLabelWidth + axisPos + minorTickStart + minorTickWidth, top));
                }

                // Find the height of this group entry
                int groupEntryHeight = h;
                if (_totalCategories > 1)
                    groupEntryHeight = (int)(membersAtThisPoint * h / _totalCategories);

                // Recurse down to draw stuff below this group.
                if (cat.Categories != null)
                    DrawCategoryOntoVerticalAxis(cat.Categories, g, brush, labelFont, context,
                        centered, l + 20, top - groupEntryHeight, groupEntryHeight, offset + 20,
                        axisPos, majorGridPen, majorTickStart, majorTickWidth,
                        minorGridPen, minorTickStart, minorTickWidth);

                i += membersAtThisPoint;
            }
        }
    }
}
