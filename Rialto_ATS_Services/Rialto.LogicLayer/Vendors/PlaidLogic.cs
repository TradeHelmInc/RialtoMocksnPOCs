using System;
using System.Configuration;
using System.Transactions;
using fwk.Common.enums;
using fwk.Common.interfaces;
using fwk.Common.util.logger;
using Rialto.BusinessEntities.Plaid;
using Rialto.DataAccessLayer;
using Rialto.DataAccessLayer.Plaid;
using Rialto.Plaid.Common.DTO.Generic;
using Rialto.Plaid.Common.DTO.Generic.Auth;
using Rialto.Plaid.Common.DTO.Generic.Balance;
using Rialto.Plaid.ServiceLayer.Client;

namespace Rialto.LogicLayer.Vendors
{
    public class PlaidLogic:BaseLayer
    {
        #region Protected Attributes
        
        protected PlaidCredentialsManager PlaidCredentialsManager { get; set; }
        
        protected PlaidSettingManager PlaidSettingManager { get; set; }
        
        protected ShareholderManager ShareholderManager { get; set; }

        protected UserManager UserManager { get; set; }

        protected bool PlaidTestEnv { get; set; }
        
        protected PlaidSetting Setting { get; set; }
        
        protected BalanceManager BalanceManager { get; set; }
        
        protected TokenManager TokenManager { get; set; }
        
        #endregion
        
        #region Protected Static Consts
        
        protected static string _PLAID_CREDENTIALS_LOADED = "OK";
        
        protected static string _PLAID_CREDENTIALS_LOADED_FAILED_BALANCE = "OK_BUT_BALANCE_FAILED";
        
        protected static string _PLAID_CREDENTIALS_LOADED_BUT_BALANCE_EXCEPTION = "OK_BUT_BALANCE_EXCEPTION";
        
        #endregion
        
        #region Constructors


        public PlaidLogic(string pAppName,string pTradingConnectionString,ILogger pLogger,bool pPlaidTestEnv)
        {
            PlaidCredentialsManager = new PlaidCredentialsManager(pTradingConnectionString);
            PlaidSettingManager = new PlaidSettingManager(pTradingConnectionString);
            ShareholderManager = new ShareholderManager(pTradingConnectionString);
            UserManager = new UserManager(pTradingConnectionString);

            AuditLogic = new AuditLogic(pAppName, pTradingConnectionString);
            
            Logger = pLogger;
            PlaidTestEnv = pPlaidTestEnv;
            LoadPlaidBalanceManager();
        }

        #endregion
        
        #region Private Methods
        
        
        private void LoadPlaidBalanceManager()
        {
            Setting =  PlaidSettingManager.GetEnabledPlaidSetting();

            if (Setting == null)
                throw new Exception("Could not retrieve Plaid settings in plaid_settings table!");
            
            if (Setting.Secret == null)
                throw new Exception("Plaid settings does not have an ATS Secret!");
            
            if (Setting.ClientId == null)
                throw new Exception("Plaid settings does not have an ATS ClientId!");
            
            BalanceManager = new BalanceManager(Setting.URL);

            TokenManager = new TokenManager(Setting.URL);
        }
        
