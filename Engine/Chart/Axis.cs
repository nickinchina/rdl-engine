using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine.Chart
{
    class Axis : ChartElement
    {
        public enum TickMarksEnum
        {
            None,
            Inside,
            Outside,
            Cross
        };

        private bool _visible = false;
        private Title _title = null;
        private bool _margin = false;
        private TickMarksEnum _majorTickMarks = TickMarksEnum.None;
        private TickMarksEnum _minorTickMarks = TickMarksEnum.None;
        private GridLines _majorGridLines = null;
        private GridLines _minorGridLines = null;
        private Expression _majorInterval = null;
        private Expression _minorInterval = null;
        protected decimal _majorIntervalValue = 0;
        protected decimal _minorIntervalValue = 0;
        private bool _reverse = false;
        private Expression _crossAt = null;
        private bool _interlaced = false;
        private bool _scalar = false;
        private Expression _min = null;
        private Expression _max = null;
        protected decimal _minValue = 0;
        protected decimal _maxValue = 0;
        private bool _logScale = false;
        private int _axisThickness = 0;

        public Axis(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            base.ParseAttribute(attr);
            switch (attr.Name.ToLower())
            {
                case "visible":
                    _visible = bool.Parse(attr.InnerText);
                    break;
                case "style":
                    _style = new Style(attr, this);
                    break;
                case "title":
                    _title = new Title(attr, this);
                    break;
                case "margin":
                    _margin = bool.Parse(attr.InnerText);
                    break;
                case "majortickmarks":
                    _majorTickMarks = (TickMarksEnum)Enum.Parse(typeof(TickMarksEnum), attr.InnerText,true);
                    break;
                case "minortickmarks":
                    _minorTickMarks = (TickMarksEnum)Enum.Parse(typeof(TickMarksEnum), attr.InnerText,true);
                    break;
                case "majorgridlines":
                    _majorGridLines = new GridLines(attr, this);
                    break;
                case "minorgridlines":
                    _minorGridLines = new GridLines(attr, this);
                    break;
                case "majorinterval":
                    _majorInterval = new Expression(attr, this);
                    break;
                case "minorinterval":
                    _minorInterval = new Expression(attr, this);
                    break;
                case "reverse":
                    _reverse = bool.Parse(attr.InnerText);
                    break;
                case "crossat":
                    _crossAt = new Expression(attr, this);
                    break;
                case "interlaced":
                    _interlaced = bool.Parse(attr.InnerText);
                    break;
                case "scalar":
                    _scalar = bool.Parse(attr.InnerText);
                    break;
                case "min":
                    _min = new Expression(attr, this);
                    break;
                case "max":
                    _max = new Expression(attr, this);
                    break;
                case "logscale":
                    _logScale = bool.Parse(attr.InnerText);
                    break;
                default:
                    break;
            }
        }

        public bool Visible
        {
            get { return _visible; }
        }

        public Title Title
        {
            get { return _title; }
        }

        public bool Margin
        {
            get { return _margin; }
        }

        public TickMarksEnum MajorTickMarks
        {
            get { return _majorTickMarks; }
        }

        public TickMarksEnum MinorTickMarks
        {
            get { return _minorTickMarks; }
        }

        public GridLines MajorGridLines
        {
            get { return _majorGridLines; }
        }

        public GridLines MinorGridLines
        {
            get { return _minorGridLines; }
        }

        public decimal MajorInterval
        {
            get{return _majorIntervalValue;}
        }

        public decimal MinorInterval
        {
            get{return _minorIntervalValue;}
        }

        public bool Interlaced
        {
            get { return _interlaced; }
        }

        public bool Scalar
        {
            get { return _scalar; }
        }

        public decimal Min
        {
            get {return _minValue;}
        }

        public decimal Max
        {
            get{return _maxValue;}
        }

        public bool LogScale
        {
            get { return _logScale; }
        }

        protected int GetAxisThickness(Rdl.Runtime.Context context)
        {
            if (_axisThickness > 0)
                return _axisThickness;
            if (MajorGridLines == null)
                _axisThickness = 1;
            else
            {
                _axisThickness = (int)MajorGridLines.Style.BorderWidth.Left(context).points;
                if (_axisThickness <= 0)
                    _axisThickness = 1;
            }
            return _axisThickness;
        }

        // Given the min and max values from scalar data, this routing adjusts the min and
        // max values and provides major and minor intervals.
        public void SetMinMaxValues(Rdl.Runtime.Context context,
            decimal minValue, decimal maxValue)
        {
            // By default the chart originates at 0 unless there is negative data.
            if (minValue > 0)
                minValue = 0;
            _minValue = minValue;
            _maxValue = maxValue;

            if (_min != null)
                _minValue = _min.ExecAsDecimal(context);
            if (_max != null)
                _maxValue = _max.ExecAsDecimal(context);

            if (_majorInterval != null)
                _majorIntervalValue = _majorInterval.ExecAsDecimal(context);
            if (_minorInterval != null)
                _minorIntervalValue = _minorInterval.ExecAsDecimal(context);

            // We are looking for between 5 and 10 major intervals on the value axis.
            // We try powers of 2, 5, and 10 to see which comes closest to the
            // max value while giving us an appropriate number of intervals.
            if (_majorIntervalValue == 0)
                if (!TryBase(10))
                    if (!TryBase(5))
                        TryBase(2);

            // If the major interval is specified and the minor interval is not specified
            // then default the minor interval to 10.
            if (_minorIntervalValue == 0)
                _minorIntervalValue = _majorIntervalValue / 10;
            
            // Adjust the min and max values to fall on an interval value.
            if (_majorIntervalValue != 0)
                _maxValue = _majorIntervalValue * ((int)(_maxValue / _majorIntervalValue) + 1);
            if (_minValue != 0 && _majorIntervalValue != 0)
                _minValue = _majorIntervalValue * ((int)(_minValue / _majorIntervalValue) - 1);
        }

        private bool TryBase(double newBase)
        {
            decimal min = _minValue;
            decimal max = _maxValue;
            int multiplier = 0;

            while (max - min > 100)
            {
                multiplier++;
                max /= 10;
                min /= 10;
            }
            while (max - min < 10)
            {
                multiplier--;
                max *= 10;
                min *= 10;
            }

            int power = (int)Math.Floor(Math.Log((double)Math.Abs(max - min), newBase));
            double dInterval = Math.Pow(newBase, power);
            if (dInterval == (double)Math.Abs(max - min))
                dInterval = Math.Pow(newBase, power - 1);
            if ((double)_maxValue / dInterval >= 5 && (double)max / dInterval <= 10)
            {
                _majorIntervalValue = (decimal)dInterval * (decimal)Math.Pow(10, multiplier);
                _minorIntervalValue = _majorIntervalValue / (decimal)newBase * (decimal)Math.Pow(10, multiplier);
                return true;
            }
            return false;
        }

        public void AdjustAxisPosition(int axisThickness, out int majorTickStart, out int majorTickSize, 
            out int minorTickStart, out int minorTickSize,
            out int axisPos, out int chartStart)
        {
            // By default major tick marks are 4 pixels and minor tick marks are 2 pixels.
            majorTickStart = 0;
            majorTickSize = 0;
            minorTickStart = 0;
            minorTickSize = 0;
            axisPos = 0;
            chartStart = axisThickness;

            switch (MajorTickMarks)
            {
                case TickMarksEnum.Inside:
                    majorTickStart = axisThickness;
                    majorTickSize = 4;
                    chartStart = 4 + axisThickness;
                    break;
                case TickMarksEnum.Outside:
                    majorTickStart = -4;
                    axisPos = 4;
                    majorTickSize = 4;
                    chartStart = 4 + axisThickness;
                    break;
                case TickMarksEnum.Cross:
                    axisPos = 2;
                    majorTickStart = -2;
                    majorTickSize = 4 + axisThickness;
                    chartStart = 4 + axisThickness;
                    break;
            }
            switch (MinorTickMarks)
            {
                case TickMarksEnum.Inside:
                    minorTickStart = axisThickness + axisPos;
                    minorTickSize = 2;
                    chartStart = Math.Max(chartStart, 2 + axisThickness);
                    break;
                case TickMarksEnum.Outside:
                    minorTickStart = -2;
                    axisPos = Math.Max(axisPos, 2);
                    minorTickStart = -2;
                    minorTickSize = 2;
                    chartStart = Math.Max(chartStart, 2 + axisThickness);
                    break;
                case TickMarksEnum.Cross:
                    axisPos = Math.Max(axisPos, 1);
                    minorTickStart = -1;
                    minorTickSize = axisThickness + 2;
                    chartStart = Math.Max(chartStart, 2 + axisThickness);
                    break;
            }
        }
    }
}
