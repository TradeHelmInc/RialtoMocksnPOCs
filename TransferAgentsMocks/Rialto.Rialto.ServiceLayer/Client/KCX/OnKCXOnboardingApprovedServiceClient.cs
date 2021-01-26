using Newtonsoft.Json;
using Rialto.Common.DTO.Services;
using Rialto.Common.DTO.Services.KCX;
using Rialto.Rialto.Common.DTO.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Rialto.ServiceLayer.Client.KCX
{
    public class OnKCXOnboardingApprovedServiceClient : RialtoBaseRESTClient
    {
        #region Constructors

        public OnKCXOnboardingApprovedServiceClient(string pBaseURL)
        {

            BaseURL = pBaseURL;
        
        }

        #endregion

        #region Protected Static Consts

        protected static string _ON_ONBOARDING_APPROVED = "/Management/OnKCXOnboardingApproved/";

        protected static string _ON_ONBOARDING_APPROVED_4096 = "/Management/OnKCXOnboardingApproved_4096/";

        #endregion

        #region Public Methods

        public TransactionResponse OnKCXOnboardingApproved_4096(string part1, string part2)
        {
            try
            {
                string url = BaseURL + _ON_ONBOARDING_APPROVED_4096;

                Dictionary<string, string> param = new Dictionary<string, string>();

                OnKCXOnboardingApproved4096DTO dto = new OnKCXOnboardingApproved4096DTO() { Params = new string[] { part1, part2 } };

                string output = JsonConvert.SerializeObject(dto);

                TransactionResponse resp = DoPostJson(url, new Dictionary<string, string>(), output);

                return resp;
            }
            catch (Exception ex)
            {
                return new TransactionResponse() { Success = false, Error = new ErrorMessage() { code = 1, msg = ex.Message } };
            }
        }

        public TransactionResponse OnKCXOnboardingApproved(string koreShareholderId, string companyKoreChainId, string key = null, string iv = null)
        {
            try
            {
                string url = BaseURL + _ON_ONBOARDING_APPROVED;

                Dictionary<string, string> param = new Dictionary<string, string>();

                OnKCXOnboardingApprovedDTO dto = new OnKCXOnboardingApprovedDTO() { KoreShareholderId = koreShareholderId, CompanyKoreChainId = companyKoreChainId, Key = key, IV = iv };

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
