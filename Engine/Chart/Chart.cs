using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Drawing;

namespace Rdl.Engine.Chart
{
    internal class Chart : DataRegion
    {
        public enum TypeEnum
        {
            Column,
            Bar,
            Line,
            Pie,
            Scatter,
            Bubble,
            Area,
            Doughnut,
            Stock
        };

        public enum SubtypeEnum
        {
            Plain,
            Stacked,
            PercentStacked,
            Smooth,
            Exploded,
            Line,
            SmoothLine,
            HighLowClose,
            OpenHighLowClose,
            Candlestick
        };

        private TypeEnum _type = TypeEnum.Column;
        private SubtypeEnum _subType = SubtypeEnum.Plain;
        private SeriesGrouping _seriesGrouping = null;
        private SeriesList _seriesList = null;
        private SeriesList _leafSeriesList = null;
        private CategoryGrouping _categoryGrouping = null;
        private Categories _categories = null;
        private Categories _leafCategories = null;
        private List<ChartSeries> _chartData = new List<ChartSeries>();
        private Legend _legend = null;
        private CategoryAxis _categoryAxis = null;
        private ValueAxis _valueAxis = null;
        private Title _title = null;
        private int _pointWidth = 100;
        private Palette.PaletteEnum _palette = Rdl.Engine.Chart.Palette.PaletteEnum.Default;
        private ThreeDProperties _threeDProperties = null;
        private Style _plotAreaStyle = null;
        private Enums.DataElementOutputEnum _chartElementOutput = Enums.DataElementOutputEnum.Output;
        private Dictionary<string, Color> _seriesColors = new Dictionary<string, Color>();
        private const int _legendPadding = 10;

        public Chart(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
            // Make sure that default axis definitions exist.
            if (_valueAxis == null)
                _valueAxis = new ValueAxis(null, this);
            if (_categoryAxis == null)
                _categoryAxis = new CategoryAxis(null, this);
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            base.ParseAttribute(attr);
            switch (attr.Name.ToLower())
            {
                case "type":
                    _type = (TypeEnum)Enum.Parse(typeof(TypeEnum), attr.InnerText, true);
                    break;
                case "subtype":
                    _subType = (SubtypeEnum)Enum.Parse(typeof(SubtypeEnum), attr.InnerText, true);
                    break;
                case "seriesgroupings":
                    {
                        SeriesGrouping lastGrouping = null;

                        foreach (XmlNode child in attr.ChildNodes)
                        {
                            SeriesGrouping seriesGrouping = SeriesGrouping.GetSeriesGrouping(child, this);
                            if (_seriesGrouping == null)
                                _seriesGrouping = seriesGrouping;
                            else
                                lastGrouping.NextGrouping = seriesGrouping;
                            lastGrouping = seriesGrouping;
                        }
                    }
                    break;
                case "categorygroupings":
                    {
                        CategoryGrouping lastGrouping = null;

                        foreach (XmlNode child in attr.ChildNodes)
                        {
                            CategoryGrouping categoryGrouping = CategoryGrouping.GetCategoryGrouping(child, this);
                            if (_categoryGrouping == null)
                                _categoryGrouping = categoryGrouping;
                            else
                                lastGrouping.NextGrouping = categoryGrouping;
                            lastGrouping = categoryGrouping;
                        }
                    }
                    break;
                case "chartdata":
                    foreach (XmlNode child in attr.ChildNodes)
                        _chartData.Add(new ChartSeries(child, this));
                    break;
                case "legend":
                    _legend = new Legend(attr, this);
                    break;
                case "categoryaxis":
                    _categoryAxis = new CategoryAxis(attr.FirstChild, this);
                    break;
                case "valueaxis":
                    _valueAxis = new ValueAxis(attr.FirstChild, this);
                    break;
                case "title":
                    _title = new Title(attr, this);
                    break;
                case "pointwidth":
                    _pointWidth = int.Parse(attr.InnerText);
                    break;
                case "pallette":
                    _palette = (Palette.PaletteEnum)Enum.Parse(typeof(Palette.PaletteEnum), attr.InnerText, true);
                    break;
                case "threedproperties":
                    _threeDProperties = new ThreeDProperties(attr, this);
                    break;
                case "plotarea":
                    _plotAreaStyle = new Style(attr.FirstChild, this);
                    break;
                case "chartelementoutput":
                    _chartElementOutput = (Enums.DataElementOutputEnum)Enum.Parse(typeof(Enums.DataElementOutputEnum), attr.InnerText, true);
                    break;
                default:
                    break;
            }
        }

