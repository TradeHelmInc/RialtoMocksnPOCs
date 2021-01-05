using fwk.Common.enums;
using Rialto.Plaid.Common.DTO.Generic;
using Rialto.Plaid.Common.DTO.Generic.Auth;
using Rialto.Plaid.Common.DTO.Generic.Balance;
using Rialto.Plaid.ServiceLayer.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PlaidValidateBalance
{
    class Program
    {

        #region Protected Attributes

        protected static TokenManager TokenManager { get; set; }

        protected static BalanceManager BalanceManager { get; set; }

        #endregion


        #region Private Static Methods

        private static void DoLog(string msg, MessageType type)
        {
            Console.WriteLine("");
            if (type == MessageType.Error)
                Console.ForegroundColor = ConsoleColor.Red;

            if (type == MessageType.Debug)
                Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine(msg);

            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("");
        }

        #endregion

        #region Protected Static Methods

        //1- This methods implements the full authentication as if we were connected to a prod o development environment (see DoTestAuthenticate)
        protected static void DoProdAuthenticate()
        {
            //TODO: To be implemented
        
        }

        //2- After authenticating (there is no need to keep a token in memory and there should be one authentication per order created)
        //   we have to requests the balances. We will use this information to validate that there is enough money to send an order
        protected static GetBalanceResp GetBalance(AuthReq authReq)
        {

            GetBalanceReq getBalanceReq = new GetBalanceReq()
            {
                access_token = authReq.access_token,
                secret = authReq.secret,
                client_id = authReq.client_id
            };

            DoLog(string.Format("Requesting balances for access token {0}", getBalanceReq.access_token), MessageType.Information);
            BaseResponse respGetBalanceReq = BalanceManager.GetBalance(getBalanceReq);

            if (respGetBalanceReq.GenericError == null)
            {

                GetBalanceResp getBalanceResp = (GetBalanceResp)respGetBalanceReq;
                DoLog(string.Format("Found {0} accounts", getBalanceResp.accounts != null ? getBalanceResp.accounts.Length : 0), MessageType.Information);

                return getBalanceResp;
            }
            else
            {
                DoLog(string.Format("Error requesting balances:{0}", respGetBalanceReq.GenericError.display_message), MessageType.Error);
                return null;
            }
        }


        //3- Validate Balance
        // The algorithm is prretty simple, until told otherwise, we will only consider those accounts with TYPE=depository and SUB_TYPE=savings
        protected static void ValidateValance(GetBalanceResp getBalanceResp)
        {
            DoLog(string.Format("The plaid account has an available balance of {0} {1}", getBalanceResp.GetBalanceForTrading().ToString("0.00"), getBalanceResp.GetBalanceCurrency()), MessageType.Information);

            DoLog(string.Format("If the order notional (Price x Size) is bigger than that , it should be rejected. Otherwise it should be allowed"), MessageType.Information);
        }

        // The authentication depends on the creation of 3 tokens
        //  A- Link token
        //  B- Public token
        //  C- Access token
        // As the transformation between the Link token and the Public token needs of third party companies (Plaid Link) this methdo implements a 
        // test authentication supposing we are in a test environment
        //More info: https://plaid.com/docs/api/tokens/
        protected static AuthReq DoTestAuthenticate()
        {
            //1.A - We create a token create request. So, in a test environment , we just create the B-Public Token
            PublicTokenCreateReq tokenCreateReq = new PublicTokenCreateReq()
            {
                client_id = ConfigurationManager.AppSettings["client_id"],//Each firm will have its plaid info that will have its client_id, secret, and we will see if some other fields too
                secret = ConfigurationManager.AppSettings["secret"]

            };

            DoLog(string.Format("Requesting B-Public Token for client_id={0} and secret={1}", tokenCreateReq.client_id, tokenCreateReq.secret), MessageType.Information);
            BaseResponse respCreatePublicToken = TokenManager.CreatePublicToken(tokenCreateReq);

            if (respCreatePublicToken.GenericError == null)
            {
                //1.B -We exchange the public token for the C-Access token 
                PublicTokenCreateResp publTokenResp = (PublicTokenCreateResp)respCreatePublicToken;
                DoLog(string.Format("Received B-Public Token : {0}", publTokenResp.public_token, tokenCreateReq.secret), MessageType.Information);

                PublicTokenExchangeReq publTokenReq = new PublicTokenExchangeReq()
                {
                    client_id = tokenCreateReq.client_id,
                    secret = tokenCreateReq.secret,
                    public_token = publTokenResp.public_token
                };

                DoLog(string.Format("Requesting C-Access Token for client_id={0} and secret={1}", tokenCreateReq.client_id, tokenCreateReq.secret), MessageType.Information);
                BaseResponse respExchangeAccessToken = TokenManager.ExchangePublicToken(publTokenReq);

                if (respExchangeAccessToken.GenericError == null)
                {
                    //1.C- Once we have the Access Token, we request the authentication
                    PublicTokenExchangeResp exchTokenResp = (PublicTokenExchangeResp)respExchangeAccessToken;
                    DoLog(string.Format("Received C-Access Token : {0}", exchTokenResp.access_token), MessageType.Information);

                    AuthReq authReq = new AuthReq() { access_token = exchTokenResp.access_token, client_id = publTokenReq.client_id, secret = publTokenReq.secret };

                    DoLog(string.Format("Requesting Authentication for client_id={0} and secret={1}", tokenCreateReq.client_id, tokenCreateReq.secret), MessageType.Information);
                    BaseResponse respAuthReq = TokenManager.Authenticate(authReq);

                    if (respAuthReq.GenericError == null)
                    {
                        //One special detail is that the authetication response will have exactly the same structure 
                        //as the Get Balance service. But for clarity reasons we will first authenticate and then request the balance again
                        AuthResp authReqResp = (AuthResp)respAuthReq;
                        DoLog(string.Format("Client authenticated. Found {0} accounts", authReqResp.accounts != null ? authReqResp.accounts.Length : 0), MessageType.Information);

                        return authReq;
                    }
                    else
                    {
                        DoLog(string.Format("Error requesting authentication:{0}", respAuthReq.GenericError.display_message), MessageType.Error);
                        return null;
                    }
                }
                else
                {
                    DoLog(string.Format("Error requesting for C-Access Token:{0}", respExchangeAccessToken.GenericError.display_message), MessageType.Error);
                    return null;
                }
            }
            else
            {
                DoLog(string.Format("Error requesting for B-Public Token:{0}", respCreatePublicToken.GenericError.display_message), MessageType.Error);
                return null;
            }

        
        }

        #endregion

        static void Main(string[] args)
        {

            string url = ConfigurationManager.AppSettings["url"];

            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12
                                                  | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;


            TokenManager = new TokenManager(url);
            BalanceManager = new BalanceManager(url);


            try
            {
                AuthReq authReq = DoTestAuthenticate();

                GetBalanceResp getBalanceResp = GetBalance(authReq);

                ValidateValance(getBalanceResp);
            }
            catch (Exception ex)
            {
                DoLog(string.Format("Critical Error:{0}", ex.Message), MessageType.Error);
            }

            Console.ReadKey();
        }
    }
}
