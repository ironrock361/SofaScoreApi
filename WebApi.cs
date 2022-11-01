using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SofaScoreApi
{
    public static class WebApi
    {
        public static async Task<string> GetAsync(string uri)
        {
            using (WebClient wc = new WebClient())
            {
                return wc.DownloadString(uri);
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        return await reader.ReadToEndAsync();
                    }
                }
            }
        }

        public static string Get(string uri)
        {
            using (WebClient wc = new WebClient())
            {
                return wc.DownloadString(uri);
            }
        }
    }
}
