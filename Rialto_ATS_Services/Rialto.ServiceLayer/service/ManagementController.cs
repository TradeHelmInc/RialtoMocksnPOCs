﻿using fwk.Common.interfaces;
using Newtonsoft.Json;
using Rialto.Common.DTO.Generic;
using Rialto.Common.DTO.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;


namespace Rialto.ServiceLayer.service
{
    public delegate string OnKCXOnboardingApproved(string koreShareholderId,string key,string IV);

    public delegate string OnKCXOnboardingStarted(string koreShareholderId);

    //public class ManagementController : Controller
    public class ManagementController : BaseController
    {

        #region Public Static Attributs

        public static event OnKCXOnboardingApproved OnKCXOnboardingApproved;//SIGNAL!!

        //public static event OnKCXOnboardingStarted OnKCXOnboardingStarted;

        public static ILogger Logger { get; set; }

        #endregion

        #region Private Methods

        

        #endregion

        #region Public Methods
        
        [Route("[controller]/OnKCXOnboardingApproved")]
        [HttpPost]
        public string Post()
        {
            try
            {
                string jsonInput = GetBody(Request.Body, Encoding.UTF8);
                OnKCXOnboardingApprovedDTO onKCXOnboardingApproved = null;
                try
                {
                    onKCXOnboardingApproved = JsonConvert.DeserializeObject<OnKCXOnboardingApprovedDTO>(jsonInput);
                    Logger.DoLog(string.Format("Incoming OnKCXOnboardingApproved: KoreShareholderId={0}  Key={1} IV={2} ",
                                                    onKCXOnboardingApproved.KoreShareholderId
                                                    , onKCXOnboardingApproved.Key != null ? onKCXOnboardingApproved.Key : "??"
                                                    , onKCXOnboardingApproved.IV != null ? onKCXOnboardingApproved.IV : "??"), 
                                fwk.Common.enums.MessageType.Information);
                }
                catch (Exception ex)
                {
                    string msg = string.Format("Could not process input json {1}:{0}", ex.Message, jsonInput);
                    Logger.DoLog(msg, fwk.Common.enums.MessageType.Error);
                    throw new Exception(msg);
                }

                string txtId = OnKCXOnboardingApproved( onKCXOnboardingApproved.KoreShareholderId,onKCXOnboardingApproved.Key,onKCXOnboardingApproved.IV);

                Logger.DoLog("OnKCXOnboardingApproved successfully processed", fwk.Common.enums.MessageType.Information);

                TransactionResponse txResp = new TransactionResponse() { Success = true, Id = new IdEntity() { id = txtId } };

                return JsonConvert.SerializeObject(txResp);
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error @OnKCXOnboardingApproved :{0}", ex.Message);
                Logger.DoLog(msg, fwk.Common.enums.MessageType.Error);
                TransactionResponse txError =  CreateTransactionError(ex.Message);
                return JsonConvert.SerializeObject(txError);
            }
        }
       
        #endregion
    }
}