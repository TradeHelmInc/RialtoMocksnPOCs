using fwk.ServiceLayer.REST;
using Newtonsoft.Json;
using Rialto.KoreConX.Common.DTO.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.KoreConX.ServiceLayer.Client
{
    public class KCXBaseRESTClient:BaseRESTClient
    {

        #region Protected Consts

        protected int _GENERIC_ERROR = -1;

        #endregion

        #region Protected Methods

        protected GenericError  GetGenericError(string msg)
        {
            return new GenericError() { message = msg };
        
        }

        protected GenericGetErrorMsg GetGenericGetErrorMsg(string msg)
        {
            return new GenericGetErrorMsg() { code = 500, msg = msg, details = null };
        
        }

        protected bool IsGenericGetError(string msg)
        {
            try
            {
                GenericGetError error = JsonConvert.DeserializeObject<GenericGetError>(msg);

                return error.message != null;
            }
            catch (Exception ex)
            {
                return true;
            }
        }

        protected ValidationResponse GetGenericGetError(string json)
        {
            GenericGetError errorMsg = JsonConvert.DeserializeObject<GenericGetError>(json, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });


            return new ValidationResponse() { Resp = json, message = errorMsg };
        }

        protected ValidationResponse BuildGetError(string msg)
        {
            return new ValidationResponse()
            {
                Resp = null,
                message = new GenericGetError()
                    {
                        message = new GenericGetErrorMsg() { code = 500, msg = msg, details = null },
                        strMessage=msg
                        
                    }

            };
        }


        protected ValidationResponse DoGetJson(string url, Dictionary<string, string> body)
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
                //httpClient.DefaultRequestHeaders.Authorization =new AuthenticationHeaderValue("Bearer", "YWRtaW46N25BOUJqN05hZU5E");
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                requestMessage.Headers.Add("Accept", "application/json");
                requestMessage.Headers.Add("ContentType", "application/json");
                httpClient.DefaultRequestHeaders.Authorization =
                 new AuthenticationHeaderValue(
                     "Basic", Convert.ToBase64String(
                         System.Text.ASCIIEncoding.ASCII.GetBytes(
                            "admin:7nA9Bj7NaeND")));
                try
                {
                    content = httpClient.SendAsync(requestMessage).Result.Content.ReadAsStringAsync().Result;
                    if (!IsGenericGetError(content))
                        return new ValidationResponse() { Resp = content };
                    else
                        return GetGenericGetError(content);
                }
                catch (Exception ex)
                {
                    return BuildGetError(ex.Message);
                }
            }
        }


        protected BaseResponse DoPostJson(string url, Dictionary<string, string> headers, string json)
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
                return new BaseResponse() { Resp = content };
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
                return new BaseResponse() { Resp = null, GenericError = JsonConvert.DeserializeObject<GenericError>(errContent) };
            }
            catch (Exception ex)
            {
                return new BaseResponse() { Resp = null, GenericError = GetGenericError(ex.Message)};
            }
        }


        #endregion
    }
}
