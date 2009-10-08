using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Infragistics.Excel;


namespace RdlRender
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

        public byte[] Render(Element report)
        {
            _workbook = new Workbook();
            _ws = _workbook.Worksheets.Add("Report");

            RecurseBuildRowsCols(report, 0, 0);
            _rows.Add(0);
            _rows.Sort(delegate(decimal d1, decimal d2) { return decimal.Compare(d1, d2); });
            _cols.Add(0);
            _cols.Sort(delegate(decimal d1, decimal d2) { return decimal.Compare(d1, d2); });

            for (int i = 1; i < _rows.Count; i++)
                _ws.Rows[i-1].Height = (int)((_rows[i] - _rows[i-1]) * _lineHeight);

            for (int i = 1; i < _cols.Count; i++)
                _ws.Columns[i-1].Width = (int)((_cols[i] - _cols[i-1]) * _colWidth);

            RecurseRender(report, 0, 0);

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
                if (ts != null && ts.TextAlign == RdlEngine.Style.TextAlignEnum.Right)
                    col = _cols.FindIndex(delegate(decimal d) { return d == left + elmt.Width; }) - 1;
                else
                    col = _cols.FindIndex(delegate(decimal d) { return d == left; });

                WorksheetCell cell = _ws.Rows[row].Cells[col];
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
                        cell.CellFormat.TopBorderColor = System.Drawing.Color.FromName(ts.BorderColor.Top);
                        cell.CellFormat.BottomBorderColor = System.Drawing.Color.FromName(ts.BorderColor.Bottom);
                        cell.CellFormat.LeftBorderColor = System.Drawing.Color.FromName(ts.BorderColor.Left);
                        cell.CellFormat.RightBorderColor = System.Drawing.Color.FromName(ts.BorderColor.Right);
                    }
                    cell.CellFormat.Font.Name = ts.FontFamily;
                    cell.CellFormat.Font.Height = fontHeight;
                    RdlEngine.Style.FontStyleEnum style = ts.FontStyle;
                    if (style == RdlEngine.Style.FontStyleEnum.Italic)
                        cell.CellFormat.Font.Italic = ExcelDefaultableBoolean.True;
                    switch (ts.FontWeight)
                    {
                        case RdlEngine.Style.FontWeightEnum.Bold:
                        case RdlEngine.Style.FontWeightEnum.Bolder:
                        case RdlEngine.Style.FontWeightEnum._600:
                        case RdlEngine.Style.FontWeightEnum._700:
                        case RdlEngine.Style.FontWeightEnum._800:
                        case RdlEngine.Style.FontWeightEnum._900:
                            cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                            break;
                    }
                    cell.CellFormat.Font.Color = System.Drawing.Color.FromName(ts.Color);
                    cell.CellFormat.Alignment = (HorizontalCellAlignment)Enum.Parse(typeof(HorizontalCellAlignment), ts.TextAlign.ToString(), true);
                    if (ts.BackgroundColor != null)
                    {
                        cell.CellFormat.FillPatternForegroundColor = System.Drawing.Color.FromName(ts.BackgroundColor);
                        cell.CellFormat.FillPatternBackgroundColor = System.Drawing.Color.White;
                        cell.CellFormat.FillPattern = FillPatternStyle.Solid;
                    }
                }
            }

            if (elmt is Container)
                foreach (Element child in ((Container)elmt).Children)
                    RecurseRender(child, top, left);
        }

        private CellBorderLineStyle ExcelBorderStyleFromRdlBorderStyle(RdlEngine.BorderStyle.BorderStyleEnum bs)
        {
            switch (bs)
            {
                case RdlEngine.BorderStyle.BorderStyleEnum.Dashed:
                    return CellBorderLineStyle.Dashed;
                case RdlEngine.BorderStyle.BorderStyleEnum.Dotted:
                    return CellBorderLineStyle.Dotted;
                case RdlEngine.BorderStyle.BorderStyleEnum.Double:
                    return CellBorderLineStyle.Double;
                case RdlEngine.BorderStyle.BorderStyleEnum.Groove:
                    return CellBorderLineStyle.DashDotDot;
                case RdlEngine.BorderStyle.BorderStyleEnum.Inset:
                    return CellBorderLineStyle.Thin;
                case RdlEngine.BorderStyle.BorderStyleEnum.None:
                    return CellBorderLineStyle.None;
                case RdlEngine.BorderStyle.BorderStyleEnum.Outset:
                    return CellBorderLineStyle.Thick;
                case RdlEngine.BorderStyle.BorderStyleEnum.Ridge:
                    return CellBorderLineStyle.Double;
                case RdlEngine.BorderStyle.BorderStyleEnum.Solid:
                    return CellBorderLineStyle.Thin;
                case RdlEngine.BorderStyle.BorderStyleEnum.WindowInset:
                    return CellBorderLineStyle.Thin;
            }
            return CellBorderLineStyle.None;
        }
    }
}
