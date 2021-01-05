using Newtonsoft.Json;
using Rialto.KoreConX.Common.DTO.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.KoreConX.ServiceLayer.Client
{
    public class SecuritiesServiceClient : KCXBaseRESTClient
    {
        #region Constructors

        public SecuritiesServiceClient(string pBaseURL)
        {

            BaseURL = pBaseURL;
        
        }

        #endregion

        #region Protected Static Consts

        protected static string _SHAREHOLDERS_REQUEST = "/securities/shareholders";

        #endregion

        #region Protected Methods

        protected DataResponse ProcessDataResponse(BaseGetResponse resp)
        {
            if (resp.Resp != null)
            {
                try
                {
                    DataResponse dataResp = JsonConvert.DeserializeObject<DataResponse>(resp.Resp);
                    return dataResp;
                }
                catch (Exception ex)
                {
                    return new DataResponse() { Resp = resp.Resp, message = resp.message };

                }
            }
            else
                return new DataResponse() { Resp = resp.Resp, message = resp.message };

        }

        #endregion

        #region Public Methods

        public DataResponse RequestShareholders(string company_id, string koresecurities_id, string requestor_id)
        {
            string url = BaseURL + _SHAREHOLDERS_REQUEST;

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("company_id", company_id);
            param.Add("koresecurities_id", koresecurities_id);
            param.Add("requestor_id", requestor_id);

            return ProcessDataResponse(DoGetJson(url, param));
        }

        #endregion
    }
}
