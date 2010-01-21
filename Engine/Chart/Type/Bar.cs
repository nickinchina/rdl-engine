using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Rdl.Engine.Chart.Type
{
    internal class Bar
    {
        private const int _maxBarHeight = 20;

        public Bar(Chart chart,
            Rdl.Runtime.Context context, 
            Graphics g, 
            int l, int t, int w, int h)
        {
            
            // Draw in the axis titles
            if (chart.ValueAxis.Title != null)
                chart.ValueAxis.Title.DrawAtBottom(g, context, ref l, ref t, ref w, ref h);
            if (chart.CategoryAxis.Title != null)
                chart.CategoryAxis.Title.DrawAtLeft(g, context, ref l, ref t, ref w, ref h);

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

            // Find the width of the category axis so the value axis can be drawn to the correct width.
            int categoryAxisWidth = chart.CategoryAxis.Width(chart, context, g);

            // Draw the value axis
            chart.ValueAxis.DrawHorizontal(chart, context, g, true, l + categoryAxisWidth, t, w - categoryAxisWidth, ref h);

            // Draw the value axis now that we know the height of the category axis.
            chart.CategoryAxis.DrawVertical(chart, context, g, true, ref l, t, ref w, h);

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
            Brush brush = new SolidBrush(Palette.GetColor(chart.Palette,0));
            t = t + h - (h / chart.CategoryAxis.TotalCategories);
            if (chart.Categories != null)
            {
                foreach (Category cat in chart.LeafCategories)
                {
                    if (chart.SeriesGrouping != null)
                    {
                        DrawSeriesBars(chart, cat.Context, g, l, t, w, (h / chart.CategoryAxis.TotalCategories),
                            minValue, maxValue, cat.CategoryIndex);
                    }
                    else
                    {
                        decimal value = chart.GetDataValue(cat.Context, 0, cat.CategoryIndex);
                        DrawBar(g, l, t, w, h / chart.LeafCategories.Count,
                            brush,
                            (value - minValue) / (maxValue - minValue));
                    }
                    t -= (h / chart.CategoryAxis.TotalCategories);
                }
            }
            else if (chart.SeriesGrouping != null)
            {
                DrawSeriesBars(chart, context, g, l, t, w , h, minValue, maxValue, 0);
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
            int i=0;
            int right = h;

            decimal seriesTotal = 0;
            if (chart.SubType == Chart.SubtypeEnum.PercentStacked)
                foreach(Series ser in chart.LeafSeriesList)
                    seriesTotal += chart.GetDataValue(context.Intersect(ser.Context), ser.SeriesIndex, categoryIndex);

            foreach(Series ser in chart.LeafSeriesList)
            {
                decimal value = chart.GetDataValue(context.Intersect(ser.Context), ser.SeriesIndex, categoryIndex);
                int width;

                Brush brush = new SolidBrush(chart.SeriesColor(ser.CombinedValue));
                if (chart.SubType == Chart.SubtypeEnum.Plain)
                {
                    width = (int)(w * (value - minValue) / (maxValue - minValue));
                    DrawBar(g, t, l, width, h, brush, 1);
                    t += h / chart.LeafSeriesList.Count;
                }

                if (chart.SubType == Chart.SubtypeEnum.Stacked)
                {
                    width = (int)(w * (value - minValue) / (maxValue - minValue));
                    DrawBar(g, l, t, width, h, brush, 1);
                    l += width;
                }

                if (chart.SubType == Chart.SubtypeEnum.PercentStacked)
                {
                    width = (int)(w * value / seriesTotal);
                    DrawBar(g, l, t, width, h, brush, 1);
                }

                i++;
            }
        }

        private void DrawBar(Graphics g, int l, int t, int w , int h,
            Brush brush,
            decimal percentage)
        {
            if (h > _maxBarHeight)
            {
                t += (h / 2) - (_maxBarHeight / 2);
                h = _maxBarHeight;
            }
            g.FillRectangle(brush, l, t, (int)(w * percentage), h);
        }
    }
}
