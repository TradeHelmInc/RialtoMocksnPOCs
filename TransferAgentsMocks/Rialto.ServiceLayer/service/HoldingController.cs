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
    public delegate string OnSell(int sellShareholderId, int securityId, double orderQuantity);

    public delegate string OnBuy(int buyShareholderId, int securityId, double orderQuantity);

    public delegate string OnOrderCancelledOrExpired(int sellOrderId, double releaseQty);


    public class HoldingController : BaseController
    {
        #region Public Static Attributs

        public static event OnSell OnSell;

        public static event OnBuy OnBuy;

        public static event OnOrderCancelledOrExpired OnOrderCancelledOrExpired;

        public static ILogger Logger { get; set; }

        #endregion

        #region Public Methods

        [HttpPost]
        [ActionName("OnSell")]
        public HttpResponseMessage OnSellSvc(HttpRequestMessage Request)
        {
            try
            {
                var content = Request.Content;
                string jsonInput = content.ReadAsStringAsync().Result;
                HttpResponseMessage resp = Request.CreateResponse(HttpStatusCode.OK);

               
                OnSellDTO onSellDTO = null;
                try
                {
                    onSellDTO = JsonConvert.DeserializeObject<OnSellDTO>(jsonInput);
                    Logger.DoLog(string.Format("Incoming OnSell: SellShareholderId={0} SecurityId={1} OrderQty={2}", onSellDTO.SellShareholderId, onSellDTO.SecurityId, onSellDTO.OrderQty), fwk.Common.enums.MessageType.Information);

                }
                catch (Exception ex)
                {
                    string msg = string.Format("Could not process input json {1}:{0}", ex.Message, jsonInput);
                    Logger.DoLog(msg, fwk.Common.enums.MessageType.Error);
                    throw new Exception(msg);
                }

                string txtId = OnSell(onSellDTO.SellShareholderId, onSellDTO.SecurityId, onSellDTO.OrderQty);

                Logger.DoLog("OnSell successfully processed", fwk.Common.enums.MessageType.Information);

                TransactionResponse txResp = new TransactionResponse() { Success = true, Id = new IdEntity() { id = txtId } };

                resp.Content = new StringContent(JsonConvert.SerializeObject(txResp), Encoding.UTF8, "application/json");

                return resp;
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error @OnSell :{0}", ex.Message);
                Logger.DoLog(msg, fwk.Common.enums.MessageType.Error);
                return CreateTransactionError(Request, ex.Message);
            }
        }

        [HttpPost]
        [ActionName("OnBuy")]
        public HttpResponseMessage OnBuySvc(HttpRequestMessage Request)
        {
            try
            {
                var content = Request.Content;
                string jsonInput = content.ReadAsStringAsync().Result;
                HttpResponseMessage resp = Request.CreateResponse(HttpStatusCode.OK);


                OnBuyDTO onBuyDTO = null;
                try
                {
                    onBuyDTO = JsonConvert.DeserializeObject<OnBuyDTO>(jsonInput);
                    Logger.DoLog(string.Format("Incoming OnBuy: BuyShareholderId={0} SecurityId={1} OrderQty={2}", onBuyDTO.BuyShareholderId, onBuyDTO.SecurityId, onBuyDTO.OrderQty), fwk.Common.enums.MessageType.Information);

                }
                catch (Exception ex)
                {
                    string msg = string.Format("Could not process input json {1}:{0}", ex.Message, jsonInput);
                    Logger.DoLog(msg, fwk.Common.enums.MessageType.Error);
                    throw new Exception(msg);
                }

                string txtId = OnBuy(onBuyDTO.BuyShareholderId, onBuyDTO.SecurityId, onBuyDTO.OrderQty);

                Logger.DoLog("OnBuy successfully processed", fwk.Common.enums.MessageType.Information);

                TransactionResponse txResp = new TransactionResponse() { Success = true, Id = new IdEntity() { id = txtId } };

                resp.Content = new StringContent(JsonConvert.SerializeObject(txResp), Encoding.UTF8, "application/json");

                return resp;
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error @OnBuy :{0}", ex.Message);
                Logger.DoLog(msg, fwk.Common.enums.MessageType.Error);
                return CreateTransactionError(Request, ex.Message);
            }
        }

        [HttpPost]
        [ActionName("OnOrderCancelledOrExpiredSvc")]
        public HttpResponseMessage OnOrderCancelledOrExpiredSvc(HttpRequestMessage Request)
        {
            try
            {
                var content = Request.Content;
                string jsonInput = content.ReadAsStringAsync().Result;
                HttpResponseMessage resp = Request.CreateResponse(HttpStatusCode.OK);


                OnOrderCancelledOrExpiredDTO onOrderCxlOrExpDTO = null;
                try
                {
                    onOrderCxlOrExpDTO = JsonConvert.DeserializeObject<OnOrderCancelledOrExpiredDTO>(jsonInput);
                    Logger.DoLog(string.Format("Incoming OnOrderCancelledOrExpiredSvc: SellOrderId={0} ReleaseQty={1} ", onOrderCxlOrExpDTO.SellOrderId, onOrderCxlOrExpDTO.ReleaseQty), fwk.Common.enums.MessageType.Information);

                }
                catch (Exception ex)
                {
                    string msg = string.Format("Could not process input json {1}:{0}", ex.Message, jsonInput);
                    Logger.DoLog(msg, fwk.Common.enums.MessageType.Error);
                    throw new Exception(msg);
                }

                string txtId = OnOrderCancelledOrExpired(onOrderCxlOrExpDTO.SellOrderId, onOrderCxlOrExpDTO.ReleaseQty);

                Logger.DoLog("OnOrderCancelledOrExpired successfully processed", fwk.Common.enums.MessageType.Information);

                TransactionResponse txResp = new TransactionResponse() { Success = true, Id = new IdEntity() { id = txtId } };

                resp.Content = new StringContent(JsonConvert.SerializeObject(txResp), Encoding.UTF8, "application/json");

                return resp;
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error @OnOrderCancelledOrExpiredSvc :{0}", ex.Message);
                Logger.DoLog(msg, fwk.Common.enums.MessageType.Error);
                return CreateTransactionError(Request, ex.Message);
            }
        }

        #endregion
    }
}
