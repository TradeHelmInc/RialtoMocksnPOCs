using fwk.Common.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
//using System.Web.Http;

namespace fwk.ServiceLayer.REST
{
    public delegate void OnLog(string msg, MessageType type);

    /*public class BaseRESTServer : ApiController
    {
        #region Public Static Attributs

        public static event OnLog OnLog;

        #endregion

        #region Protected Methods

        protected static HttpResponseMessage CreateErrorResponse(HttpRequestMessage Request, string error)
        {
            return Request.CreateResponse(HttpStatusCode.InternalServerError,
                                                           new
                                                           {
                                                               IsOK = false,
                                                               Error = error,
                                                           });

        }

       


        #endregion

        #region Private Methods

       

        #endregion
    }*/
}
