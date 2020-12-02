using fwk.ServiceLayer.REST;
using Newtonsoft.Json;
using Rialto.Rialto.Common.DTO.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Rialto.ServiceLayer.Client
{
    public class RialtoBaseRESTClient : BaseRESTClient
    {

        #region Protected Consts

        protected int _GENERIC_ERROR = -1;

        #endregion

        #region Protected Methods

        protected string DoGetJson(string url, Dictionary<string, string> body)
        {
            string content = string.Empty;

            string postStr = "";

            url += body.Keys.Count > 0 ? "?" : "";

            foreach (string key in body.Keys)
            {
                postStr += string.Format("{0}={1}&", key, body[key]);
            }

            url += postStr;


            using (var httpClient = new HttpClient())
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                requestMessage.Headers.Add("Accept", "application/json");
                requestMessage.Headers.Add("ContentType", "application/json");
                content = httpClient.SendAsync(requestMessage).Result.Content.ReadAsStringAsync().Result;

                return content;
            }
        }


        protected TransactionResponse DoPostJson(string url, Dictionary<string, string> headers, string json)
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
                TransactionResponse resp = JsonConvert.DeserializeObject<TransactionResponse>(content, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                return resp;
            }
            catch (WebException ex)
            {
                string errContent = string.Empty;
                using (Stream stream = ex.Response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        errContent = sr.ReadToEnd();
                    }
                }

                TransactionResponse errResp = JsonConvert.DeserializeObject<TransactionResponse>(errContent, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                return errResp;
            }
            catch (Exception ex)
            {
                return new TransactionResponse() { Success = false, Error = new ErrorMessage() { code = 500, msg = ex.Message } };
            }
        }


        #endregion
    }
}
