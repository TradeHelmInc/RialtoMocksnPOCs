using fwk.Common.interfaces;
using Newtonsoft.Json;
using Rialto.Common.DTO.Generic;
using Rialto.Common.DTO.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Rialto.ServiceLayer.service
{
    public delegate string OnKCXOnboardingApproved(string koreShareholderId);

    //public delegate string OnKCXOnboardingStarted(string koreShareholderId);


    public class ManagementController : BaseController
    {

        #region Public Static Attributs

        public static event OnKCXOnboardingApproved OnKCXOnboardingApproved;//SIGNAL!!

        //public static event OnKCXOnboardingStarted OnKCXOnboardingStarted;

        public static ILogger Logger { get; set; }

        #endregion


        #region Public Methods

        [HttpPost]
        [ActionName("OnKCXOnboardingApproved")]
        public HttpResponseMessage OnKCXOnboardingApprovedSvc(HttpRequestMessage Request)
        {
            try
            {
                var content = Request.Content;
                string jsonInput = content.ReadAsStringAsync().Result;
                HttpResponseMessage resp = Request.CreateResponse(HttpStatusCode.OK);


                OnKCXOnboardingApprovedDTO onKCXOnboardingApproved = null;
                try
                {
                    onKCXOnboardingApproved = JsonConvert.DeserializeObject<OnKCXOnboardingApprovedDTO>(jsonInput);
                    Logger.DoLog(string.Format("Incoming OnKCXOnboardingApproved: KoreShareholderId={0} ", onKCXOnboardingApproved.KoreShareholderId), fwk.Common.enums.MessageType.Information);

                }
                catch (Exception ex)
                {
                    string msg = string.Format("Could not process input json {1}:{0}", ex.Message, jsonInput);
                    Logger.DoLog(msg, fwk.Common.enums.MessageType.Error);
                    throw new Exception(msg);
                }

                string txtId = OnKCXOnboardingApproved( onKCXOnboardingApproved.KoreShareholderId);

                Logger.DoLog("OnKCXOnboardingApproved successfully processed", fwk.Common.enums.MessageType.Information);

                TransactionResponse txResp = new TransactionResponse() { Success = true, Id = new IdEntity() { id = txtId } };

                resp.Content = new StringContent(JsonConvert.SerializeObject(txResp), Encoding.UTF8, "application/json");

                return resp;
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error @OnKCXOnboardingApproved :{0}", ex.Message);
                Logger.DoLog(msg, fwk.Common.enums.MessageType.Error);
                return CreateTransactionError(Request, ex.Message);
            }
        }

        //[HttpPost]
        //[ActionName("OnKCXOnboardingStared")]
        //public HttpResponseMessage OnKCXOnboardingStaredSvc(HttpRequestMessage Request)
        //{
        //    try
        //    {
        //        var content = Request.Content;
        //        string jsonInput = content.ReadAsStringAsync().Result;
        //        HttpResponseMessage resp = Request.CreateResponse(HttpStatusCode.OK);


        //        OnKCXOnboardingStaredDTO onKCXOnboardingStared = null;
        //        try
        //        {
        //            onKCXOnboardingStared = JsonConvert.DeserializeObject<OnKCXOnboardingStaredDTO>(jsonInput);
        //            Logger.DoLog(string.Format("Incoming OnKCXOnboardingStared: KoreShareholderId={0} ", onKCXOnboardingStared.KoreShareholderId), fwk.Common.enums.MessageType.Information);

        //        }
        //        catch (Exception ex)
        //        {
        //            string msg = string.Format("Could not process input json {1}:{0}", ex.Message, jsonInput);
        //            Logger.DoLog(msg, fwk.Common.enums.MessageType.Error);
        //            throw new Exception(msg);
        //        }

        //        string txtId = OnKCXOnboardingStarted(onKCXOnboardingStared.KoreShareholderId);

        //        Logger.DoLog("OnKCXOnboardingStared successfully processed", fwk.Common.enums.MessageType.Information);

        //        TransactionResponse txResp = new TransactionResponse() { Success = true, Id = new IdEntity() { id = txtId } };

        //        resp.Content = new StringContent(JsonConvert.SerializeObject(txResp), Encoding.UTF8, "application/json");

        //        return resp;
        //    }
        //    catch (Exception ex)
        //    {
        //        string msg = string.Format("Error @OnKCXOnboardingStared :{0}", ex.Message);
        //        Logger.DoLog(msg, fwk.Common.enums.MessageType.Error);
        //        return CreateTransactionError(Request, ex.Message);
        //    }
        //}

        #endregion
    }
}
