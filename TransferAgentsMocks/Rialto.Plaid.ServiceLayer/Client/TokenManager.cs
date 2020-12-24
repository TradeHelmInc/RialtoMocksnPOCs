using Newtonsoft.Json;
using Rialto.Plaid.Common.DTO.Generic;
using Rialto.Plaid.Common.DTO.Generic.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Plaid.ServiceLayer.Client
{
    public class TokenManager : PlaidBaseRESTClient
    {
        #region Constructors

        public TokenManager(string pBaseURL)
        {

            BaseURL = pBaseURL;
        
        }

        #endregion

        #region Private Consts

        protected static string _TEST_INSTITUTION = "ins_1";

        protected static string _TEST_USER = "user_good";

        protected static string _TEST_PASSWORD = "pass_good";

        protected static string _AUTH_PRODUCT="auth";

        protected static string _TEST_WEBHOOK_URL = "https://www.genericwebhookurl.com/webhook";

        #endregion

        #region Protected Static Consts

        protected static string _CREATE_PUBLIC_TOKEN = "/sandbox/public_token/create";//this is only available in testing environments

        protected static string _EXCHANGE_PUBLIC_TOKEN = "/item/public_token/exchange";

        protected static string _AUTH = "/auth/get";

        #endregion

        #region Public Methods

        public BaseResponse CreatePublicToken(PublicTokenCreateReq tokenCreateReq)
        {
            string url = BaseURL + _CREATE_PUBLIC_TOKEN;

            tokenCreateReq.institution_id = _TEST_INSTITUTION;
            tokenCreateReq.options = new PublicTokenCreateOptionsReq() { override_username = _TEST_USER, override_password = _TEST_PASSWORD, webhook = _TEST_WEBHOOK_URL };
            tokenCreateReq.initial_products = new string[] { _AUTH_PRODUCT };

            string output = JsonConvert.SerializeObject(tokenCreateReq);
            BaseResponse resp =  DoPostJson(url,new Dictionary<string, string>(), output);


            if (resp.GenericError == null)
            {
                try
                {
                    PublicTokenCreateResp publTokenResp = JsonConvert.DeserializeObject<PublicTokenCreateResp>(resp.Resp);
                    return publTokenResp;

                }
                catch (Exception ex)
                {
                    return new BaseResponse { Resp = resp.Resp, GenericError = new GenericError() { display_message = ex.Message, error_message = ex.Message } };
                }
            }
            else
                return resp;
        }

        public BaseResponse ExchangePublicToken(PublicTokenExchangeReq tokenExchangeReq)
        {
            string url = BaseURL + _EXCHANGE_PUBLIC_TOKEN;

            string output = JsonConvert.SerializeObject(tokenExchangeReq);
            BaseResponse resp = DoPostJson(url, new Dictionary<string, string>(), output);

            if (resp.GenericError == null)
            {
                try
                {
                    PublicTokenExchangeResp accessTokenResp = JsonConvert.DeserializeObject<PublicTokenExchangeResp>(resp.Resp);
                    return accessTokenResp;

                }
                catch (Exception ex)
                {
                    return new BaseResponse { Resp = resp.Resp, GenericError = new GenericError() { display_message = ex.Message, error_message = ex.Message } };
                }
            }
            else
                return resp;
        }


        public BaseResponse Authenticate(AuthReq authReq)
        {
            string url = BaseURL + _AUTH;

            string output = JsonConvert.SerializeObject(authReq);
            BaseResponse resp = DoPostJson(url, new Dictionary<string, string>(), output);

            if (resp.GenericError == null)
            {
                try
                {
                    AuthResp accessTokenResp = JsonConvert.DeserializeObject<AuthResp>(resp.Resp);
                    return accessTokenResp;

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
