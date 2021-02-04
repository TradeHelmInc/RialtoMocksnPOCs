using Newtonsoft.Json;
using Rialto.Common.DTO.Generic;
using Rialto.Common.DTO.Services.Solidus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Rialto.ServiceLayer.Client
{
    public class OnApplicationApprovalClient : RialtoBaseRESTClient
    {
         #region Constructors

        public OnApplicationApprovalClient(string pBaseURL)
        {

            BaseURL = pBaseURL;
        
        }

        #endregion

        #region Protected Static Consts

        protected static string _ON_APPLICATION_APPROVAL = "/Management/ApproveApplication";

        #endregion

        #region Public Methods

        public Rialto.Common.DTO.Generic.TransactionResponse OnApplicationApproval(OnApplicationApprovalDTO dto)
        {
            try
            {
                string url = BaseURL + _ON_APPLICATION_APPROVAL ;

                Dictionary<string, string> param = new Dictionary<string, string>();

                string output = JsonConvert.SerializeObject(dto);

                Rialto.Common.DTO.Generic.TransactionResponse resp = DoPostJson(url, new Dictionary<string, string>(), output);

                return resp;
            }
            catch (Exception ex)
            {
                return new Rialto.Common.DTO.Generic.TransactionResponse() { Success = false, Error = new Rialto.Common.DTO.Generic.ErrorMessage() { code = 1, msg = ex.Message } };
            }
        }

        #endregion
    }
}
