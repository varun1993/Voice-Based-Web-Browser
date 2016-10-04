using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;
using UWIC.FinalProject.Common;

namespace UWIC.FinalProject.WebBrowser.Model
{
    public class BrowserContainerModel
    {
        /// <summary>
        /// This method is used to return a Bitmap Image from an Drawing.Image value
        /// </summary>
        /// <param name="value">System.Drawing.Image image</param>
        /// <returns></returns>
        public BitmapImage GetFavicon(System.Drawing.Image value)
        {
            try
            {
                if (value == null) { return null; }

                var image = value;
                // Winforms Image we want to get the WPF Image from...
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                var memoryStream = new MemoryStream();
                // Save to a memory stream...
                image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
                // Rewind the stream...
                memoryStream.Seek(0, SeekOrigin.Begin);
                bitmap.StreamSource = memoryStream;
                bitmap.EndInit();
                return bitmap;
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
            
        }

        /// <summary>
        /// This method is used to get the favicon from a particular website
        /// </summary>
        /// <param name="url">Url</param>
        /// <returns></returns>
        public System.Drawing.Image GetImageSource(Uri url)
        {
            try
            {
                var iconurl = "http://" + url.Host + "/favicon.ico";

                try
                {
                    var request = WebRequest.Create(iconurl);
                    var response = request.GetResponse();
                    var s = response.GetResponseStream();
                    return s != null ? System.Drawing.Image.FromStream(s) : null;
                }
                catch (Exception)
                {
                    //return Properties.Resources.LargeGlobe;
                    return null;
                }
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// This method is used to get the title of a particular webpage
        /// </summary>
        /// <param name="url"></param>
        public string GetWebPageTitle(string url)
        {
            var title = "";
            try
            {
                var request = (WebRequest.Create(url) as HttpWebRequest);
                if (request != null)
                {
                    var response = (request.GetResponse() as HttpWebResponse);

                    if (response != null)
                        using (var stream = response.GetResponseStream())
                        {
                            // compiled regex to check for <title></title> block
                            var titleCheck = new Regex(@"<title>\s*(.+?)\s*</title>",
                                                       RegexOptions.Compiled | RegexOptions.IgnoreCase);
                            const int bytesToRead = 8092;
                            var buffer = new byte[bytesToRead];
                            var contents = "";
                            int length;
                            while (stream != null && (length = stream.Read(buffer, 0, bytesToRead)) > 0)
                            {
                                // convert the byte-array to a string and add it to the rest of the
                                // contents that have been downloaded so far
                                contents += Encoding.UTF8.GetString(buffer, 0, length);

                                var m = titleCheck.Match(contents);
                                if (m.Success)
                                {
                                    // we found a <title></title> match =]
                                    title = m.Groups[1].Value;
                                    break;
                                }
                                if (contents.Contains("</head>"))
                                {
                                    title = "";
                                    // reached end of head-block; no title found
                                    break;
                                }
                            }
                        }
                }
            }
            catch (WebException ex)
            {
                Log.ErrorLog(ex);
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }

            return title;
        }
    }
}
