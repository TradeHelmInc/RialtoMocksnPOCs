using fwk.ServiceLayer.REST;
using Newtonsoft.Json;
using Rialto.KoreConX.Common.DTO.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
