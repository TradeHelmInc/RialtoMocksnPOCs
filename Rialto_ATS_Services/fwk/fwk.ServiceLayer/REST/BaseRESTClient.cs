using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace fwk.ServiceLayer.REST
{
    public class BaseRESTClient
    {
        #region Protected Attributes

        protected string BaseURL { get; set; }
        
        protected string UserName { get; set; }
        
        protected string Password { get; set; }

        #endregion


        #region Protected Methods

        protected string DoPost(string url, Dictionary<string, string> headers, Dictionary<string, string> body)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "POST";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            request.ContentType = "application/x-www-form-urlencoded";

            foreach (string key in headers.Keys)
            {
                request.Headers[key] = headers[key];
            }

            string postStr = "";

            foreach (string key in body.Keys)
                postStr += string.Format("{0}={1}&", key, body[key]);

            var postData = Encoding.ASCII.GetBytes(postStr);

            using (var stream = request.GetRequestStream())
            {
                stream.Write(postData, 0, postData.Length);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            string content = string.Empty;
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    content = sr.ReadToEnd();
                }
            }
            return content;
        }





        //protected string DoGetJson(string url, Dictionary<string, string> body)
        //{
        //    string content = string.Empty;

        //    string postStr = "";

        //    url += body.Keys.Count > 0 ? "?" : "";

        //    foreach (string key in body.Keys)
        //    {
        //        postStr += string.Format("{0}={1}&", key, body[key]);
        //    }

        //    url += postStr;


        //    using (var httpClient = new HttpClient())
        //    {
        //        var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
        //        requestMessage.Headers.Add("Accept", "application/json");
        //        requestMessage.Headers.Add("ContentType", "application/json");
        //        content = httpClient.SendAsync(requestMessage).Result.Content.ReadAsStringAsync().Result;
        //    }
        //    return content;
        //}

        #endregion
    }
}
