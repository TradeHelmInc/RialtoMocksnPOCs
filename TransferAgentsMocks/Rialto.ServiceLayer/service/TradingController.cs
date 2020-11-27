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
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Could not process input json:{0}", ex.Message));
                }

                string txtId = OnTransferShares(transferDto.BuyShareholderId, transferDto.SellShareholderId, transferDto.TradeQuantity, transferDto.SecurityId, transferDto.SellOrderId);

                TransactionResponse txResp = new TransactionResponse() { Success = true, Id = new IdEntity() { id = txtId } };

                resp.Content = new StringContent(JsonConvert.SerializeObject(txResp), Encoding.UTF8, "application/json");

                return resp;
            }
            catch (Exception ex)
            {
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
                GetResponse gResp = new GetResponse()
                {
                    Success=true,
                    data = OnGetTradesToClear().ToArray()

                };
                resp.Content = new StringContent(JsonConvert.SerializeObject(gResp), Encoding.UTF8, "application/json");

                return resp;

            }
            catch (Exception ex)
            {
                return CreateGetError(Request, ex.Message);
            }
        }



        #endregion
    }
}
