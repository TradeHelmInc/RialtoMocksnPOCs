using fwk.ServiceLayer.REST;
using KoreConX.Common.DTO.Generic;
using Mocks.KoreConX.Common.DTO.Generic;
using Mocks.KoreConX.Common.DTO.Securities;
using Mocks.KoreConX.Common.DTO.Shareholders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace KoreConX.ServiceLayer.service
{
    public delegate TransactionResponse OnTransferShares(TransferSharesDTO transferSharesDto);

    public delegate DataResponse OnShareholderRequest(ShareholderSharesDTO shareholdersReqDto);

    public class securitiesController : BaseRESTServer
    {
        #region Private Methods

        public HttpResponseMessage CreateKoreConXError(HttpRequestMessage Request, string msg)
        {

            return Request.CreateResponse(HttpStatusCode.InternalServerError,
                                            new
                                            {
                                                message = new ErrorMessage() { code = 500, msg = msg }

                                            }
                );
        }

        #endregion


        #region Public Static Attributs

        public static event OnTransferShares OnTransferShares;

        public static event OnShareholderRequest OnShareholderRequest;

        #endregion

        #region Public Methods

        [HttpPost]
        [ActionName("transfer")]
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


                resp.Content = new StringContent(JsonConvert.SerializeObject(OnTransferShares(transferDto)), Encoding.UTF8, "application/json");

                return resp;

            }
            catch (Exception ex)
            {
                return CreateErrorResponse(Request, ex.Message);
            }

        }

        [HttpGet]
        [ActionName("shareholders")]
        public HttpResponseMessage ShareholdersRequest(HttpRequestMessage Request, string company_id, string koresecurities_id, string requestor_id)
        {
            try
            {
                HttpResponseMessage resp = Request.CreateResponse(HttpStatusCode.OK);

                ShareholderSharesDTO shareholdersSharesDTO = new ShareholderSharesDTO() { company_id = company_id, koresecurities_id = koresecurities_id, requestor_id = requestor_id };

                resp.Content = new StringContent(JsonConvert.SerializeObject(OnShareholderRequest(shareholdersSharesDTO)), Encoding.UTF8, "application/json");

                return resp;

            }
            catch (Exception ex)
            {
                return CreateKoreConXError(Request, ex.Message);
            }
        }



        #endregion

    }
}
