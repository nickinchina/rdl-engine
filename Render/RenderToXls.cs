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
using System.Text;
using System.IO;
using CSharpJExcel.Jxl;
using CSharpJExcel.Jxl.Write;
using CSharpJExcel.Jxl.Format;


namespace Rdl.Render
{
    public class RenderToXls
    {
        private List<decimal> _rows = new List<decimal>();
        private List<decimal> _cols = new List<decimal>();
        //private const int _lineHeight = 25;  // The point multiplier for line heights
        //private const int _fontHeight = 20; // The point multiplier for font heights
        //private const int _colWidth = 45; // The point multiplier for column widths
        private const int _lineHeight = 20;  // The point multiplier for line heights
        private const int _fontHeight = 1; // The point multiplier for font heights
        private const decimal _colWidth = 0.22M; // The point multiplier for column widths
        private WritableWorkbook _workbook = null;
        private WritableSheet _ws = null;
        private WritableCellFormat[] formats;
        Rdl.Render.GenericRender _report;

        public byte[] Render(Rdl.Render.GenericRender report, bool renderAll)
        {
            _report = report;
            MemoryStream ms = new MemoryStream();
            report.SetSizes(renderAll);

            _workbook = Workbook.createWorkbook(ms);
            //_workbook = Workbook.createWorkbook(new System.IO.FileInfo(@"c:\foo.xls"));
            _ws = _workbook.createSheet("Sheet 1", 0);

            RecurseBuildRowsCols(report.BodyContainer, 0, 0, renderAll);
            _rows.Add(0);
            _rows.Sort(delegate(decimal d1, decimal d2) { return decimal.Compare(d1, d2); });
            _cols.Add(0);
            _cols.Sort(delegate(decimal d1, decimal d2) { return decimal.Compare(d1, d2); });

            for (int i = 1; i < _rows.Count; i++)
                _ws.setRowView(i-1, (int)((_rows[i] - _rows[i-1]) * _lineHeight));

            for (int i = 1; i < _cols.Count; i++)
                _ws.setColumnView(i-1, (int)((_cols[i] - _cols[i-1]) * _colWidth));

            formats = new WritableCellFormat[report.StyleList.Count];
            for (int i = 0; i < report.StyleList.Count; i++)
                if (report.StyleList[i] is TextStyle)
                    formats[i] = GetWritableFormat((TextStyle)report.StyleList[i]);

            RecurseRender(report.BodyContainer, 0, 0, renderAll);

            _workbook.write();
            _workbook.close();
            //BIFF8Writer.WriteWorkbookToStream(_workbook, ms);
            return ms.ToArray();
        }

        private void RecurseBuildRowsCols(Element elmt, decimal top, decimal left, bool renderAll)
        {
            top += elmt.Top;
            left += elmt.Left;

            if (!renderAll)
                if (elmt is Container && !((Container)elmt).IsVisible)
                    return;

            //if (_rows.Find(delegate(decimal d) { return d == top; }) == decimal.Zero)
            //    _rows.Add(top);
            if (elmt is TextElement || elmt is ImageElement)
            {
                if (top != 0 && _rows.Find(delegate(decimal d) { return d == top; }) == decimal.Zero)
                    _rows.Add(top);

                if (left != 0 && _cols.Find(delegate(decimal d) { return d == left; }) == decimal.Zero)
                    _cols.Add(left);
                if (left + elmt.Width != 0 && _cols.Find(delegate(decimal d) { return d == left + elmt.Width; }) == decimal.Zero)
                    _cols.Add(left + elmt.Width);
            }

            if (elmt is Container)
                foreach (Element child in ((Container)elmt).Children)
                    RecurseBuildRowsCols(child, top, left, renderAll);
        }


        private void RecurseRender(Element elmt, decimal top, decimal left, bool renderAll)
        {
            top += elmt.Top;
            left += elmt.Left;

            if (!renderAll)
                if (elmt is Container && !((Container)elmt).IsVisible)
                    return;

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

                WritableCell cell;
                double dValue;
                if (double.TryParse(te.Text, out dValue))
                    cell = new Number(col, row, dValue);
                else
                    cell = new Label(col, row, te.Text);
                cell.setCellFormat(formats[te.StyleIndex]);
                _ws.addCell(cell);
            }

            if (elmt is ImageElement)
            {
                ImageElement ie = elmt as ImageElement;

                Int32 row = _rows.FindIndex(delegate(decimal d) { return d == top; });
                Int32 col;
                if (ie != null)
                    col = _cols.FindIndex(delegate(decimal d) { return d == left; });

                Rdl.Render.ImageData id = _report.ImageList[ie._imageIndex];
                WritableImage img = new WritableImage(0, 0, (double)ie.Width, (double)ie.Height,
                    id.GetImageData());
                _ws.addImage(img);
            }

            if (elmt is Container)
                foreach (Element child in ((Container)elmt).Children)
                    RecurseRender(child, top, left, renderAll);
        }

        private Alignment ExcelAlignmentFromRdlAlignment(Rdl.Engine.Style.TextAlignEnum align)
        {
            switch( align )
            {
                case Engine.Style.TextAlignEnum.Center:
                    return Alignment.CENTRE;
                case Engine.Style.TextAlignEnum.General:
                    return Alignment.GENERAL;
                case Engine.Style.TextAlignEnum.Left:
                    return Alignment.LEFT;
                case Engine.Style.TextAlignEnum.Right:
                    return Alignment.RIGHT;
            }
            return Alignment.GENERAL;
        }

