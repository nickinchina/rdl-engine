using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Infragistics.Excel;


namespace Rdl.Render
{
    public class RenderToXls
    {
        private List<decimal> _rows = new List<decimal>();
        private List<decimal> _cols = new List<decimal>();
        private const int _lineHeight = 25;  // The point multiplier for line heights
        private const int _fontHeight = 20; // The point multiplier for font heights
        private const int _colWidth = 45; // The point multiplier for column widths
        private Workbook _workbook = null;
        private Worksheet _ws = null;

        public byte[] Render(Rdl.Engine.Report report)
        {
            report.SetSizes(true);

            _workbook = new Workbook();
            _ws = _workbook.Worksheets.Add("Report");

            RecurseBuildRowsCols(report.BodyContainer, 0, 0);
            _rows.Add(0);
            _rows.Sort(delegate(decimal d1, decimal d2) { return decimal.Compare(d1, d2); });
            _cols.Add(0);
            _cols.Sort(delegate(decimal d1, decimal d2) { return decimal.Compare(d1, d2); });

            for (int i = 1; i < _rows.Count; i++)
                _ws.Rows[i-1].Height = (int)((_rows[i] - _rows[i-1]) * _lineHeight);

            for (int i = 1; i < _cols.Count; i++)
                _ws.Columns[i-1].Width = (int)((_cols[i] - _cols[i-1]) * _colWidth);

            RecurseRender(report.BodyContainer, 0, 0);

            MemoryStream ms = new MemoryStream();
            BIFF8Writer.WriteWorkbookToStream(_workbook, ms);
            return ms.ToArray();
        }

        private void RecurseBuildRowsCols(Element elmt, decimal top, decimal left)
        {
            top += elmt.Top;
            left += elmt.Left;

            //if (_rows.Find(delegate(decimal d) { return d == top; }) == decimal.Zero)
            //    _rows.Add(top);
            if (top != 0 && _rows.Find(delegate(decimal d) { return d == top; }) == decimal.Zero)
                _rows.Add(top);

            if (left != 0 && _cols.Find(delegate(decimal d) { return d == left; }) == decimal.Zero)
                _cols.Add(left);
            if (left + elmt.Width != 0 && _cols.Find(delegate(decimal d) { return d == left + elmt.Width ; }) == decimal.Zero)
                _cols.Add(left + elmt.Width);

            if (elmt is Container)
                foreach (Element child in ((Container)elmt).Children)
                    RecurseBuildRowsCols(child, top, left);
        }


