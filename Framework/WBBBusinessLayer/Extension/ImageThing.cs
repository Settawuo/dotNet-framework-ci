using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using System;
using System.Collections.Generic;

namespace WBBBusinessLayer.Extension
{
    public class ImageThing : IImageProvider
    {
        //Store a reference to the main document so that we can access the page size and margins
        private Document MainDoc = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="doc"></param>
        public ImageThing(Document doc)
        {
            this.MainDoc = doc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        /// <param name="attrs"></param>
        /// <param name="chain"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        Image IImageProvider.GetImage(string src, IDictionary<string, string> attrs, ChainedProperties chain, IDocListener doc)
        {
            //Prepend the src tag with our path. NOTE, when using HTMLWorker.IMG_PROVIDER, HTMLWorker.IMG_BASEURL gets ignored unless you choose to implement it on your own

            // Local image file
            //src = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + src;

            //Get the image. NOTE, this will attempt to download/copy the image, you'd really want to sanity check here
            Image img = null;
            if (src.IndexOf("data:image/png;base64,") < 0)
            {
                img = Image.GetInstance(src);
                //Make sure we got something
                if (img == null)
                    return null;
                img.ScalePercent(40);
            }
            else
            {
                Byte[] bitmapData = Convert.FromBase64String(FixBase64ForImage(src.Replace("data:image/png;base64,", "")));
                img = Image.GetInstance(bitmapData);
                //Make sure we got something
                if (img == null)
                    return null;
                img.ScalePercent(40);
            }
            //If the downloaded image is bigger than either width and/or height then shrink it
            //if (img.Width > usableW || img.Height > usableH)
            //{
            //    img.ScaleToFit(usableW, usableH);
            //    img.ScaleAbsolute(img.Width, img.Height);
            //}

            //return our image
            return img;
        }

        public string FixBase64ForImage(string Image)
        {
            System.Text.StringBuilder sbText = new System.Text.StringBuilder(Image, Image.Length);
            sbText.Replace("\r\n", String.Empty); sbText.Replace(" ", String.Empty);
            return sbText.ToString();
        }
    }
}
