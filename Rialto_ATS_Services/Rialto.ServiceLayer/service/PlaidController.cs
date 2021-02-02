using System;
using System.Text;
using fwk.Common.interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Rialto.Common.DTO.Generic;
using Rialto.Common.DTO.Services;

namespace Rialto.ServiceLayer.service
{
    public delegate string OnPlaidCredentialsLoad(string userIdentifier,string plaidAccessToken,string plaidItemId);
    
    public class PlaidController : BaseController
    {
        #region Public Static Attributs

        public static event OnPlaidCredentialsLoad OnPlaidCredentialsLoad;

        public static ILogger Logger { get; set; }

        #endregion
        
        #region Public Methods
        
        [Route("[controller]/OnPlaidCredentialsLoad")]
        [HttpPost]
        public string Post(HttpRequest Rq)
        {
            try
            {
                string jsonInput = GetBody(Rq.Body, Encoding.UTF8);
                OnPlaidCredentialsLoadDTO onPlaidCredentialsLoaddDTO = null;
                try
                {
                    onPlaidCredentialsLoaddDTO = JsonConvert.DeserializeObject<OnPlaidCredentialsLoadDTO>(jsonInput);
                    Logger.DoLog(string.Format(
                            "Incoming OnPlaidCredentialsLoad: UserIdentifier={0}  PlaidAccessToken={1} PlaidItemId={2} ",
                            onPlaidCredentialsLoaddDTO.UserIdentifier
                            , onPlaidCredentialsLoaddDTO.PlaidAccessToken
                            , onPlaidCredentialsLoaddDTO.PlaidItemId),
                        fwk.Common.enums.MessageType.Information);
                }
                catch (Exception ex)
                {
                    string msg = string.Format("Could not process input json {1}:{0}", ex.Message, jsonInput);
                    Logger.DoLog(msg, fwk.Common.enums.MessageType.Error);
                    throw new Exception(msg);
                }

                string txtId = OnPlaidCredentialsLoad( onPlaidCredentialsLoaddDTO.UserIdentifier,onPlaidCredentialsLoaddDTO.PlaidAccessToken,onPlaidCredentialsLoaddDTO.PlaidItemId);

                Logger.DoLog("OnPlaidCredentialsLoad successfully processed", fwk.Common.enums.MessageType.Information);

                TransactionResponse txResp = new TransactionResponse() { Success = true, Id = new IdEntity() { id = txtId } };

                return JsonConvert.SerializeObject(txResp);
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error @OnPlaidCredentialsLoadDTO :{0}", ex.Message);
                Logger.DoLog(msg, fwk.Common.enums.MessageType.Error);
                TransactionResponse txError =  CreateTransactionError(ex.Message);
                return JsonConvert.SerializeObject(txError);
            }
        }
       
        #endregion
    }
}