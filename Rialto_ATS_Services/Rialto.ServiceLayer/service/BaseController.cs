using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using fwk.ServiceLayer.REST;
//using Microsoft.AspNetCore.Mvc;
using Rialto.Common.DTO.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Rialto.ServiceLayer.service
{
    public class BaseController : Controller
    {
        #region Protected Methods
        
        protected string GetBody(Stream stream,Encoding enc)
        {
            using (StreamReader reader = new StreamReader(stream, enc))
            {  
                string resp = reader.ReadToEnd();
                return resp;
            }
        }

        public TransactionResponse CreateTransactionError(string msg)
        {
            //Error = new ErrorMessage { code = 500, msg = msg }

            TransactionResponse error = new TransactionResponse()
            {
                Success = false,Error  = new ErrorMessage { code = 500, msg = msg }
            };

            return error;
        }
        
        public string CreateStrTransactionError(string msg)
        {
            //Error = new ErrorMessage { code = 500, msg = msg }

            TransactionResponse error = new TransactionResponse()
            {
                Success = false,Error  = new ErrorMessage { code = 500, msg = msg }
            };
            
            return JsonConvert.SerializeObject(error);
        }

        public GetResponse CreateGetError(string msg)
        {
            GetResponse genResp = new GetResponse()
            {


                Success = false,
                Error = new ErrorMessage {code = 500, msg = msg}

            };
            
            return genResp;
        }
        #endregion
    }
}
