using Newtonsoft.Json;
using Rialto.Rialto.Common.DTO.Generic;
using Rialto.Rialto.Common.DTO.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Rialto.ServiceLayer.Client
{
    public class OnSellServiceClient : RialtoBaseRESTClient
    {
        #region Constructors

        public OnSellServiceClient(string pBaseURL)
        {

            BaseURL = pBaseURL;
        
        }

        #endregion

        #region Protected Static Consts

        protected static string _ON_SELL = "/Holding/OnSell/";

        #endregion

        #region Public Methods

        public TransactionResponse OnSell(OnSellDTO dto)
        {
            try
            {
                string url = BaseURL + _ON_SELL;

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
