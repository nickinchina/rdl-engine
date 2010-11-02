using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Rdl.Engine.Chart.Type
{
    class Line
    {
        public Line(Chart chart,
            Rdl.Runtime.Context context, 
            Graphics g, 
            int l, int t, int w, int h)
        {
            // Draw in the axis titles
            if (chart.ValueAxis.Title != null)
                chart.ValueAxis.Title.DrawAtLeft(g, context, ref l, ref t, ref w, ref h);
            if (chart.CategoryAxis.Title != null)
                chart.CategoryAxis.Title.DrawAtBottom(g, context, ref l, ref t, ref w, ref h);

            // Find the min and max values from the data.  These values depend on the chart subtype.
            decimal minValue = 0, maxValue = 0;
            chart.FindMinMaxValues(context, ref minValue, ref maxValue, false);

            // Adjust the min and max values and find intervals.
            chart.ValueAxis.SetMinMaxValues(context, minValue, maxValue);

            // Find the width of the value axis so the category axis can be drawn to the correct width.
            int valueAxisWidth = chart.ValueAxis.Width(chart, context, g);

            // Draw the category axis
            chart.CategoryAxis.DrawHorizontal(chart, context, g, false, l + valueAxisWidth, t, w - valueAxisWidth, ref h);

            // Draw the value axis now that we know the height of the category axis.
            chart.ValueAxis.DrawVertical(chart, context, g, ref l, t, ref w, h);

            // Draw the lines.
            if (chart.SeriesGrouping != null)
            {
                foreach (Series ser in chart.LeafSeriesList)
                {
                    DrawLine(chart, ser.Context, g, 
                        new SolidBrush(chart.SeriesColor(ser.CombinedValue)),
                        ser.SeriesIndex,
                        l, t, w, h, chart.ValueAxis.Min, chart.ValueAxis.Max);
                }
            }
            else
            {
                DrawLine(chart, context, g, 
                    new SolidBrush(Style.W32Color(chart.PlotAreaStyle.Color(context))),
                    1,
                    l, t, w, h, chart.ValueAxis.Min, chart.ValueAxis.Max);
            }
        }

        private void DrawLine(
            Chart chart,
            Rdl.Runtime.Context context,
            Graphics g,
            Brush brush,
            int seriesIndex,
            int l, int t, int w, int h,
            decimal minValue, decimal maxValue)
        {
            int lastX = 0;
            int lastY = 0;
            bool firstValue = true;

            if (chart.Categories != null)
            {
                foreach (Category cat in chart.LeafCategories)
                {
                    Rdl.Runtime.Context ctxt = context.Intersect(cat.Context);
                    if (ctxt.Rows.Count == 0)
                        firstValue = true;
                    else
                    {
                        decimal value = chart.GetDataValue(ctxt, seriesIndex, cat.CategoryIndex);

                        int height = (int)(h * (value - minValue) / (maxValue - minValue));
                        g.FillEllipse(brush, new System.Drawing.Rectangle(l - 3, t + h - height - 3, 6, 6));

                        if (!firstValue)
                        {
                            g.DrawLine(new Pen(brush, 1), new Point(lastX, lastY), new Point(l, t + h - height));
                        }

                        lastY = t + h - height;
                        lastX = l;
                        firstValue = false;
                    }

                    l += (w / (chart.CategoryAxis.TotalCategories-1));
                }
            }
        }

    }
}
