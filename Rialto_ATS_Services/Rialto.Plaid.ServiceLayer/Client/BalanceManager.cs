using Newtonsoft.Json;
using Rialto.Plaid.Common.DTO.Generic;
using Rialto.Plaid.Common.DTO.Generic.Balance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Plaid.ServiceLayer.Client
{
    public class BalanceManager: PlaidBaseRESTClient
    {
         #region Constructors

        public BalanceManager(string pBaseURL)
        {

            BaseURL = pBaseURL;
        
        }

        #endregion

        #region Private Consts


        #endregion

        #region Protected Static Consts

        protected static string _GET_BALANCE = "/accounts/balance/get";

        #endregion

        #region Public Methods


        public BaseResponse GetBalance(GetBalanceReq getBalanceReq)
        {
            string url = BaseURL + _GET_BALANCE;

            string output = JsonConvert.SerializeObject(getBalanceReq);
            BaseResponse resp = DoPostJson(url, new Dictionary<string, string>(), output);

            if (resp.GenericError == null)
            {
                try
                {
                    GetBalanceResp getBalanceResp = JsonConvert.DeserializeObject<GetBalanceResp>(resp.Resp);
                    return getBalanceResp;

                }
                catch (Exception ex)
                {
                    return new BaseResponse { Resp = resp.Resp, GenericError = new GenericError() { display_message = ex.Message, error_message = ex.Message } };
                }
            }
            else
                return resp;
        
        }

        #endregion
    }
}
