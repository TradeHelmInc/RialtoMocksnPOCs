using fwk.ServiceLayer.REST;
using KoreConX.Common.DTO.Generic;
using Mocks.KoreConX.Common.DTO.Generic;
using Mocks.KoreConX.Common.DTO.Holdings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Mocks.KoreConX.ServiceLayer.service
{
    public delegate ValidationResponse OnAvailableShares(string koreShareholderId,string koreSecurityId, int qty, string koreATSId);

    public delegate TransactionResponse OnHoldShares(HoldSharesDTO holdDto);

    public delegate TransactionResponse OnReleaseShares(ReleaseSharesDTO holdDto);


    public class holdingsController : BaseRESTServer
    {

        #region Private Methods

        public HttpResponseMessage CreateKoreConXError(HttpRequestMessage Request, string msg)
        {

            return Request.CreateResponse(HttpStatusCode.InternalServerError, 
                                            new {
                                                    message = new ErrorMessage() { code = 500, msg = msg }
            
                                                }
                );
        }

        #endregion


        #region Public Static Attributs

        public static event OnAvailableShares OnAvailableShares;

        public static event OnHoldShares OnHoldShares;

        public static event OnReleaseShares OnReleaseShares;

        #endregion

        #region Public Methods
        [HttpGet]
        [ActionName("available-shares")]
        public HttpResponseMessage AvailableShares(HttpRequestMessage Request, string securities_holder_id, string koresecurities_id, int number_of_shares,
                                                    string requestor_id)
        {
            try
            {
                HttpResponseMessage resp = Request.CreateResponse(HttpStatusCode.OK);
                resp.Content = new StringContent(JsonConvert.SerializeObject(OnAvailableShares(securities_holder_id, koresecurities_id, number_of_shares, requestor_id))
                                                , Encoding.UTF8, "application/json");

                return resp;

            }
            catch (Exception ex)
            {
                return CreateKoreConXError(Request, ex.Message);
            }
        }


        [HttpPost]
        [ActionName("release-shares")]
        public HttpResponseMessage ReleaseShares(HttpRequestMessage Request)
        {

            try
            {
                var content = Request.Content;
                string jsonInput = content.ReadAsStringAsync().Result;
                HttpResponseMessage resp = Request.CreateResponse(HttpStatusCode.OK);


                ReleaseSharesDTO releaseDto = null;
                try
                {
                    releaseDto = JsonConvert.DeserializeObject<ReleaseSharesDTO>(jsonInput);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Could not process input json:{0}", ex.Message));
                }


                resp.Content = new StringContent(JsonConvert.SerializeObject(OnReleaseShares(releaseDto)), Encoding.UTF8, "application/json");

                return resp;

            }
            catch (Exception ex)
            {
                return CreateErrorResponse(Request, ex.Message);
            }

        }


        [HttpPost]
        [ActionName("hold-shares")]
        public HttpResponseMessage HoldShares(HttpRequestMessage Request)
        {
            try
            {
                var content = Request.Content;
                string jsonInput = content.ReadAsStringAsync().Result;
                HttpResponseMessage resp = Request.CreateResponse(HttpStatusCode.OK);


                HoldSharesDTO holdDto=null;
                try
                {
                    holdDto = JsonConvert.DeserializeObject<HoldSharesDTO>(jsonInput);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Could not process input json:{0}", ex.Message));
                }


                resp.Content = new StringContent(JsonConvert.SerializeObject(OnHoldShares(holdDto)), Encoding.UTF8, "application/json");

                return resp;

            }
            catch (Exception ex)
            {
                return CreateErrorResponse(Request, ex.Message);
            }
        }

        #endregion



    }
}
