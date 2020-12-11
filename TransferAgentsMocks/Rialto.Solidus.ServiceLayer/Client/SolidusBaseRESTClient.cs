using fwk.ServiceLayer.REST;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Solidus.ServiceLayer.Client
{
    public class SolidusBaseRESTClient : BaseRESTClient
    {
        #region Protected Consts

        protected int _GENERIC_ERROR = -1;

        #endregion

        #region Protected Methods

        protected void DoPostJson(string url, Dictionary<string, string> headers, string json)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "POST";
            request.ContentType = "application/json";

            foreach (string key in headers.Keys)
            {
                request.Headers[key] = headers[key];
            }

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
                //streamWriter.Flush();
                //streamWriter.Close();
            }
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                string content = string.Empty;
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        content = sr.ReadToEnd();
                    }
                }
                //return new BaseResponse() { Resp = content };
            }
            catch (WebException ex)
            {
                //string errContent = string.Empty;
                //using (Stream stream = ex.Response.GetResponseStream())
                //{
                //    using (StreamReader sr = new StreamReader(stream))
                //    {
                //        errContent = sr.ReadToEnd();
                //    }
                //}
                //return new BaseResponse() { Resp = null, GenericError = JsonConvert.DeserializeObject<GenericError>(errContent) };
            }
            catch (Exception ex)
            {
                //return new BaseResponse() { Resp = null, GenericError = GetGenericError(ex.Message) };
            }
        }


        #endregion

    }
}
