using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Rdl.Engine.Chart.Type
{
    internal class Column
    {
        private const int _maxBarWidth = 20;

        public Column(Chart chart,
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
            if (chart.SubType == Chart.SubtypeEnum.PercentStacked)
                maxValue = 100;
            else if (chart.SubType == Chart.SubtypeEnum.Plain)
                chart.FindMinMaxValues(context, ref minValue, ref maxValue, false);
            else if (chart.SubType == Chart.SubtypeEnum.Stacked)
                chart.FindMinMaxValues(context, ref minValue, ref maxValue, true);

            // Adjust the min and max values and find intervals.
            chart.ValueAxis.SetMinMaxValues(context, minValue, maxValue);

            // Find the width of the value axis so the category axis can be drawn to the correct width.
            int valueAxisWidth = chart.ValueAxis.Width(chart, context, g);

            // Draw the category axis
            chart.CategoryAxis.DrawHorizontal(chart, context, g, true, l + valueAxisWidth, t, w - valueAxisWidth, ref h);

            // Draw the value axis now that we know the height of the category axis.
            chart.ValueAxis.DrawVertical(chart, context, g, ref l, t, ref w, h);

            // Draw the bars.
            if (chart.CategoryAxis.Scalar)
                DrawScalarBars(chart, context, g, l, t, w, h,
                    chart.ValueAxis.Min, chart.ValueAxis.Max);
            else
                DrawNonScalarBars(chart, context, g, l, t, w, h,
                    chart.ValueAxis.Min, chart.ValueAxis.Max);
        }

        private void DrawScalarBars(
            Chart chart,
            Rdl.Runtime.Context context,
            Graphics g,
            int l, int t, int w, int h,
            decimal minValue, decimal maxValue)
        {
        }

        private void DrawNonScalarBars(
            Chart chart,
            Rdl.Runtime.Context context,
            Graphics g,
            int l, int t, int w, int h,
            decimal minValue, decimal maxValue)
        {
            Brush brush = new SolidBrush(Palette.GetColor(chart.Palette, 0));
            if (chart.Categories != null)
            {
                foreach (Category cat in chart.LeafCategories)
                {
                    if (chart.SeriesGrouping != null)
                    {
                        DrawSeriesBars(chart, cat.Context, g, l, t, (w / chart.CategoryAxis.TotalCategories), h,
                            minValue, maxValue, cat.CategoryIndex);
                    }
                    else
                    {
                        decimal value = chart.GetDataValue(cat.Context, 0, cat.CategoryIndex);
                        DrawBar(g, l, t, w / chart.LeafCategories.Count, h,
                            brush,
                            (value - minValue) / (maxValue - minValue));
                    }
                    l += (w / chart.CategoryAxis.TotalCategories);
                }
            }
            else if (chart.SeriesGrouping != null)
            {
                DrawSeriesBars(chart, context, g, l, t, w, h, minValue, maxValue, 0);
            }
            else
            {
                decimal value = chart.GetDataValue(context, 0, 0);
                DrawBar(g, l, t, w, h,
                    brush,
                    (value - minValue) / (maxValue - minValue));
            }
        }

        private void DrawSeriesBars(
            Chart chart,
            Rdl.Runtime.Context context,
            Graphics g,
            int l, int t, int w, int h,
            decimal minValue, decimal maxValue,
            int categoryIndex)
        {
            int i = 0;
            int bot = h;

            decimal seriesTotal = 0;
            if (chart.SubType == Chart.SubtypeEnum.PercentStacked)
                foreach (Series ser in chart.LeafSeriesList)
                    seriesTotal += chart.GetDataValue(context.Intersect(ser.Context), ser.SeriesIndex, categoryIndex);

            foreach (Series ser in chart.LeafSeriesList)
            {
                decimal value = chart.GetDataValue(context.Intersect(ser.Context), ser.SeriesIndex, categoryIndex);
                int height;

                Brush brush = new SolidBrush(chart.SeriesColor(ser.CombinedValue));
                if (chart.SubType == Chart.SubtypeEnum.Plain)
                {
                    height = (int)(h * (value - minValue) / (maxValue - minValue));
                    DrawBar(g, l, t + bot - height, w / chart.LeafSeriesList.Count, height, brush, 1);
                    l += w / chart.LeafSeriesList.Count;
                }

                if (chart.SubType == Chart.SubtypeEnum.Stacked)
                {
                    height = (int)(h * (value - minValue) / (maxValue - minValue));
                    DrawBar(g, l, t + bot - height, w, height, brush, 1);
                    bot -= height;
                }

                if (chart.SubType == Chart.SubtypeEnum.PercentStacked)
                {
                    height = (int)(h * value / seriesTotal);
                    DrawBar(g, l, t + bot - height, w, height, brush, 1);
                    bot -= height;
                }

                i++;
            }
        }

        private void DrawBar(Graphics g, int l, int t, int w, int h,
            Brush brush,
            decimal percentage)
        {
            if (w > _maxBarWidth)
            {
                l += (w / 2) - (_maxBarWidth / 2);
                w = _maxBarWidth;
            }
            g.FillRectangle(brush, l, t + (h - (int)(h * percentage)), w, (int)(h * percentage));
        }
    }
}
