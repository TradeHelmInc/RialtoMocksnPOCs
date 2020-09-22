using fwk.ServiceLayer.REST;
using Newtonsoft.Json;
using Rialto.KoreConX.Common.DTO.Generic;
using Rialto.KoreConX.Common.DTO.Holdings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.KoreConX.ServiceLayer.Client
{
    public class HoldingsServiceClient : KCXBaseRESTClient
    {
        #region Constructors

        public HoldingsServiceClient(string pBaseURL)
        {

            BaseURL = pBaseURL;
        
        }

        #endregion

        #region Protected Static Consts

        protected static string _AVAILABLE_SHARES = "/holdings/available-shares";

        protected static string _HOLD_SHARES = "/holdings/hold-shares";

        protected static string _RELEASE_SHARES = "/holdings/release-shares";

        #endregion

        #region Private Methods

        protected TransactionResponse ProcessTransactionResponse(BaseResponse resp)
        {
            if (resp.Resp != null)
            {
                try
                {
                    TransactionResponse tranResp = JsonConvert.DeserializeObject<TransactionResponse>(resp.Resp);
                    return tranResp;
                }
                catch (Exception ex)
                {
                    return new TransactionResponse() { Resp = resp.Resp, GenericError = GetGenericError(ex.Message) };

                }
            }
            else
                return new TransactionResponse() { Resp = resp.Resp, GenericError = resp.GenericError };
        
        }

        protected ValidationResponse ProcessValidationResponse(BaseGetResponse resp)
        {
            if (resp.Resp != null)
            {
                try
                {
                    ValidationResponse valResp = JsonConvert.DeserializeObject<ValidationResponse>(resp.Resp);
                    return valResp;
                }
                catch (Exception ex)
                {
                    return new ValidationResponse() { Resp = resp.Resp, message = resp.message };

                }
            }
            else
                return new ValidationResponse() { Resp = resp.Resp, message = resp.message };

        }

        #endregion

        #region Public Methods

        public ValidationResponse AvailableShares(string securities_holder_id, string koresecurities_id, int number_of_shares,
                                                    string requestor_id)
        {
            string url = BaseURL + _AVAILABLE_SHARES;


            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("securities_holder_id", securities_holder_id);
            param.Add("koresecurities_id", koresecurities_id);
            param.Add("number_of_shares", number_of_shares.ToString());
            param.Add("requestor_id", requestor_id);


            return ProcessValidationResponse(DoGetJson(url, param));
        }

        public TransactionResponse PutHoldOnShares(HoldSharesDTO dto)
        {
            string url = BaseURL + _HOLD_SHARES;

            string output = JsonConvert.SerializeObject(dto);

            BaseResponse resp = DoPostJson(url, new Dictionary<string, string>(), output);

            return ProcessTransactionResponse(resp);

        }

        public TransactionResponse ReleaseShares(ReleaseSharesDTO dto)
        {
            string url = BaseURL + _RELEASE_SHARES;

            string output = JsonConvert.SerializeObject(dto);

            BaseResponse resp = DoPostJson(url, new Dictionary<string, string>(), output);

            return ProcessTransactionResponse(resp);
        }

        #endregion
    }
}
