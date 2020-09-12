using fwk.ServiceLayer.REST;
using KoreConX.Common.DTO.Generic;
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
    public delegate ValidationResponse OnAvailableShares(string koreShareholderId,string koreSecurityId, int qty, string koreATSId);

    public class holdingsController : BaseREST
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


        [HttpGet]
        [ActionName("release-shares")]
        public HttpResponseMessage ReleaseShares(HttpRequestMessage Request)
        {
            try
            {
                HttpResponseMessage resp = Request.CreateResponse(HttpStatusCode.OK);
                resp.Content = new StringContent(JsonConvert.SerializeObject(""), Encoding.UTF8, "application/json");

                return resp;

            }
            catch (Exception ex)
            {
                return CreateErrorResponse(Request, ex.Message);
            }


        }


        [HttpGet]
        [ActionName("hold-shares")]
        public HttpResponseMessage HoldShares(HttpRequestMessage Request)
        {
            try
            {
                HttpResponseMessage resp = Request.CreateResponse(HttpStatusCode.OK);
                resp.Content = new StringContent(JsonConvert.SerializeObject(""), Encoding.UTF8, "application/json");

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
