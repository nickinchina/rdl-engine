using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Rdl.Render
{
    public class ImageData
    {
        public System.Drawing.Image imageData = null;
        public string MimeType = null;
        public Rdl.Engine.BackgroundImage.ImageRepeatEnum ImageRepeat = Rdl.Engine.BackgroundImage.ImageRepeatEnum.NoRepeat;
        public Rdl.Engine.Image.ImageSizingEnum Sizing = Rdl.Engine.Image.ImageSizingEnum.Autosize;

        public System.Drawing.Image GetSizedImage(int width, int height)
        {
            if ((Sizing == Rdl.Engine.Image.ImageSizingEnum.Clip || Sizing == Rdl.Engine.Image.ImageSizingEnum.Fit || Sizing == Rdl.Engine.Image.ImageSizingEnum.FitProportional) &&
                width > 0 && height > 0)
            {
                // Create a new bitmap for the image
                System.Drawing.Bitmap bm = null;

                System.Drawing.Rectangle sourceRect = System.Drawing.Rectangle.Empty;
                System.Drawing.Rectangle destRect = System.Drawing.Rectangle.Empty;

                if (Sizing == Rdl.Engine.Image.ImageSizingEnum.Clip)
                {
                    bm = new System.Drawing.Bitmap(width, height);

                    // Create the clipping rectangles
                    int xAdj = (width > imageData.Width) ? 0 : ((imageData.Width - width) / 2);
                    int yAdj = (height > imageData.Height) ? 0 : ((imageData.Height - height) / 2);
                    sourceRect = new System.Drawing.Rectangle(xAdj, yAdj, imageData.Width - xAdj, imageData.Height - yAdj);

                    xAdj = (imageData.Width > width) ? 0 : ((width - imageData.Width) / 2);
                    yAdj = (imageData.Height > height) ? 0 : ((height - imageData.Height) / 2);
                    destRect = new System.Drawing.Rectangle(xAdj, yAdj, width - xAdj, height - yAdj);
                }
                if (Sizing == Rdl.Engine.Image.ImageSizingEnum.Fit)
                {
                    bm = new System.Drawing.Bitmap(width, height);

                    // Resize the whole image to the full size of the box.
                    sourceRect = new System.Drawing.Rectangle(0, 0, imageData.Width, imageData.Height);
                    destRect = new System.Drawing.Rectangle(0, 0, width, height);
                }
                if (Sizing == Rdl.Engine.Image.ImageSizingEnum.FitProportional)
                {
                    // Resize the image proportionally to fit the box.
                    sourceRect = new System.Drawing.Rectangle(0, 0, imageData.Width, imageData.Height);
                    int xAdj = 0;
                    int yAdj = 0;
                    float f1 = (float)imageData.Width / (float)imageData.Height;
                    float f2 = (float)width / (float)height;
                    if (f1 > f2)
                        yAdj = (height - (int)((float)width / f1));
                    else
                        xAdj = (width - (int)((float)height * f1));

                    bm = new System.Drawing.Bitmap(width - xAdj, height - yAdj);
                    destRect = new System.Drawing.Rectangle(0, 0, width - xAdj, height - yAdj);
                }

                // Draw the image onto the new bitmap.
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bm);
                g.DrawImage(imageData, destRect, sourceRect, System.Drawing.GraphicsUnit.Pixel);
                g.Dispose();

                return bm;
            }
            else if (ImageRepeat != Rdl.Engine.BackgroundImage.ImageRepeatEnum.NoRepeat && width > 0 && height > 0)
            {
                // If the image is repeated and we know the space that it is going to be repeated into
                // then draw multiple copies of the image into a new sized bitmap

                // Create a new bitmap for the image
                System.Drawing.Bitmap bm = new System.Drawing.Bitmap(width, height);;
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bm);

                System.Drawing.Rectangle sourceRect = System.Drawing.Rectangle.Empty;
                System.Drawing.Rectangle destRect = System.Drawing.Rectangle.Empty;

                int x = 0; int y = 0;
                bool done = false;
                while (!done)
                {
                    sourceRect = new System.Drawing.Rectangle(0, 0,
                        Math.Min(imageData.Width, width - x),
                        Math.Min(imageData.Height, height - y));
                    destRect = new System.Drawing.Rectangle(x, y,
                        Math.Min(imageData.Width, width - x),
                        Math.Min(imageData.Height, height - y));

                    // Draw the image onto the new bitmap.
                    g.DrawImage(imageData, destRect, sourceRect, System.Drawing.GraphicsUnit.Pixel);

                    if (ImageRepeat == Rdl.Engine.BackgroundImage.ImageRepeatEnum.Repeat ||
                        ImageRepeat == Rdl.Engine.BackgroundImage.ImageRepeatEnum.RepeatX)
                    {
                        x += imageData.Width;
                        if (x >= width)
                        {
                            if (ImageRepeat == Rdl.Engine.BackgroundImage.ImageRepeatEnum.Repeat)
                            {
                                x = 0;
                                y += imageData.Height;
                            }
                            else
                                done = true;
                        }
                    }

                    if (ImageRepeat == Rdl.Engine.BackgroundImage.ImageRepeatEnum.RepeatY)
                        y += imageData.Height;

                    if (y >= height)
                        done = true;
                }
                g.Dispose();
                return bm;
            }
            else
                return imageData;
        }

        public byte[] GetSizedImageData(int width, int height)
        {
            MemoryStream ms = new MemoryStream();

            GetSizedImage(width, height).Save(ms, imageData.RawFormat);

            return ms.ToArray();
        }

        public byte[] GetImageData()
        {
            MemoryStream ms = new MemoryStream();
            imageData.Save(ms, imageData.RawFormat);

            return ms.ToArray();
        }

        public bool CompareTo(System.Drawing.Image img2)
        {
            if (imageData.RawFormat.Guid.CompareTo(img2.RawFormat.Guid) != 0)
                return false;
            MemoryStream ms1 = new MemoryStream();
            MemoryStream ms2 = new MemoryStream();
            imageData.Save(ms1, imageData.RawFormat);
            img2.Save(ms2, img2.RawFormat);

            if (ms1.Length != ms2.Length)
                return false;

            byte[] buffer1 = ms1.ToArray();
            byte[] buffer2 = ms2.ToArray();
            for (int i = 0; i < buffer1.Length; i++)
                if (buffer1[i] != buffer2[i])
                    return false;
            return true;
        }
    }
}
