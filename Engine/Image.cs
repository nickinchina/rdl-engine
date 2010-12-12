using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace Rdl.Engine
{
    public class Image : ReportItem
    {
        protected enum ImageSourceEnum
        {
            External,
            Embedded,
            Database
        }
        public enum ImageSizingEnum
        {
            Autosize,
            Fit,
            FitProportional,
            Clip,
        }

        protected ImageSourceEnum _source = ImageSourceEnum.External;
        internal Expression _value = null;
        internal Expression _mIMEType = null;
        protected ImageSizingEnum _sizing = ImageSizingEnum.Autosize;
        internal int _imageIndex = -1;

        public Image(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            base.ParseAttribute(attr);
            switch (attr.Name.ToLower())
            {
                case "source":
                    _source = (ImageSourceEnum)Enum.Parse(typeof(ImageSourceEnum), attr.InnerText, true);
                    break;
                case "value":
                    _value = new Expression(attr, this);
                    break;
                case "mimetype":
                    _mIMEType = new Expression(attr, this);
                    break;
                case "sizing":
                    _sizing = (ImageSizingEnum)Enum.Parse(typeof(ImageSizingEnum), attr.InnerText, true);
                    break;
                default:
                    break;
            }
        }

        public System.Drawing.Image GetImage(Rdl.Runtime.Context context)
        {
            System.Drawing.Image img = null;
            string name;

            switch (_source)
            {
                case ImageSourceEnum.Database:
                    img = System.Drawing.Image.FromStream(new MemoryStream((byte[])_value.Exec(context)));
                    break;
                case ImageSourceEnum.Embedded:
                    name = _value.ExecAsString(context);
                    try
                    {
                        EmbeddedImage e = Report.GetEmbeddedImage(name);
                        img = System.Drawing.Image.FromStream(new MemoryStream(e.ImageData));
                    }
                    catch (Exception err)
                    {
                        throw new Exception("Error locating embedded image " + name, err);
                    }
                    break;
                case ImageSourceEnum.External:
                    name = _value.ExecAsString(context);
                    try
                    {
                        Stream s = null;
                        if (name.Contains("://"))
                            s = System.Net.HttpWebRequest.Create(name).GetResponse().GetResponseStream();
                        else
                            s = new FileStream(name, FileMode.Open, FileAccess.Read);
                        img = System.Drawing.Image.FromStream(s);
                        s.Close();
                        s.Dispose();
                    }
                    catch (Exception err)
                    {
                        throw new Exception("Error loading image file " + name, err);
                    }
                    break;
            }

            return img;
        }

        public string MIMEType(Rdl.Runtime.Context context)
        {
            System.Drawing.Image img = GetImage(context);

            return MIMEType(img);
        }

        private string MIMEType(System.Drawing.Image img)
        {
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Bmp))
                return "image/bmp";
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Gif))
                return "image/gif";
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                return "image/jpeg";
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Png))
                return "image/png";

            throw new Exception("Unknown image type " + img.RawFormat.ToString());
        }

        protected override void Render1(Rdl.Render.Container parentBox, Rdl.Runtime.Context context, bool visible)
        {
            if (visible && parentBox != null)
            {
                _box = parentBox.AddImageElement(this, Style, context);
                _box.Name = "Image";
            }
        }

        protected override void Render2(Rdl.Runtime.Context context)
        {
            if (_box != null)
            {
                Rdl.Render.ImageElement ie = (Rdl.Render.ImageElement)_box;

                ie._imageIndex = ie.Render.AddImage(this, context);

                if (_sizing == ImageSizingEnum.Autosize)
                {
                    ie.Width = ie.Render.ImageList[ie._imageIndex].imageData.Width;
                    ie.Height = ie.Render.ImageList[ie._imageIndex].imageData.Height;
                }
            }
        }

        public virtual ImageSizingEnum Sizing
        {
            get { return _sizing; }
        }
    }
}