        protected override void Render1(Rdl.Render.Container parentBox, Rdl.Runtime.Context context, bool visible)
        {
            base.Render1(parentBox, context, visible);
            if (visible)
            {
                _box = parentBox.AddChartElement(this, Style, _context);
                _box.Name = _name;
                _box.KeepTogether = true;
            }
        }

        protected override void Render2(Rdl.Runtime.Context context)
        {
            base.Render2(context);
        }

        public CategoryAxis CategoryAxis
        {
            get { return _categoryAxis; }
        }

        public ValueAxis ValueAxis
        {
            get { return _valueAxis; }
        }

        public List<ChartSeries> ChartData
        {
            get { return _chartData; }
        }

        public Style PlotAreaStyle
        {
            get { return (_plotAreaStyle == null) ? Style : _plotAreaStyle; }
        }

        public SeriesGrouping SeriesGrouping
        {
            get { return _seriesGrouping; }
        }

        public CategoryGrouping CategoryGrouping
        {
            get { return _categoryGrouping; }
        }

        public Categories Categories
        {
            get { return _categories; }
        }

        public Categories LeafCategories
        {
            get { return _leafCategories; }
        }

        public SeriesList SeriesList
        {
            get { return _seriesList; }
        }

        public SeriesList LeafSeriesList
        {
            get { return _leafSeriesList; }
        }

        public Color SeriesColor(string name)
        {
            return _seriesColors[name];
        }

        public SubtypeEnum SubType
        {
            get { return _subType; }
        }

        public Palette.PaletteEnum Palette
        {
            get { return _palette; }
        }

        public System.Drawing.Image RenderChart(Rdl.Runtime.Context context, int width, int height, decimal xMult, decimal yMult)
        {
            if (width <= 0 || height <= 0)
                return null;
            Bitmap bm = new Bitmap(width, height);

            Graphics g = Graphics.FromImage(bm);
            g.PageUnit = GraphicsUnit.Pixel;

            int t = 0;
            int l = 0;
            int w = width;
            int h = height;

            if (context.Rows.Count == 0)
            {
                g.FillRectangle(new SolidBrush(Style.W32Color(Style.BackgroundColor(context))),
                    new System.Drawing.Rectangle(0, 0, w, h));
                g.DrawString("No Data", Style.GetWindowsFont(context), new SolidBrush(Style.W32Color(Style.Color(context))), new Point(0, 0));
                return bm;
            }

            System.Drawing.Rectangle clipRect = new System.Drawing.Rectangle(l, t, w, h);
            if (Style.BackgroundColor(context) != null)
                g.FillRectangle(new SolidBrush(Style.W32Color(Style.BackgroundColor(context))),
                    clipRect);

            if (Style.BorderWidth != null)
                Rdl.Render.Drawing.DrawBorder(g, new System.Drawing.Rectangle(l, t, w, h),
                    new Rdl.Render.BorderWidth(Style.BorderWidth, context), 
                    new Rdl.Render.BorderStyle(Style.BorderStyle, context), 
                    new Rdl.Render.BorderColor(Style.BorderColor, context),
                    xMult, yMult);

            if (Style != null)
            {
                l += (int)(Style.PaddingLeft(context).points * xMult);
                t += (int)(Style.PaddingTop(context).points * yMult);
                w -= (int)((Style.PaddingLeft(context).points + Style.PaddingRight(context).points) * xMult);
                h -= (int)((Style.PaddingTop(context).points + Style.PaddingBottom(context).points) * yMult);
            }


            // Draw in the title.
            if (_title != null)
                _title.DrawAtTop(g, context, ref l, ref t, ref w, ref h);

            // Get the categories.
            if (_categoryGrouping != null)
            {
                _categories = _categoryGrouping.GetCategories(context, null);
                _leafCategories = _categories.LeafCategories;
            }

            // Get the series
            if (_seriesGrouping != null)
            {
                _seriesList = _seriesGrouping.GetSeriesList(context, null);
                _leafSeriesList = _seriesList.LeafSeriesList;
            }

            // Draw in the Legend
            DrawLegend(context, g, bm, ref l, ref t, ref w, ref h, (_type == TypeEnum.Pie), xMult, yMult);

            // Draw the chart.
            switch (_type)
            {
                case TypeEnum.Area:
                    break;
                case TypeEnum.Bar:
                    Type.Bar barChart = new Type.Bar(this, context, g, l, t, w, h);
                    break;
                case TypeEnum.Bubble:
                    break;
                case TypeEnum.Column:
                    Type.Column columnChart = new Type.Column(this, context, g, l, t, w, h);
                    break;
                case TypeEnum.Doughnut:
                    break;
                case TypeEnum.Line:
                    Type.Line lineChart = new Type.Line(this, context, g, l, t, w, h);
                    break;
                case TypeEnum.Pie:
                    break;
                case TypeEnum.Scatter:
                    break;
                case TypeEnum.Stock:
                    break;
            }
            return bm;
        }

