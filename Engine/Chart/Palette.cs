using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Engine.Chart
{
    class Palette
    {
        public enum PaletteEnum
        {
            Default,
            EarthTones,
            Excel,
            GrayScale,
            Light,
            Pastel,
            SemiTransparent
        };

        private static System.Drawing.Color[] _excelColors = 
            { System.Drawing.Color.FromArgb(0xFF, 0, 0, 0),
                System.Drawing.Color.FromArgb(0xFF, 255, 255, 255),
                System.Drawing.Color.FromArgb(0xFF, 255, 0, 0),
                System.Drawing.Color.FromArgb(0xFF, 0, 255, 0),
                System.Drawing.Color.FromArgb(0xFF, 0, 0, 255),
                System.Drawing.Color.FromArgb(0xFF, 255, 255, 0),
                System.Drawing.Color.FromArgb(0xFF, 255, 0, 255),
                System.Drawing.Color.FromArgb(0xFF, 0, 255, 255),
                System.Drawing.Color.FromArgb(0xFF, 128, 0, 0),
                System.Drawing.Color.FromArgb(0xFF, 0, 128, 0),
                System.Drawing.Color.FromArgb(0xFF, 0, 0, 128),
                System.Drawing.Color.FromArgb(0xFF, 128, 128, 0),
                System.Drawing.Color.FromArgb(0xFF, 128, 0, 128),
                System.Drawing.Color.FromArgb(0xFF, 0, 128, 128),
                System.Drawing.Color.FromArgb(0xFF, 192, 192, 192),
                System.Drawing.Color.FromArgb(0xFF, 128, 128, 128),
                System.Drawing.Color.FromArgb(0xFF, 153, 153, 255),
                System.Drawing.Color.FromArgb(0xFF, 153, 51, 102),
                System.Drawing.Color.FromArgb(0xFF, 255, 255, 204),
                System.Drawing.Color.FromArgb(0xFF, 204, 255, 255),
                System.Drawing.Color.FromArgb(0xFF, 102, 0, 102),
                System.Drawing.Color.FromArgb(0xFF, 255, 128, 128),
                System.Drawing.Color.FromArgb(0xFF, 0, 102, 204),
                System.Drawing.Color.FromArgb(0xFF, 204, 204, 255),
                System.Drawing.Color.FromArgb(0xFF, 0, 0, 128),
                System.Drawing.Color.FromArgb(0xFF, 255, 0, 255),
                System.Drawing.Color.FromArgb(0xFF, 255, 255, 0),
                System.Drawing.Color.FromArgb(0xFF, 0, 255, 255),
                System.Drawing.Color.FromArgb(0xFF, 128, 0, 128),
                System.Drawing.Color.FromArgb(0xFF, 128, 0, 0),
                System.Drawing.Color.FromArgb(0xFF, 0, 128, 128),
                System.Drawing.Color.FromArgb(0xFF, 0, 0, 255),
                System.Drawing.Color.FromArgb(0xFF, 0, 204, 255),
                System.Drawing.Color.FromArgb(0xFF, 204, 255, 255),
                System.Drawing.Color.FromArgb(0xFF, 204, 255, 204),
                System.Drawing.Color.FromArgb(0xFF, 255, 255, 153),
                System.Drawing.Color.FromArgb(0xFF, 153, 204, 255),
                System.Drawing.Color.FromArgb(0xFF, 255, 153, 204),
                System.Drawing.Color.FromArgb(0xFF, 204, 153, 255),
                System.Drawing.Color.FromArgb(0xFF, 255, 204, 153),
                System.Drawing.Color.FromArgb(0xFF, 51, 102, 255),
                System.Drawing.Color.FromArgb(0xFF, 51, 204, 204),
                System.Drawing.Color.FromArgb(0xFF, 153, 204, 0),
                System.Drawing.Color.FromArgb(0xFF, 255, 204, 0),
                System.Drawing.Color.FromArgb(0xFF, 255, 153, 0),
                System.Drawing.Color.FromArgb(0xFF, 255, 102, 0),
                System.Drawing.Color.FromArgb(0xFF, 102, 102, 153),
                System.Drawing.Color.FromArgb(0xFF, 150, 150, 150),
                System.Drawing.Color.FromArgb(0xFF, 0, 51, 102),
                System.Drawing.Color.FromArgb(0xFF, 51, 153, 102),
                System.Drawing.Color.FromArgb(0xFF, 0, 51, 0),
                System.Drawing.Color.FromArgb(0xFF, 51, 51, 0),
                System.Drawing.Color.FromArgb(0xFF, 153, 51, 0),
                System.Drawing.Color.FromArgb(0xFF, 153, 51, 102),
                System.Drawing.Color.FromArgb(0xFF, 51, 51, 153),
                System.Drawing.Color.FromArgb(0xFF, 51, 51, 51) 
            };
        private static int[] _excelChartLineColors = { 24, 25, 26, 27, 28, 29, 30, 31 };

        private static System.Drawing.Color[] _earthToneColors = 
            {
				System.Drawing.Color.Brown,
				System.Drawing.Color.Chocolate,
				System.Drawing.Color.IndianRed,
				System.Drawing.Color.Peru,
				System.Drawing.Color.BurlyWood,
				System.Drawing.Color.AntiqueWhite,
				System.Drawing.Color.FloralWhite,
				System.Drawing.Color.Ivory,
				System.Drawing.Color.LightCoral,
				System.Drawing.Color.DarkSalmon,
				System.Drawing.Color.LightSalmon,
				System.Drawing.Color.PeachPuff,
				System.Drawing.Color.NavajoWhite,
				System.Drawing.Color.Moccasin,
				System.Drawing.Color.PapayaWhip,
				System.Drawing.Color.Goldenrod,
				System.Drawing.Color.DarkGoldenrod,
				System.Drawing.Color.DarkKhaki,
				System.Drawing.Color.Khaki,
				System.Drawing.Color.Beige,
				System.Drawing.Color.Cornsilk
            };

        private static System.Drawing.Color[] _lightColors = 
            {
				System.Drawing.Color.LightBlue,
				System.Drawing.Color.LightCoral,
				System.Drawing.Color.LightCyan,
				System.Drawing.Color.LightGoldenrodYellow,
				System.Drawing.Color.LightGray,
				System.Drawing.Color.LightGreen,
				System.Drawing.Color.LightPink,
				System.Drawing.Color.LightSalmon,
				System.Drawing.Color.LightSeaGreen,
				System.Drawing.Color.LightSkyBlue,
				System.Drawing.Color.LightSlateGray,
				System.Drawing.Color.LightSteelBlue,
				System.Drawing.Color.LightYellow
            };

        private static System.Drawing.Color[] _pastelColors =
            {
				System.Drawing.Color.CadetBlue,
				System.Drawing.Color.MediumTurquoise,
				System.Drawing.Color.Aquamarine,
				System.Drawing.Color.LightCyan,
				System.Drawing.Color.Azure,
				System.Drawing.Color.AliceBlue,
				System.Drawing.Color.MintCream,
				System.Drawing.Color.DarkSeaGreen,
				System.Drawing.Color.PaleGreen,
				System.Drawing.Color.LightGreen,
				System.Drawing.Color.MediumPurple,
				System.Drawing.Color.CornflowerBlue,
				System.Drawing.Color.Lavender,
				System.Drawing.Color.GhostWhite,
				System.Drawing.Color.PaleGoldenrod,
				System.Drawing.Color.LightGoldenrodYellow,
				System.Drawing.Color.LemonChiffon,
				System.Drawing.Color.LightYellow,
				System.Drawing.Color.Orchid,
				System.Drawing.Color.Plum,
				System.Drawing.Color.LightPink,
				System.Drawing.Color.Pink,
				System.Drawing.Color.LavenderBlush,
				System.Drawing.Color.Linen,
				System.Drawing.Color.PaleTurquoise,
				System.Drawing.Color.OldLace
            };

        private static System.Drawing.Color[] _defaultColors = 
            {
                System.Drawing.Color.Blue,
                System.Drawing.Color.Red,
                System.Drawing.Color.Green,
				System.Drawing.Color.DarkBlue,
				System.Drawing.Color.Pink,
				System.Drawing.Color.Yellow,
				System.Drawing.Color.Turquoise,
				System.Drawing.Color.Violet,
				System.Drawing.Color.DarkRed,
				System.Drawing.Color.Teal,
				System.Drawing.Color.Blue,
				System.Drawing.Color.Plum,
				System.Drawing.Color.Ivory,
				System.Drawing.Color.Coral
            };

        public static System.Drawing.Color GetColor(PaletteEnum palette, int index)
        {
            System.Drawing.Color c = System.Drawing.Color.Black;
            switch (palette)
            {
                case PaletteEnum.EarthTones:
                    c = _earthToneColors[index % _earthToneColors.Length];
                    break;
                case PaletteEnum.Excel:
                    c = _excelColors[_excelChartLineColors[index % 8]];
                    break;
                case PaletteEnum.GrayScale:
                    c = System.Drawing.Color.FromArgb(0xFF, (index % 15) + 1, (index % 15) + 1, (index % 15) + 1);
                    break;
                case PaletteEnum.Light:
                    c = _lightColors[index % _lightColors.Length];
                    break;
                case PaletteEnum.Pastel:
                    c = _pastelColors[index % _pastelColors.Length];
                    break;
                case PaletteEnum.SemiTransparent:
                    c = _defaultColors[index % _defaultColors.Length];
                    c = System.Drawing.Color.FromArgb(0x7F, c.R, c.G, c.B);
                    break;
                default:
                    c = _defaultColors[index % _defaultColors.Length];
                    break;
            }
            return c;
        }
    }
}