        private BorderLineStyle ExcelBorderStyleFromRdlBorderStyle(Rdl.Engine.BorderStyle.BorderStyleEnum bs)
        {
            switch (bs)
            {
                case Rdl.Engine.BorderStyle.BorderStyleEnum.Dashed:
                    return BorderLineStyle.DASHED;
                case Rdl.Engine.BorderStyle.BorderStyleEnum.Dotted:
                    return BorderLineStyle.DOTTED;
                case Rdl.Engine.BorderStyle.BorderStyleEnum.Double:
                    return BorderLineStyle.DOUBLE;
                case Rdl.Engine.BorderStyle.BorderStyleEnum.Groove:
                    return BorderLineStyle.DOUBLE;
                case Rdl.Engine.BorderStyle.BorderStyleEnum.Inset:
                    return BorderLineStyle.THIN;
                case Rdl.Engine.BorderStyle.BorderStyleEnum.None:
                    return BorderLineStyle.NONE;
                case Rdl.Engine.BorderStyle.BorderStyleEnum.Outset:
                    return BorderLineStyle.THICK;
                case Rdl.Engine.BorderStyle.BorderStyleEnum.Ridge:
                    return BorderLineStyle.DOUBLE;
                case Rdl.Engine.BorderStyle.BorderStyleEnum.Solid:
                    return BorderLineStyle.THIN;
                case Rdl.Engine.BorderStyle.BorderStyleEnum.WindowInset:
                    return BorderLineStyle.THIN;
            }
            return BorderLineStyle.NONE;
        }

        private WritableFont.BoldStyle BoldStyle(TextStyle ts)
        {
            switch(ts.FontWeight)
            {
                case Engine.Style.FontWeightEnum._100:
                    return new WritableFont.BoldStyle(100);
                case Engine.Style.FontWeightEnum._200:
                    return new WritableFont.BoldStyle(200);
                case Engine.Style.FontWeightEnum._300:
                    return new WritableFont.BoldStyle(300);
                case Engine.Style.FontWeightEnum._400:
                    return new WritableFont.BoldStyle(400);
                case Engine.Style.FontWeightEnum._500:
                    return new WritableFont.BoldStyle(500);
                case Engine.Style.FontWeightEnum._600:
                    return new WritableFont.BoldStyle(600);
                case Engine.Style.FontWeightEnum._700:
                    return new WritableFont.BoldStyle(700);
                case Engine.Style.FontWeightEnum._800:
                    return new WritableFont.BoldStyle(800);
                case Engine.Style.FontWeightEnum._900:
                    return new WritableFont.BoldStyle(900);
                case Engine.Style.FontWeightEnum.Bold:
                    return new WritableFont.BoldStyle(800);
                case Engine.Style.FontWeightEnum.Bolder:
                    return new WritableFont.BoldStyle(1000);
                case Engine.Style.FontWeightEnum.Lighter:
                    return new WritableFont.BoldStyle(200);
                case Engine.Style.FontWeightEnum.Normal:
                    return new WritableFont.BoldStyle(300);
            }
            return new WritableFont.BoldStyle(300);
        }

        Colour getXlsColor(string color, Colour defaultColor)
        {
            if (color == null)
                return defaultColor;
            foreach (Colour c in Colour.getAllColours())
            {
                if (c.getDescription().ToLower() == color.ToLower())
                    return c;
            }
            return defaultColor;
        }

        private WritableCellFormat GetWritableFormat(TextStyle ts)
        {
            WritableCellFormat cellFormat = new WritableCellFormat();

            WritableFont font = new WritableFont(
                new WritableFont.FontName(ts.FontFamily),
                (int)ts.FontSize.points * _fontHeight,
                BoldStyle(ts),
                (ts.FontStyle == Engine.Style.FontStyleEnum.Italic),
                (ts.TextDecoration == Engine.Style.TextDecorationEnum.Underline) ? UnderlineStyle.SINGLE : UnderlineStyle.NO_UNDERLINE,
                (ts.Color == null) ? Colour.BLACK : getXlsColor(ts.Color, Colour.BLACK),
                ScriptStyle.NORMAL_SCRIPT);
            cellFormat.setFont(font);
            cellFormat.setAlignment(ExcelAlignmentFromRdlAlignment(ts.TextAlign));


            if (ts.BorderStyle != null && ts.BorderStyle != null)
            {
                BorderStyle bs = ts.BorderStyle;
                BorderColor bc = ts.BorderColor;

                cellFormat.setBorder(Border.TOP, ExcelBorderStyleFromRdlBorderStyle(bs.Top),
                    (bc == null) ?  Colour.BLACK : getXlsColor(ts.BorderColor.Top, Colour.BLACK));
                cellFormat.setBorder(Border.BOTTOM, ExcelBorderStyleFromRdlBorderStyle(bs.Bottom),
                    (bc == null) ? Colour.BLACK : getXlsColor(ts.BorderColor.Bottom, Colour.BLACK));
                cellFormat.setBorder(Border.LEFT, ExcelBorderStyleFromRdlBorderStyle(bs.Left),
                    (bc == null) ? Colour.BLACK : getXlsColor(ts.BorderColor.Left, Colour.BLACK));
                cellFormat.setBorder(Border.RIGHT, ExcelBorderStyleFromRdlBorderStyle(bs.Right),
                    (bc == null) ? Colour.BLACK : getXlsColor(ts.BorderColor.Right, Colour.BLACK));
            }
            if (ts.BackgroundColor != null)
                cellFormat.setBackground(getXlsColor(ts.BackgroundColor, Colour.WHITE), Pattern.SOLID);
            return cellFormat;
        }
    }
}