         // The authentication depends on the creation of 3 tokens
        //  A- Link token
        //  B- Public token
        //  C- Access token
        // As the transformation between the Link token and the Public token needs of third party companies (Plaid Link) this methdo implements a 
        // test authentication supposing we are in a test environment
        //More info: https://plaid.com/docs/api/tokens/
        protected AuthReq DoAuthInPlaidTestEnv(string clientId, string secret)
        {
            //1.A - We create a token create request. So, in a test environment , we just create the B-Public Token
            PublicTokenCreateReq tokenCreateReq = new PublicTokenCreateReq()
            {
                client_id =clientId,
                secret = secret

            };

            Logger.DoLog(
                string.Format("Requesting B-Public Token for client_id={0} and secret={1}", tokenCreateReq.client_id,
                    tokenCreateReq.secret), MessageType.Information);
            BaseResponse respCreatePublicToken = TokenManager.CreatePublicToken(tokenCreateReq);

            if (respCreatePublicToken.GenericError == null)
            {
                //1.B -We exchange the public token for the C-Access token 
                PublicTokenCreateResp publTokenResp = (PublicTokenCreateResp) respCreatePublicToken;
                Logger.DoLog(
                    string.Format("Received B-Public Token : {0}", publTokenResp.public_token, tokenCreateReq.secret),
                    MessageType.Information);

                PublicTokenExchangeReq publTokenReq = new PublicTokenExchangeReq()
                {
                    client_id = tokenCreateReq.client_id,
                    secret = tokenCreateReq.secret,
                    public_token = publTokenResp.public_token
                };

                Logger.DoLog(
                    string.Format("Requesting C-Access Token for client_id={0} and secret={1}",
                        tokenCreateReq.client_id, tokenCreateReq.secret), MessageType.Information);
                BaseResponse respExchangeAccessToken = TokenManager.ExchangePublicToken(publTokenReq);

                if (respExchangeAccessToken.GenericError == null)
                {
                    //1.C- Once we have the Access Token, we request the authentication
                    PublicTokenExchangeResp exchTokenResp = (PublicTokenExchangeResp) respExchangeAccessToken;
                    Logger.DoLog(string.Format("Received C-Access Token : {0}", exchTokenResp.access_token),
                        MessageType.Information);

                    AuthReq authReq = new AuthReq()
                    {
                        access_token = exchTokenResp.access_token, client_id = publTokenReq.client_id,
                        secret = publTokenReq.secret
                    };

                    Logger.DoLog(
                        string.Format("Requesting Authentication for client_id={0} and secret={1}",
                            tokenCreateReq.client_id, tokenCreateReq.secret), MessageType.Information);
                    BaseResponse respAuthReq = TokenManager.Authenticate(authReq);

                    if (respAuthReq.GenericError == null)
                    {
                        //One special detail is that the authetication response will have exactly the same structure 
                        //as the Get Balance service. But for clarity reasons we will first authenticate and then request the balance again
                        AuthResp authReqResp = (AuthResp) respAuthReq;
                        Logger.DoLog(
                            string.Format("Client authenticated. Found {0} accounts",
                                authReqResp.accounts != null ? authReqResp.accounts.Length : 0),
                            MessageType.Information);

                        return authReq;
                    }
                    else
                    {
                        Logger.DoLog(
                            string.Format("Error requesting authentication:{0}",
                                respAuthReq.GenericError.display_message), MessageType.Error);
                        return null;
                    }
                }
                else
                {
                    Logger.DoLog(
                        string.Format("Error requesting for C-Access Token:{0}",
                            respExchangeAccessToken.GenericError.display_message), MessageType.Error);
                    return null;
                }
            }
            else
            {
                Logger.DoLog(
                    string.Format("Error requesting for B-Public Token:{0}",
                        respCreatePublicToken.GenericError.display_message), MessageType.Error);
                return null;
            }

        }
        
        #endregion
        
        #region Private Methods

        private void DoUpadateBalance(PlaidCredential cred,GetBalanceResp getBalanceResp)
        {
            DoLog(AuditLogic.PLAID_CREDENTIALS_PERSISTING_BALANCE,string.Format("Persisting balance for Tax Id {0}",cred.Shareholder.FirmTaxId), AuditLogic.ID_NAME_TAX_ID,cred.Shareholder.FirmTaxId);

            double availableBalance = getBalanceResp.GetBalanceForTrading();
            cred.Shareholder.FirmLimit = availableBalance;
            cred.User.BuyingPower = availableBalance;
            cred.User.UsedLimit = 0;
            cred.User.TradeLimit = availableBalance;
            cred.User.CashOnHand = availableBalance;

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                UserManager.PersistUser(cred.Shareholder.Id,cred.User);
                ShareholderManager.PersistShareholder(cred.Shareholder);
                
                scope.Complete();
            }
            DoLog(AuditLogic.PLAID_CREDENTIALS_PERSISTED_BALANCE,string.Format("Persisted balance for Tax Id {0}",cred.Shareholder.FirmTaxId), AuditLogic.ID_NAME_TAX_ID,cred.Shareholder.FirmTaxId);
        }

