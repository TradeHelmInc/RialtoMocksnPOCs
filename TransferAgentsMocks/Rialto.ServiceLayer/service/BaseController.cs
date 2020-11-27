using fwk.ServiceLayer.REST;
using Rialto.Common.DTO.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.ServiceLayer.service
{
    public class BaseController : BaseRESTServer
    {
        #region Protected Methods

        public HttpResponseMessage CreateTransactionError(HttpRequestMessage Request, string msg)
        {

            return Request.CreateResponse(HttpStatusCode.InternalServerError,
                                            new TransactionResponse()
                                            {


                                                Success = false,
                                                Error = new ErrorMessage { code = 500, msg = msg }

                                            }
                );
        }

        public HttpResponseMessage CreateGetError(HttpRequestMessage Request, string msg)
        {

            return Request.CreateResponse(HttpStatusCode.InternalServerError,
                                          new GetResponse()
                                          {


                                              Success = false,
                                              Error = new ErrorMessage { code = 500, msg = msg }

                                          }
                );
        }



        #endregion
    }
}
