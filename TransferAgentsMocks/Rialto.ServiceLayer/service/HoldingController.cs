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
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Could not process input json:{0}", ex.Message));
                }

                string txtId = OnSell(onSellDTO.SellShareholderId, onSellDTO.SecurityId, onSellDTO.OrderQty);

                TransactionResponse txResp = new TransactionResponse() { Success = true, Id = new IdEntity() { id = txtId } };

                resp.Content = new StringContent(JsonConvert.SerializeObject(txResp), Encoding.UTF8, "application/json");

                return resp;
            }
            catch (Exception ex)
            {
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
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Could not process input json:{0}", ex.Message));
                }

                string txtId = OnBuy(onBuyDTO.BuyShareholderId, onBuyDTO.SecurityId, onBuyDTO.OrderQty);

                TransactionResponse txResp = new TransactionResponse() { Success = true, Id = new IdEntity() { id = txtId } };

                resp.Content = new StringContent(JsonConvert.SerializeObject(txResp), Encoding.UTF8, "application/json");

                return resp;
            }
            catch (Exception ex)
            {
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
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Could not process input json:{0}", ex.Message));
                }

                string txtId = OnOrderCancelledOrExpired(onOrderCxlOrExpDTO.SellOrderId, onOrderCxlOrExpDTO.ReleaseQty);

                TransactionResponse txResp = new TransactionResponse() { Success = true, Id = new IdEntity() { id = txtId } };

                resp.Content = new StringContent(JsonConvert.SerializeObject(txResp), Encoding.UTF8, "application/json");

                return resp;
            }
            catch (Exception ex)
            {
                return CreateTransactionError(Request, ex.Message);
            }
        }

        #endregion
    }
}
