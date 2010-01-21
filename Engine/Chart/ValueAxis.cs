using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Drawing;

namespace Rdl.Engine.Chart
{
    class ValueAxis : Axis
    {
        private int _labelWidth = 0;

        public ValueAxis(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        internal int Width(Chart chart,
            Rdl.Runtime.Context context,
            Graphics g)
        {
            int majorIntervals = (int)((_maxValue - _minValue) / _majorIntervalValue);
            int minorIntervals = (int)((_maxValue - _minValue) / _minorIntervalValue);

            // Find the maximum string width of the value data.
            GetValueLableWidth(context, g, majorIntervals);

            // Find the tick mark and axis position for the axis.
            int majorTickStart, majorTickWidth, minorTickStart, minorTickWidth, axisPos, axisWidth;
            AdjustAxisPosition(GetAxisThickness(context), out majorTickStart, out majorTickWidth,
                out minorTickStart, out minorTickWidth,
                out axisPos, out axisWidth);

            return _labelWidth + axisWidth;
        }

        private void GetValueLableWidth(Rdl.Runtime.Context context, System.Drawing.Graphics g, int majorIntervals)
        {
            // Find the maximum string width of the value data.
            _labelWidth = 0;
            if (Visible)
            {
                // Determine the amount of space required for the axis labels.
                SizeF stringSize;
                Font labelFont = Style.GetWindowsFont(context);

                for (int i = 0; i <= majorIntervals; i++)
                {
                    decimal dValue = _minValue + (i * _majorIntervalValue);
                    stringSize = g.MeasureString(dValue.ToString(), labelFont);
                    _labelWidth = Math.Max(_labelWidth, (int)stringSize.Width);
                }
            }
        }

        internal void DrawVertical(Chart chart,
            Rdl.Runtime.Context context,
            Graphics g,
            ref int l, int t, ref int w, int h)
        {
            int majorIntervals = (int)((_maxValue - _minValue) / _majorIntervalValue);
            int minorIntervals = (int)((_maxValue - _minValue) / _minorIntervalValue);

            // Find the tick mark and axis position for the axis.
            int majorTickStart, majorTickWidth, minorTickStart, minorTickWidth, axisPos, axisWidth;
            AdjustAxisPosition(GetAxisThickness(context), out majorTickStart, out majorTickWidth,
                out minorTickStart, out minorTickWidth,
                out axisPos, out axisWidth);

            // If we are showing the grid lines then adjust the tick width to include the whole chart area.
            if (MajorGridLines.ShowGridLines)
                majorTickWidth = w - _labelWidth - axisPos - majorTickStart;
            if (MinorGridLines.ShowGridLines)
                minorTickWidth = w - _labelWidth - axisPos - minorTickStart;

            // Draw the axis line
            Color color = Style.W32Color(Style.Color(context));
            Brush brush = new SolidBrush(color);
            Pen majorGridPen = new Pen(brush, 1);
            if (MajorGridLines != null)
                majorGridPen = new Pen(new SolidBrush(Style.W32Color(MajorGridLines.Style.Color(context))),
                    (int)MajorGridLines.Style.BorderWidth.Left(context).points);
            Pen minorGridPen = new Pen(brush, 1);
            if (MinorGridLines != null)
                minorGridPen = new Pen(new SolidBrush(Style.W32Color(MinorGridLines.Style.Color(context))),
                    (int)MinorGridLines.Style.BorderWidth.Left(context).points);

            g.DrawLine(new Pen(brush, GetAxisThickness(context)),
                new Point(l + _labelWidth + axisPos + (GetAxisThickness(context)>>1), t),
                new Point(l + _labelWidth + axisPos + (GetAxisThickness(context) >> 1), t + h));

            Font labelFont = chart.ValueAxis.Style.GetWindowsFont(context);

            float majorEntryHeight = (float)h / majorIntervals;
            float minorEntryHeight = (float)h / minorIntervals;
            for (int i = 0; i <= majorIntervals; i++)
            {
                int top = t + h - (int)(i * majorEntryHeight);
                if (Visible)
                {
                    // Determine the amount of space required for the axis labels.
                    SizeF stringSize;

                    // Draw the axis labels.
                    decimal dValue = _minValue + (i * _majorIntervalValue);
                    stringSize = g.MeasureString(dValue.ToString().ToString(), labelFont);
                    g.DrawString(dValue.ToString(), labelFont, new SolidBrush(color),
                        l + _labelWidth - (int)stringSize.Width - 1,
                        top - ((int)stringSize.Height / 2));
                }

                if (i > 0)
                {
                    // Draw the major tick marks
                    if (majorTickWidth > 0)
                        g.DrawLine(majorGridPen,
                            new Point(l + _labelWidth + axisPos + majorTickStart, top),
                            new Point(l + _labelWidth + axisPos + majorTickStart + majorTickWidth, top));

                    // Draw the minor tick marks
                    if (minorTickWidth > 0)
                        for (int j = 1; j < minorIntervals / majorIntervals; j++)
                        {
                            g.DrawLine(minorGridPen,
                                new Point(l + _labelWidth + axisPos + minorTickStart, top - (int)(j * minorEntryHeight)),
                                new Point(l + _labelWidth + axisPos + minorTickStart + majorTickWidth, top - (int)(j * minorEntryHeight)));
                        }
                }
            }
            l += _labelWidth + axisWidth;
            w -= _labelWidth + axisWidth;
        }

        internal void DrawHorizontal(Chart chart,
            Rdl.Runtime.Context context,
            Graphics g,
            bool centered,
            int l, int t, int w, ref int h)
        {
            int majorIntervals = (int)((_maxValue - _minValue) / _majorIntervalValue);
            int minorIntervals = (int)((_maxValue - _minValue) / _minorIntervalValue);

            // Find the tick mark and axis position for the axis.
            int majorTickStart, majorTickHeight, minorTickStart, minorTickHeight, axisPos, axisWidth;
            AdjustAxisPosition(GetAxisThickness(context), out majorTickStart, out majorTickHeight,
                out minorTickStart, out minorTickHeight,
                out axisPos, out axisWidth);

            Color color = Style.W32Color(Style.Color(context));
            Brush brush = new SolidBrush(color);
            Pen majorGridPen = new Pen(brush, 1);
            if (MajorGridLines != null)
                majorGridPen = new Pen(new SolidBrush(Style.W32Color(MajorGridLines.Style.Color(context))), 
                    (int)MajorGridLines.Style.BorderWidth.Left(context).points);
            Pen minorGridPen = new Pen(brush, 1);
            if (MinorGridLines != null)
                minorGridPen = new Pen(new SolidBrush(Style.W32Color(MinorGridLines.Style.Color(context))), 
                    (int)MinorGridLines.Style.BorderWidth.Left(context).points);

            Font font = Style.GetWindowsFont(context);
            bool vertical = false;
            float yMult = (float)g.DpiY / 72;
            int labelHeight = 0;
            float majorEntryWidth = (float)w / majorIntervals;
            float minorEntryWidth = (float)w / minorIntervals;

            // Determine how much vertical space is required for the axis labels.
            if (Visible)
            {
                // Determine if the labels need to be vertical and find the height required for the labels.
                labelHeight = (int)(font.Height * yMult);

                // Determine the amount of space required for the axis labels.
                GetValueLableWidth(context, g, majorIntervals);

                if (_labelWidth > (int)majorEntryWidth)
                {
                    labelHeight = _labelWidth;
                    vertical = true;
                }
            }

            // If we are showing the grid lines then adjust the tick width to include the whole chart area.
            if (MajorGridLines.ShowGridLines)
                majorTickHeight = h - labelHeight - axisPos - majorTickStart;
            if (MinorGridLines.ShowGridLines)
                minorTickHeight = h - labelHeight - axisPos - minorTickStart;

            // Fill in the background of the plot area.
            g.FillRectangle(new SolidBrush(Style.W32Color(chart.PlotAreaStyle.BackgroundColor(context))),
                l, t, w, h - (labelHeight + axisWidth));

            // Draw the axis line 
            g.DrawLine(new Pen(new SolidBrush(color), GetAxisThickness(context)),
                new Point(l, t + h - labelHeight - axisPos - (GetAxisThickness(context) >> 1)),
                new Point(l + w, t + h - labelHeight - axisPos - (GetAxisThickness(context) >> 1)));

            // Loop through the major interval values 
            int left = l;
            for (int i = 0; i <= majorIntervals; i++)
            {
                left = l + (int)(i * majorEntryWidth);

                if (Visible)
                {
                    decimal dValue = _minValue + (i * _majorIntervalValue);

                    int center = left;
                    if (centered)
                        center += (int)(majorEntryWidth / 2);
                    SizeF size = g.MeasureString(dValue.ToString(), font);
                    if (vertical)
                    {
                        System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                        path.AddString(dValue.ToString(), font.FontFamily, (int)font.Style, font.Size * yMult,
                            new Point(center - ((int)size.Width / 2), t + h - labelHeight),
                            StringFormat.GenericDefault);
                        if (vertical)
                        {
                            System.Drawing.Drawing2D.Matrix matrix = new System.Drawing.Drawing2D.Matrix();
                            matrix.Translate(0 - (size.Width / 2), 0);
                            matrix.RotateAt(-30, new PointF(center, t + h - labelHeight), System.Drawing.Drawing2D.MatrixOrder.Append);
                            path.Transform(matrix);
                        }
                        g.DrawPath(new Pen(brush), path);
                    }
                    else
                        g.DrawString(dValue.ToString(), font, brush,
                            center - (int)size.Width / 2, t + h - labelHeight, StringFormat.GenericDefault);
                }

                if (i > 0)
                {
                    // Draw the major tick mark
                    if (majorTickHeight > 0)
                        g.DrawLine(majorGridPen,
                            new Point(left, t + h - labelHeight - (axisWidth >> 1) - majorTickStart),
                            new Point(left, t + h - labelHeight - (axisWidth >> 1) - majorTickStart - majorTickHeight));

                    // Draw the minor tick marks
                    if (minorTickHeight > 0)
                        for (int j = 1; j < minorIntervals / majorIntervals; j++)
                        {
                            g.DrawLine(minorGridPen,
                                new Point(left + (int)(j * minorEntryWidth), t + h - labelHeight - (axisWidth >> 1) + minorTickStart),
                                new Point(left + (int)(j * minorEntryWidth), t + h - labelHeight - (axisWidth >> 1) + minorTickStart - minorTickHeight));
                        }
                }
            }

            h -= labelHeight + ((axisWidth + GetAxisThickness(context)) >> 1);
        }
    }
}
