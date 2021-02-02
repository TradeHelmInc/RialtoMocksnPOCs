using Newtonsoft.Json;
using Rialto.Common.DTO.Services.Plaid;
using Rialto.Rialto.Common.DTO.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Rialto.ServiceLayer.Client
{
    public class OnPlaidCredentialsLoadClient : RialtoBaseRESTClient
    {
        #region Constructors

        public OnPlaidCredentialsLoadClient(string pBaseURL)
        {

            BaseURL = pBaseURL;
        
        }

        #endregion

        #region Protected Static Consts

        protected static string _ON_PLAID_CREDENTIALS_LOAD = "/Plaid/OnPlaidCredentialsLoad";

        #endregion

        #region Public Methods

        public TransactionResponse OnPlaidCredentialsLoad(OnPlaidCredentialsLoadDTO dto)
        {
            try
            {
                string url = BaseURL + _ON_PLAID_CREDENTIALS_LOAD;

                Dictionary<string, string> param = new Dictionary<string, string>();

                string output = JsonConvert.SerializeObject(dto);

                TransactionResponse resp = DoPostJson(url, new Dictionary<string, string>(), output);

                return resp;
            }
            catch (Exception ex)
            {
                return new TransactionResponse() { Success = false, Error = new ErrorMessage() { code = 1, msg = ex.Message } };
            }
        }

        #endregion
    }
}