        private void DrawLegend(Rdl.Runtime.Context context, System.Drawing.Graphics g, Bitmap bm,
            ref int l, ref int t, ref int w, ref int h, bool includeGroups, decimal xMult, decimal yMult)
        {
            if (_legend == null)
                return;

            if (!_legend.Visible)
                return;

            // The legend is only required if there is a series.
            if (_seriesGrouping == null)
                return;

            // Build the color values to use for each group
            int index = 0;
            foreach (Series ser in _leafSeriesList)
            {
                if (!_seriesColors.ContainsKey(ser.CombinedValue))
                {
                    _seriesColors.Add(ser.CombinedValue, Rdl.Engine.Chart.Palette.GetColor(_palette, index));
                    index++;
                }
            }

            // Get the font to use to draw the text.
            System.Drawing.Font font = _legend.Style.GetWindowsFont(context);

            // Measure the space required for the text.
            int height = 0;
            int width = 0;
            int lineHeight = 0;
            foreach (string s in _seriesColors.Keys)
            {
                SizeF size = g.MeasureString(s, font);
                lineHeight = (int)size.Height + 1;
                height += lineHeight;
                width = Math.Max(width, (int)size.Width + 1);
            }

            // Add some room for the color boxes and space between columns.
            width += lineHeight * 2;

            int columns = 1;

            int top = t, left = l;

            if (_legend.Position == Legend.PositionEnum.LeftBottom ||
                _legend.Position == Legend.PositionEnum.LeftCenter ||
                _legend.Position == Legend.PositionEnum.LeftTop ||
                _legend.Position == Legend.PositionEnum.RightTop ||
                _legend.Position == Legend.PositionEnum.RightCenter ||
                _legend.Position == Legend.PositionEnum.RightBottom)
            {
                // If the legend is going on the left or right of the chart then don't use more than
                // 1/3 of the available area for the legend.
                width = Math.Min(width, w / 3);
                w -= width + _legendPadding;
                height = Math.Min(height, h - (_legendPadding << 1));
                if (_legend.Position == Legend.PositionEnum.LeftBottom ||
                    _legend.Position == Legend.PositionEnum.LeftCenter ||
                    _legend.Position == Legend.PositionEnum.LeftTop)
                    l += width + _legendPadding;
                else
                    left = w + _legendPadding;

                if (_legend.Position == Legend.PositionEnum.LeftBottom ||
                    _legend.Position == Legend.PositionEnum.RightBottom )
                    top = t + h - height;
                if (_legend.Position == Legend.PositionEnum.LeftCenter ||
                    _legend.Position == Legend.PositionEnum.RightCenter)
                    top = t + (h/2) - (height/2);
            }
            else
            {
                columns = (int)(w / width);
                height = lineHeight * (int)Math.Ceiling((double)_seriesColors.Count / columns);

                // If the legend is going above or below the chart then don't use more than 
                // %60 the available space.
                height = Math.Min(height, (int)((h - _legendPadding - _legendPadding) * .6));
                h -= height + _legendPadding + _legendPadding;
                if (_legend.Position == Legend.PositionEnum.TopCenter ||
                    _legend.Position == Legend.PositionEnum.TopLeft ||
                    _legend.Position == Legend.PositionEnum.TopRight)
                {
                    top = t + _legendPadding;
                    t += height + _legendPadding + _legendPadding;
                }
                else
                    top = t + h + _legendPadding;

                if (_legend.Position == Legend.PositionEnum.BottomRight ||
                    _legend.Position == Legend.PositionEnum.TopRight)
                    left = l + w - (width*columns);
                if (_legend.Position == Legend.PositionEnum.BottomCenter ||
                    _legend.Position == Legend.PositionEnum.TopCenter)
                    left = l + (w / 2) - ((width*columns) / 2);
            }

            if (lineHeight * _seriesColors.Count > height)
                lineHeight = height / _seriesColors.Count;

            if (_legend.Style.BorderWidth != null)
                Rdl.Render.Drawing.DrawBorder(g, new System.Drawing.Rectangle(left, top, (width*columns), height),
                    new Rdl.Render.BorderWidth(_legend.Style.BorderWidth, context),
                    new Rdl.Render.BorderStyle(_legend.Style.BorderStyle, context),
                    new Rdl.Render.BorderColor(_legend.Style.BorderColor, context),
                    xMult, yMult);


            // Draw the text onto the chart legend area.
            System.Drawing.Brush brush = new SolidBrush(Style.W32Color(Style.Color(context)));
            float x, y;
            x = left;
            y = top;
            int c = 1;
            foreach (string s in _seriesColors.Keys)
            {
                if (y + lineHeight <= top + height)
                {
                    // Draw the collored box
                    g.FillRectangle(new SolidBrush(_seriesColors[s]),
                        new RectangleF(x, y + (lineHeight / 10), (lineHeight * .8f), (lineHeight * .8f)));
                    g.DrawString(s, font, brush, new RectangleF(x + lineHeight, y, width, height - (y - top)), new StringFormat(StringFormatFlags.NoWrap));
                    if (c == columns)
                    {
                        c = 1;
                        y += lineHeight;
                        x = left;
                    }
                    else
                    {
                        x += width;
                        c++;
                    }
                }
            }
        }

