using System;
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

            byte[] tmpImg = GetBufferFromImage(imageLink);
            if(tmpImg != null)
                context.Response.BinaryWrite(tmpImg);
        }

        private byte[] GetBufferFromImage(string imageLnk)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(imageLnk);

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Image myImage = Image.FromStream(response.GetResponseStream());
                MemoryStream ImageStream = new MemoryStream();
                myImage.Save(ImageStream, ImageFormat.Jpeg);
                ImageStream.Position = 0;
                byte[] Buffer = new byte[(int)ImageStream.Length];
                ImageStream.Read(Buffer, 0, (int)ImageStream.Length);
                return Buffer;
            }
            catch (Exception ex)
            {
                return null;
            }
           
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