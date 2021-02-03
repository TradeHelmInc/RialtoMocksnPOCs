using Newtonsoft.Json;
using Rialto.Rialto.Common.DTO.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Rialto.ServiceLayer.Client
{
    public class GetTradesServiceClient : RialtoBaseRESTClient
    {
        #region Constructors

        public GetTradesServiceClient(string pBaseURL, string pUser, string pPassword)
        {

            BaseURL = pBaseURL;

            User = pUser;

            Password = pPassword;

        }

        #endregion

        #region Protected Static Consts

        protected static string _GET_TRADES = "/Trading/GetTradesToClear/";

        #endregion

        #region Public Methods

        public GetResponse GetTrades()
        {
            try
            {
                string url = BaseURL + _GET_TRADES;

                Dictionary<string, string> param = new Dictionary<string, string>();

                string resp = DoGetJson(url, param);

                return JsonConvert.DeserializeObject<GetResponse>(resp);
            }
            catch (Exception ex)
            {
                return new GetResponse() { Success = false, Error = new ErrorMessage() { code = 1, msg = ex.Message } };
            }
        }

        #endregion
    }
}
