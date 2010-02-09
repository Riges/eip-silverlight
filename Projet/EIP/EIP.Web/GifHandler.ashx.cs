﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;

namespace EIP.Web
{
    /// <summary>
    /// Summary description for GifHandler
    /// </summary>
    public class GifHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            /*context.Response.ContentType = "text/plain";*/
            /*context.Response.Write("Hello World");*/
            string imageLink = context.Request.Params["link"];

            context.Response.ContentType = "image/jpeg";
            context.Response.BinaryWrite(GetBufferFromImage(imageLink));
        }

        private byte[] GetBufferFromImage(string imageLnk)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(imageLnk);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Image myImage = Image.FromStream(response.GetResponseStream());
            MemoryStream ImageStream = new MemoryStream();
            myImage.Save(ImageStream, ImageFormat.Jpeg);
            ImageStream.Position = 0;
            byte[] Buffer = new byte[(int)ImageStream.Length];
            ImageStream.Read(Buffer, 0, (int)ImageStream.Length);
            return Buffer;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}