        public void FindMinMaxValues(Rdl.Runtime.Context context, ref decimal min, ref decimal max, bool ignoreSeries)
        {
            min = 0;
            max = 0;
            // Loop through the categories and series looking for min and max data values.
            if (_leafCategories != null)
            {
                foreach (Category cat in _leafCategories)
                {
                    if (SeriesGrouping != null && !ignoreSeries)
                    {
                        foreach (Series ser in SeriesGrouping.GetSeriesList(cat.Context, null).LeafSeriesList)
                            GetMinMax(ser.Context, ser.SeriesIndex, cat.CategoryIndex, ref min, ref max);
                    }
                    else
                        GetMinMax(cat.Context, 0, cat.CategoryIndex, ref min, ref max);
                }
            }
            else
            {
                if (SeriesGrouping != null && !ignoreSeries)
                {
                    foreach (Series ser in SeriesGrouping.GetSeriesList(context, null).LeafSeriesList)
                        GetMinMax(ser.Context, ser.SeriesIndex, 0, ref min, ref max);
                }
                else
                    GetMinMax(context, 0, 0, ref min, ref max);
            }
        }

        private void GetMinMax(Rdl.Runtime.Context context, int seriesIndex, int categoryIndex, ref decimal min, ref decimal max)
        {
            decimal[] values = GetDataValues(context, seriesIndex, categoryIndex);
            foreach (decimal value in values)
            {
                min = Math.Min(min, value);
                max = Math.Max(max, value);
            }
        }

        public decimal GetDataValue(Rdl.Runtime.Context context, int seriesIndex, int catagoryIndex)
        {
            return _chartData[seriesIndex].DataPoints[catagoryIndex].DataValues[0].ExecAsDecimal(context);
        }

        public decimal[] GetDataValues(Rdl.Runtime.Context context, int seriesIndex, int catagoryIndex)
        {
            decimal[] values = new decimal[_chartData[seriesIndex].DataPoints[catagoryIndex].DataValues.Count];

            for( int i=0; i < values.Length; i++)
                values[i] = _chartData[seriesIndex].DataPoints[catagoryIndex].DataValues[i].ExecAsDecimal(context);

            return values;
        }
     }
}
