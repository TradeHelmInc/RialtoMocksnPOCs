using fwk.Common.interfaces;
using fwk.ServiceLayer.REST;
using Newtonsoft.Json;
using Rialto.BusinessEntities;
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
    public delegate string OnTransferShares(int buyShareholderId, int sellShareholderId, double tradeQuantity,int securityId, int sellOrderId);
    public delegate List<Trade> OnGetTradesToClear();

    public class TradingController : BaseController
    {

        #region Public Static Attributs

        public static event OnTransferShares OnTransferShares;

        public static event OnGetTradesToClear OnGetTradesToClear;

        public static ILogger Logger { get; set; }

        #endregion

        #region Public Methods
        [HttpPost]
        [ActionName("TransferShares")]
        public HttpResponseMessage TransferShares(HttpRequestMessage Request)
        {
            try
            {
                var content = Request.Content;
                string jsonInput = content.ReadAsStringAsync().Result;
                HttpResponseMessage resp = Request.CreateResponse(HttpStatusCode.OK);


                TransferSharesDTO transferDto = null;
                try
                {
                    transferDto = JsonConvert.DeserializeObject<TransferSharesDTO>(jsonInput);
                    Logger.DoLog(string.Format("Incoming TransferShares: BuyShareholderId={0} SellShareholderId={1} SecurityId={2} TradeQuantity={3} ", transferDto.BuyShareholderId, transferDto.SellShareholderId, transferDto.SecurityId, transferDto.TradeQuantity), fwk.Common.enums.MessageType.Information);

                }
                catch (Exception ex)
                {
                    string msg = string.Format("Could not process input json {1}:{0}", ex.Message, jsonInput);
                    Logger.DoLog(msg, fwk.Common.enums.MessageType.Error);
                    throw new Exception(msg);
                }

                string txtId = OnTransferShares(transferDto.BuyShareholderId, transferDto.SellShareholderId, transferDto.TradeQuantity, transferDto.SecurityId, transferDto.SellOrderId);

                Logger.DoLog("TransferShares successfully processed", fwk.Common.enums.MessageType.Information);

                TransactionResponse txResp = new TransactionResponse() { Success = true, Id = new IdEntity() { id = txtId } };

                resp.Content = new StringContent(JsonConvert.SerializeObject(txResp), Encoding.UTF8, "application/json");

                return resp;
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error @TransferShares :{0}", ex.Message);
                Logger.DoLog(msg, fwk.Common.enums.MessageType.Error);
                return CreateTransactionError(Request, ex.Message);
            }
        }

        [HttpGet]
        [ActionName("GetTradesToClear")]
        public HttpResponseMessage GetTradesToClear(HttpRequestMessage Request)
        {
            try
            {
                HttpResponseMessage resp = Request.CreateResponse(HttpStatusCode.OK);
                Logger.DoLog(string.Format("Incoming GetTradesToClear"), fwk.Common.enums.MessageType.Information);

                GetResponse gResp = new GetResponse()
                {
                    Success=true,
                    data = OnGetTradesToClear().ToArray()

                };

                Logger.DoLog("GetTradesToClear successfully processed", fwk.Common.enums.MessageType.Information);
                resp.Content = new StringContent(JsonConvert.SerializeObject(gResp), Encoding.UTF8, "application/json");

                return resp;

            }
            catch (Exception ex)
            {
                string msg = string.Format("Error @GetTradesToClear :{0}", ex.Message);
                Logger.DoLog(msg, fwk.Common.enums.MessageType.Error);
                return CreateGetError(Request, ex.Message);
            }
        }



        #endregion
    }
}
