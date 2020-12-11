using Newtonsoft.Json;
using Rialto.Solidus.Common.DTO.Shareholders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Solidus.ServiceLayer.Client
{
    public class ShareholdersServiceClient : SolidusBaseRESTClient
    {
        #region Constructors

        public ShareholdersServiceClient(string pBaseURL)
        {

            BaseURL = pBaseURL;
        
        }

        #endregion

        #region Protected Static Consts

        protected static string _CREATE_APPLICATION = "/onboarding/create-application";

        #endregion

        #region Public Methods
        public void CreateApplication(Shareholder dto)
        {
            string url = BaseURL + _CREATE_APPLICATION;

            string output = JsonConvert.SerializeObject(dto);

            //BaseResponse resp = DoPostJson(url, new Dictionary<string, string>(), output);

            DoPostJson(url, new Dictionary<string, string>(), output);

            //return ProcessTransactionResponse(resp);
        }

        #endregion
    }
}