        private string UpdatePlaidBalance(PlaidCredential cred)
        {
            try
            {
                GetBalanceReq getBalanceReq = new GetBalanceReq()
                {
                    access_token = cred.AccessToken,
                    secret = Setting.Secret,
                    client_id = Setting.ClientId
                };
                
                if (PlaidTestEnv && Setting.SecretAndClientIdLoaded())//if we are in a test environment , we have to use the test env credentials
                {
                    DoLog(AuditLogic.PLAID_CREDENTIALS_AUTH_TEST_ENV,string.Format("Auth Plaid Credentials in test env for TaxId={0}",cred.Shareholder.FirmTaxId), AuditLogic.ID_NAME_TAX_ID,cred.Shareholder.FirmTaxId);
                    AuthReq authReq = DoAuthInPlaidTestEnv(Setting.ClientId, Setting.Secret);
                    getBalanceReq.access_token = authReq.access_token;
                    DoLog(AuditLogic.PLAID_CREDENTIALS_AUTHENTICATED,string.Format("Plaid Credentials Authenticated for TaxId={0}",cred.Shareholder.FirmTaxId), AuditLogic.ID_NAME_TAX_ID,cred.Shareholder.FirmTaxId);
                }

                if (!string.IsNullOrEmpty(Setting.Secret) && !string.IsNullOrEmpty(Setting.ClientId) && !string.IsNullOrEmpty(cred.AccessToken))
                {
                    DoLog(AuditLogic.PLAID_CREDENTIALS_REQUESTING_BALANCE,string.Format("Requesting balances for access token {0}", getBalanceReq.access_token), AuditLogic.ID_NAME_TAX_ID,cred.Shareholder.FirmTaxId);
                    BaseResponse respGetBalanceReq =BalanceManager.GetBalance(getBalanceReq);

                    if (respGetBalanceReq.GenericError == null)
                    {

                        GetBalanceResp getBalanceResp = (GetBalanceResp) respGetBalanceReq;
                        DoLog(AuditLogic.PLAID_CREDENTIALS_FOUND_ACCOUNTS,string.Format("Found {0} accounts",getBalanceResp.accounts != null ? getBalanceResp.accounts.Length : 0), AuditLogic.ID_NAME_TAX_ID,cred.Shareholder.FirmTaxId);

                        DoUpadateBalance(cred, getBalanceResp);
                        
                        return _PLAID_CREDENTIALS_LOADED;
                    }
                    else
                    {
                        Exception ex = new Exception(string.Format("Error requesting balances:{0}",
                            respGetBalanceReq.GenericError.display_message));
                        DoLogError(AuditLogic.PLAID_CREDENTIALS_FAILED,ex,ex.Message );
                        return _PLAID_CREDENTIALS_LOADED_FAILED_BALANCE;
                    }
                }
            }
            catch (Exception ex)
            {
                Exception ex2 =
                    new Exception(string.Format("Error requesting balances for populated token:{0}", ex.Message));
                DoLogError(AuditLogic.PLAID_CREDENTIALS_FAILED,ex2,ex2.Message );
                return _PLAID_CREDENTIALS_LOADED_BUT_BALANCE_EXCEPTION;
            }
            
            return _PLAID_CREDENTIALS_LOADED;
        }
        
        #endregion
        
        #region Public Methods

        public string PersistCredentialsAndUpdateBalance(PlaidCredential plaidCred)
        {

            DoLog(AuditLogic.PLAID_CREDENTIALS_PERSISTING,
                string.Format("Persisting Plaid Credentials for TaxId={0}",
                    plaidCred.Shareholder.FirmTaxId), AuditLogic.ID_NAME_TAX_ID,
                plaidCred.Shareholder.FirmTaxId);

            
            PlaidCredentialsManager.PersistPlaidCredentials(plaidCred);
            
            DoLog(AuditLogic.PLAID_CREDENTIALS_PERSISTED,
                string.Format("Plaid Credentials Persisted for TaxId={0}",
                    plaidCred.Shareholder.FirmTaxId), AuditLogic.ID_NAME_TAX_ID,
                plaidCred.Shareholder.FirmTaxId);

            return UpdatePlaidBalance(plaidCred);
        }

        #endregion
        
    }
}