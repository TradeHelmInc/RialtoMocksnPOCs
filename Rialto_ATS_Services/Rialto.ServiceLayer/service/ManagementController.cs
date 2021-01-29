using fwk.Common.interfaces;
using Newtonsoft.Json;
using Rialto.Common.DTO.Generic;
using Rialto.Common.DTO.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


namespace Rialto.ServiceLayer.service
{
    public delegate string OnKCXOnboardingApproved(string koreShareholderId,string companyKoreChainId,string key,string IV);
    
    public delegate string OnKCXOnboardingApproved_4096(string[] param);

    public delegate string OnKCXOnboardingStarted(string koreShareholderId);

    //public class ManagementController : Controller
    [ApiController]
    public class ManagementController : BaseController
    {

        #region Public Static Attributs

        public static event OnKCXOnboardingApproved OnKCXOnboardingApproved;//SIGNAL!!
        
        public static event OnKCXOnboardingApproved_4096 OnKCXOnboardingApproved_4096;//SIGNAL!!

        //public static event OnKCXOnboardingStarted OnKCXOnboardingStarted;

        public static ILogger Logger { get; set; }

        #endregion

        #region Private Methods

        

        #endregion

        #region Public Methods

        [Route("[controller]/OnKCXOnboardingApproved_4096")]
        [HttpPost]
        public string  Post(HttpRequest Rq)
        {
         
        try
         {
             string jsonInput = GetBody(Rq.Body, Encoding.UTF8);
             OnKCXOnboardingApproved4096DTO onKCXOnboardingApproved = null;
             try
             {
                 onKCXOnboardingApproved = JsonConvert.DeserializeObject<OnKCXOnboardingApproved4096DTO>(jsonInput);
                 Logger.DoLog(string.Format("Incoming OnKCXOnboardingApproved for 4096 bits key received:{0}",
                     onKCXOnboardingApproved.GetCSV()), fwk.Common.enums.MessageType.Information);
             }
             catch (Exception ex)
             {
                 string msg = string.Format("Could not process input json {1}:{0}", jsonInput, ex.Message);
                 Logger.DoLog(msg, fwk.Common.enums.MessageType.Error);
                 throw new Exception(msg);
             }

             string txtId = OnKCXOnboardingApproved_4096( onKCXOnboardingApproved.Params);

             Logger.DoLog("OnKCXOnboardingApproved for 4096 bits key successfully processed", fwk.Common.enums.MessageType.Information);

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
                    Logger.DoLog(string.Format("Incoming OnKCXOnboardingApproved: KoreShareholderId={0}  Key={1} IV={2} CompanyKoreChainId={3} ",
                                                    onKCXOnboardingApproved.KoreShareholderId
                                                    , onKCXOnboardingApproved.Key != null ? onKCXOnboardingApproved.Key : "??"
                                                    , onKCXOnboardingApproved.IV != null ? onKCXOnboardingApproved.IV : "??"
                                                    , onKCXOnboardingApproved.CompanyKoreChainId != null ? onKCXOnboardingApproved.CompanyKoreChainId : "??"), 
                                fwk.Common.enums.MessageType.Information);
                }
                catch (Exception ex)
                {
                    string msg = string.Format("Could not process input json {1}:{0}", jsonInput, ex.Message);
                    Logger.DoLog(msg, fwk.Common.enums.MessageType.Error);
                    throw new Exception(msg);
                }

                string txtId = OnKCXOnboardingApproved( onKCXOnboardingApproved.KoreShareholderId, onKCXOnboardingApproved.CompanyKoreChainId, 
                                                        onKCXOnboardingApproved.Key,onKCXOnboardingApproved.IV);

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
