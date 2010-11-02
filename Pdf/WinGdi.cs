using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Rdl.Pdf
{
    class WinGdi
    {
        [DllImport("user32")]
        public extern static IntPtr GetDC(IntPtr hWnd);
        [DllImport("user32")]
        public extern static IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("gdi32")]
        public extern static IntPtr SelectObject(IntPtr hDC, IntPtr hObj);
        [DllImport("gdi32")]
        public extern static int GetTextMetrics(IntPtr hDC, ref TEXTMETRICS tm);
        [DllImport("gdi32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern uint GetOutlineTextMetrics(IntPtr hdc, uint cbData, IntPtr ptrZero);
        [DllImport("gdi32")]
        public extern static bool GetCharWidth32(IntPtr hdc, uint iFirstChar, uint iLastChar, ref Int32 lpBuffer);
        [DllImport("gdi32.dll")]
        public static extern bool GetCharABCWidths(IntPtr hdc, uint uFirstChar, uint uLastChar, ref ABC lpabc);
        [DllImport("gdi32")]
        public static extern int GetTextFace(IntPtr hdc, int count, StringBuilder lpBuffer);

        [StructLayout(LayoutKind.Sequential)]
        public struct ABC
        { /* abc */
            public int abcA;
            public uint abcB;
            public int abcC;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TEXTMETRICS
        {
            public int Height;
            public int Ascent;
            public int Descent;
            public int InternalLeading;
            public int ExternalLeading;
            public int AveCharWidth;
            public int MaxCharWidth;
            public int Weight;
            public int Overhang;
            public int DigitizedAspectX;
            public int DigitizedAspectY;
            public byte FirstChar;
            public byte LastChar;
            public byte DefaultChar;
            public byte BreakChar;
            public byte Italic;
            public byte Underlined;
            public byte StruckOut;
            public byte PitchAndFamily;
            public byte CharSet;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct PANOSE 
        { 
            public byte bFamilyType; 
            public byte bSerifStyle; 
            public byte bWeight; 
            public byte bProportion; 
            public byte bContrast; 
            public byte bStrokeVariation; 
            public byte bArmStyle; 
            public byte bLetterform; 
            public byte bMidline; 
            public byte bXHeight; 
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            int left;
            int top;
            int right;
            int bottom;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            int x;
            int y;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public struct OUTLINETEXTMETRIC 
        { 
            public uint Size; 
            public TEXTMETRICS TextMetrics; 
            public byte Filler; 
            public PANOSE PanoseNumber; 
            public uint fsSelection; 
            public uint fsType; 
            public int sCharSlopeRise; 
            public int sCharSlopeRun; 
            public int ItalicAngle; 
            public uint EMSquare; 
            public int Ascent; 
            public int Descent; 
            public uint LineGap; 
            public uint sCapEmHeight; 
            public uint sXHeight; 
            public RECT rcFontBox; 
            public int MacAscent; 
            public int MacDescent; 
            public uint MacLineGap; 
            public uint usMinimumPPEM; 
            public POINT ptSubscriptSize; 
            public POINT ptSubscriptOffset; 
            public POINT ptSuperscriptSize; 
            public POINT ptSuperscriptOffset; 
            public uint sStrikeoutSize; 
            public int sStrikeoutPosition; 
            public int sUnderscoreSize; 
            public int sUnderscorePosition; 
            public uint pFamilyName;
            public uint pFaceName;
            public uint pStyleName;
            public uint pFullName; 
        };


        public const byte TMPF_FIXED_PITCH = 0x01;
        public const byte TMPF_VECTOR = 0x02;
        public const byte TMPF_DEVICE = 0x08;
        public const byte TMPF_TRUETYPE = 0x04;

        public const byte PAN_ANY                         = 0; /* Any                            */
        public const byte PAN_NO_FIT                      = 1; /* No Fit                         */

        public const byte PAN_FAMILY_TEXT_DISPLAY         = 2; /* Text and Display               */
        public const byte PAN_FAMILY_SCRIPT               = 3; /* Script                         */
        public const byte PAN_FAMILY_DECORATIVE           = 4; /* Decorative                     */
        public const byte PAN_FAMILY_PICTORIAL            = 5; /* Pictorial                      */

        public const byte PAN_SERIF_COVE                  = 2; /* Cove                           */
        public const byte PAN_SERIF_OBTUSE_COVE           = 3; /* Obtuse Cove                    */
        public const byte PAN_SERIF_SQUARE_COVE           = 4; /* Square Cove                    */
        public const byte PAN_SERIF_OBTUSE_SQUARE_COVE    = 5; /* Obtuse Square Cove             */
        public const byte PAN_SERIF_SQUARE                = 6; /* Square                         */
        public const byte PAN_SERIF_THIN                  = 7; /* Thin                           */
        public const byte PAN_SERIF_BONE                  = 8; /* Bone                           */
        public const byte PAN_SERIF_EXAGGERATED           = 9; /* Exaggerated                    */
        public const byte PAN_SERIF_TRIANGLE             = 10; /* Triangle                       */
        public const byte PAN_SERIF_NORMAL_SANS          = 11; /* Normal Sans                    */
        public const byte PAN_SERIF_OBTUSE_SANS          = 12; /* Obtuse Sans                    */
        public const byte PAN_SERIF_PERP_SANS            = 13; /* Prep Sans                      */
        public const byte PAN_SERIF_FLARED               = 14; /* Flared                         */
        public const byte PAN_SERIF_ROUNDED              = 15; /* Rounded                        */

        public const byte PAN_WEIGHT_VERY_LIGHT           = 2; /* Very Light                     */
        public const byte PAN_WEIGHT_LIGHT                = 3; /* Light                          */
        public const byte PAN_WEIGHT_THIN                 = 4; /* Thin                           */
        public const byte PAN_WEIGHT_BOOK                 = 5; /* Book                           */
        public const byte PAN_WEIGHT_MEDIUM               = 6; /* Medium                         */
        public const byte PAN_WEIGHT_DEMI                 = 7; /* Demi                           */
        public const byte PAN_WEIGHT_BOLD                 = 8; /* Bold                           */
        public const byte PAN_WEIGHT_HEAVY                = 9; /* Heavy                          */
        public const byte PAN_WEIGHT_BLACK               = 10; /* Black                          */
        public const byte PAN_WEIGHT_NORD                = 11; /* Nord                           */

        public const byte PAN_PROP_OLD_STYLE              = 2; /* Old Style                      */
        public const byte PAN_PROP_MODERN                 = 3; /* Modern                         */
        public const byte PAN_PROP_EVEN_WIDTH             = 4; /* Even Width                     */
        public const byte PAN_PROP_EXPANDED               = 5; /* Expanded                       */
        public const byte PAN_PROP_CONDENSED              = 6; /* Condensed                      */
        public const byte PAN_PROP_VERY_EXPANDED          = 7; /* Very Expanded                  */
        public const byte PAN_PROP_VERY_CONDENSED         = 8; /* Very Condensed                 */
        public const byte PAN_PROP_MONOSPACED             = 9; /* Monospaced                     */

        public const byte PAN_CONTRAST_NONE               = 2; /* None                           */
        public const byte PAN_CONTRAST_VERY_LOW           = 3; /* Very Low                       */
        public const byte PAN_CONTRAST_LOW                = 4; /* Low                            */
        public const byte PAN_CONTRAST_MEDIUM_LOW         = 5; /* Medium Low                     */
        public const byte PAN_CONTRAST_MEDIUM             = 6; /* Medium                         */
        public const byte PAN_CONTRAST_MEDIUM_HIGH        = 7; /* Mediim High                    */
        public const byte PAN_CONTRAST_HIGH               = 8; /* High                           */
        public const byte PAN_CONTRAST_VERY_HIGH          = 9; /* Very High                      */

        public const byte PAN_STROKE_GRADUAL_DIAG         = 2; /* Gradual/Diagonal               */
        public const byte PAN_STROKE_GRADUAL_TRAN         = 3; /* Gradual/Transitional           */
        public const byte PAN_STROKE_GRADUAL_VERT         = 4; /* Gradual/Vertical               */
        public const byte PAN_STROKE_GRADUAL_HORZ         = 5; /* Gradual/Horizontal             */
        public const byte PAN_STROKE_RAPID_VERT           = 6; /* Rapid/Vertical                 */
        public const byte PAN_STROKE_RAPID_HORZ           = 7; /* Rapid/Horizontal               */
        public const byte PAN_STROKE_INSTANT_VERT         = 8; /* Instant/Vertical               */

        public const byte PAN_STRAIGHT_ARMS_HORZ          = 2; /* Straight Arms/Horizontal       */
        public const byte PAN_STRAIGHT_ARMS_WEDGE         = 3; /* Straight Arms/Wedge            */
        public const byte PAN_STRAIGHT_ARMS_VERT          = 4; /* Straight Arms/Vertical         */
        public const byte PAN_STRAIGHT_ARMS_SINGLE_SERIF  = 5; /* Straight Arms/Single-Serif     */
        public const byte PAN_STRAIGHT_ARMS_DOUBLE_SERIF  = 6; /* Straight Arms/Double-Serif     */
        public const byte PAN_BENT_ARMS_HORZ              = 7; /* Non-Straight Arms/Horizontal   */
        public const byte PAN_BENT_ARMS_WEDGE             = 8; /* Non-Straight Arms/Wedge        */
        public const byte PAN_BENT_ARMS_VERT              = 9; /* Non-Straight Arms/Vertical     */
        public const byte PAN_BENT_ARMS_SINGLE_SERIF     = 10; /* Non-Straight Arms/Single-Serif */
        public const byte PAN_BENT_ARMS_DOUBLE_SERIF     = 11; /* Non-Straight Arms/Double-Serif */

        public const byte PAN_LETT_NORMAL_CONTACT         = 2; /* Normal/Contact                 */
        public const byte PAN_LETT_NORMAL_WEIGHTED        = 3; /* Normal/Weighted                */
        public const byte PAN_LETT_NORMAL_BOXED           = 4; /* Normal/Boxed                   */
        public const byte PAN_LETT_NORMAL_FLATTENED       = 5; /* Normal/Flattened               */
        public const byte PAN_LETT_NORMAL_ROUNDED         = 6; /* Normal/Rounded                 */
        public const byte PAN_LETT_NORMAL_OFF_CENTER      = 7; /* Normal/Off Center              */
        public const byte PAN_LETT_NORMAL_SQUARE          = 8; /* Normal/Square                  */
        public const byte PAN_LETT_OBLIQUE_CONTACT        = 9; /* Oblique/Contact                */
        public const byte PAN_LETT_OBLIQUE_WEIGHTED      = 10; /* Oblique/Weighted               */
        public const byte PAN_LETT_OBLIQUE_BOXED         = 11; /* Oblique/Boxed                  */
        public const byte PAN_LETT_OBLIQUE_FLATTENED     = 12; /* Oblique/Flattened              */
        public const byte PAN_LETT_OBLIQUE_ROUNDED       = 13; /* Oblique/Rounded                */
        public const byte PAN_LETT_OBLIQUE_OFF_CENTER    = 14; /* Oblique/Off Center             */
        public const byte PAN_LETT_OBLIQUE_SQUARE        = 15; /* Oblique/Square                 */

        public const byte PAN_MIDLINE_STANDARD_TRIMMED    = 2; /* Standard/Trimmed               */
        public const byte PAN_MIDLINE_STANDARD_POINTED    = 3; /* Standard/Pointed               */
        public const byte PAN_MIDLINE_STANDARD_SERIFED    = 4; /* Standard/Serifed               */
        public const byte PAN_MIDLINE_HIGH_TRIMMED        = 5; /* High/Trimmed                   */
        public const byte PAN_MIDLINE_HIGH_POINTED        = 6; /* High/Pointed                   */
        public const byte PAN_MIDLINE_HIGH_SERIFED        = 7; /* High/Serifed                   */
        public const byte PAN_MIDLINE_CONSTANT_TRIMMED    = 8; /* Constant/Trimmed               */
        public const byte PAN_MIDLINE_CONSTANT_POINTED    = 9; /* Constant/Pointed               */
        public const byte PAN_MIDLINE_CONSTANT_SERIFED   = 10; /* Constant/Serifed               */
        public const byte PAN_MIDLINE_LOW_TRIMMED        = 11; /* Low/Trimmed                    */
        public const byte PAN_MIDLINE_LOW_POINTED        = 12; /* Low/Pointed                    */
        public const byte PAN_MIDLINE_LOW_SERIFED        = 13; /* Low/Serifed                    */

        public const byte PAN_XHEIGHT_CONSTANT_SMALL      = 2; /* Constant/Small                 */
        public const byte PAN_XHEIGHT_CONSTANT_STD        = 3; /* Constant/Standard              */
        public const byte PAN_XHEIGHT_CONSTANT_LARGE      = 4; /* Constant/Large                 */
        public const byte PAN_XHEIGHT_DUCKING_SMALL       = 5; /* Ducking/Small                  */
        public const byte PAN_XHEIGHT_DUCKING_STD         = 6; /* Ducking/Standard               */
        public const byte PAN_XHEIGHT_DUCKING_LARGE = 7; /* Ducking/Large                  */

    }
}