        private void RecurseRender(Element elmt, decimal top, decimal left)
        {
            top += elmt.Top;
            left += elmt.Left;

            if (elmt is TextElement)
            {
                TextElement te = elmt as TextElement;
                TextStyle ts = null;
                if (te.Style is TextStyle)
                    ts = (TextStyle)te.Style;

                Int32 row = _rows.FindIndex(delegate(decimal d) { return d == top; });
                Int32 col;
                if (ts != null && ts.TextAlign == Rdl.Engine.Style.TextAlignEnum.Right)
                    col = _cols.FindIndex(delegate(decimal d) { return d == left + elmt.Width; }) - 1;
                else
                    col = _cols.FindIndex(delegate(decimal d) { return d == left; });

                WorksheetCell cell = _ws.Rows[row].Cells[col];
                decimal dValue;
                if (decimal.TryParse(te.Text, out dValue))
                    cell.Value = dValue;
                else
                    cell.Value = te.Text;

                if (ts != null)
                {
                    int fontHeight = (int)(ts.FontSize.points * _fontHeight);

                    if (ts.BorderStyle != null && ts.BorderStyle != null)
                    {
                        BorderStyle bs = ts.BorderStyle;

                        cell.CellFormat.TopBorderStyle = ExcelBorderStyleFromRdlBorderStyle(bs.Top);
                        cell.CellFormat.BottomBorderStyle = ExcelBorderStyleFromRdlBorderStyle(bs.Bottom);
                        cell.CellFormat.LeftBorderStyle = ExcelBorderStyleFromRdlBorderStyle(bs.Left);
                        cell.CellFormat.RightBorderStyle = ExcelBorderStyleFromRdlBorderStyle(bs.Right);
                    }
                    if (ts.BorderColor != null)
                    {
                        cell.CellFormat.TopBorderColor = Rdl.Engine.Style.W32Color(ts.BorderColor.Top);
                        cell.CellFormat.BottomBorderColor = Rdl.Engine.Style.W32Color(ts.BorderColor.Bottom);
                        cell.CellFormat.LeftBorderColor = Rdl.Engine.Style.W32Color(ts.BorderColor.Left);
                        cell.CellFormat.RightBorderColor = Rdl.Engine.Style.W32Color(ts.BorderColor.Right);
                    }
                    cell.CellFormat.Font.Name = ts.FontFamily;
                    cell.CellFormat.Font.Height = fontHeight;
                    Rdl.Engine.Style.FontStyleEnum style = ts.FontStyle;
                    if (style == Rdl.Engine.Style.FontStyleEnum.Italic)
                        cell.CellFormat.Font.Italic = ExcelDefaultableBoolean.True;
                    switch (ts.FontWeight)
                    {
                        case Rdl.Engine.Style.FontWeightEnum.Bold:
                        case Rdl.Engine.Style.FontWeightEnum.Bolder:
                        case Rdl.Engine.Style.FontWeightEnum._600:
                        case Rdl.Engine.Style.FontWeightEnum._700:
                        case Rdl.Engine.Style.FontWeightEnum._800:
                        case Rdl.Engine.Style.FontWeightEnum._900:
                            cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                            break;
                    }
                    cell.CellFormat.Font.Color = Rdl.Engine.Style.W32Color(ts.Color);
                    cell.CellFormat.Alignment = (HorizontalCellAlignment)Enum.Parse(typeof(HorizontalCellAlignment), ts.TextAlign.ToString(), true);
                    if (ts.BackgroundColor != null)
                    {
                        cell.CellFormat.FillPatternForegroundColor = Rdl.Engine.Style.W32Color(ts.BackgroundColor);
                        cell.CellFormat.FillPatternBackgroundColor = System.Drawing.Color.White;
                        cell.CellFormat.FillPattern = FillPatternStyle.Solid;
                    }
                }
            }

            if (elmt is Container)
                foreach (Element child in ((Container)elmt).Children)
                    RecurseRender(child, top, left);
        }

        private CellBorderLineStyle ExcelBorderStyleFromRdlBorderStyle(Rdl.Engine.BorderStyle.BorderStyleEnum bs)
        {
            switch (bs)
            {
                case Rdl.Engine.BorderStyle.BorderStyleEnum.Dashed:
                    return CellBorderLineStyle.Dashed;
                case Rdl.Engine.BorderStyle.BorderStyleEnum.Dotted:
                    return CellBorderLineStyle.Dotted;
                case Rdl.Engine.BorderStyle.BorderStyleEnum.Double:
                    return CellBorderLineStyle.Double;
                case Rdl.Engine.BorderStyle.BorderStyleEnum.Groove:
                    return CellBorderLineStyle.DashDotDot;
                case Rdl.Engine.BorderStyle.BorderStyleEnum.Inset:
                    return CellBorderLineStyle.Thin;
                case Rdl.Engine.BorderStyle.BorderStyleEnum.None:
                    return CellBorderLineStyle.None;
                case Rdl.Engine.BorderStyle.BorderStyleEnum.Outset:
                    return CellBorderLineStyle.Thick;
                case Rdl.Engine.BorderStyle.BorderStyleEnum.Ridge:
                    return CellBorderLineStyle.Double;
                case Rdl.Engine.BorderStyle.BorderStyleEnum.Solid:
                    return CellBorderLineStyle.Thin;
                case Rdl.Engine.BorderStyle.BorderStyleEnum.WindowInset:
                    return CellBorderLineStyle.Thin;
            }
            return CellBorderLineStyle.None;
        }
    }
}
