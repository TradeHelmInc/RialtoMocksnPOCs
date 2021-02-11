using fwk.Common.interfaces;
using fwk.Common.util.encryption.RSA;
using Newtonsoft.Json;
using Rialto.DataAccessLayer;
using Rialto.KoreConX.Common.DTO.Shareholders;
using Rialto.KoreConX.Common.Util;
using Rialto.KoreConX.ServiceLayer.Client;
using Rialto.LogicLayer.Builders;
using Rialto.Solidus.Common.Util.Builders;
using Rialto.Solidus.ServiceLayer.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using fwk.Common.enums;
using fwk.common.util.encryption.RSA;
using fwk.common.util.serialization;
using Rialto.BusinessEntities;
using Rialto.BusinessEntities.KoreConX;
using Rialto.BusinessEntities.Plaid;
using Rialto.Common.DTO.Services;
using Rialto.DataAccessLayer.KoreConX;
using Rialto.DataAccessLayer.Plaid;
using Rialto.KoreConX.Common.DTO.Generic;
using Rialto.LogicLayer.Vendors;
using Rialto.Plaid.Common.DTO.Generic.Auth;
using Rialto.Plaid.Common.DTO.Generic.Balance;
using Rialto.Plaid.ServiceLayer.Client;
using Shareholder = Rialto.Solidus.Common.DTO.Shareholders.Shareholder;
using User = Rialto.BusinessEntities.User;

namespace Rialto.LogicLayer
{
    public class ManagementLogic : BaseLayer
    {
        #region Protected Static Conts

        protected static string _KCX_ONBOARDING_APPROVED_OK = "OK";

        protected static string _KCX_ONBOARDING_STARTED_OK = "OK";
        
        protected static string _SOLIDUS_APPLICATION_APPROVAL_OK = "OK";
        
        #endregion

        #region Protected Attributes

        protected  string AppName { get; set; }
        
        protected ShareholderManager ShareholderManager { get; set; }

        protected UserManager UserManager { get; set; }

        protected AccountManager AccountManager { get; set; }
        
        protected TransferAgentManager TransferAgentManager { get; set; }
        
        protected KCXConnectionSettingManager KCXConnectionSettingManager { get; set; }
        
        protected AESManager AESmanager { get; set; }
        
        #region RSA Encryption

        protected RSAEncryption RSAEncryption { get; set; }
        
        protected string RSAPrivateKeyPath { get; set; }
        
        protected bool RSAPrivateKeyEncrypted { get; set; }
        
        #endregion
        
        protected string KCXPublicKeyPath { get; set; }
        
        protected string RSAPublicKey { get; set; }

        protected bool AESKeyEncrypted { get; set; }

        #region KoreConX

        public static PersonsServiceClient PersonsServiceClient { get; set; }

        public ShareholdersServiceClient ShareholdersServiceClient { get; set; }

        #endregion
        
        #region Plaid
        
        protected PlaidLogic PlaidLogic { get; set; }
        
        #endregion

        #endregion

        #region Constructors

        public ManagementLogic(string pAppName, string pTradingConnectionString, string pOrderConnectionString,
                                string pKCXName, string pKCXKeyAndIV, bool pAESKeyEncrypted,string pRASPrivateKeyPath, bool pRSAKeyEncrypted, string pSolidusURL, ILogger pLogger)
        {

            AppName = pAppName;

            AuditLogic = new AuditLogic(pAppName,pTradingConnectionString);
            
            ShareholderManager = new ShareholderManager(pTradingConnectionString);

            UserManager = new UserManager(pTradingConnectionString);

            AccountManager = new AccountManager(pTradingConnectionString);

            TransferAgentManager = new TransferAgentManager(pTradingConnectionString);

            #region KCX

            LoadKoreConXNodeClient(pTradingConnectionString, pKCXName);

            AESmanager = new AESManager(pKCXKeyAndIV, pAESKeyEncrypted);

            RSAEncryption = new RSAEncryption(pRASPrivateKeyPath, pRSAKeyEncrypted);

            AESKeyEncrypted = pAESKeyEncrypted;

            #endregion
            
            #region Solidus

            ShareholdersServiceClient = new Solidus.ServiceLayer.Client.ShareholdersServiceClient(pSolidusURL);

            #endregion

            Logger = pLogger;
        }
        
        public ManagementLogic(string pAppName,string pTradingConnectionString, string pOrderConnectionString,
            string pKCXName, string pKCXPublicKeyPath,string pRSAPrivateKeyPath, bool pRSAKeyEncrypted, string pSolidusURL, ILogger pLogger)
        {

            AppName = pAppName;

            AuditLogic = new AuditLogic(AppName, pTradingConnectionString);
            
            ShareholderManager = new ShareholderManager(pTradingConnectionString);

            UserManager = new UserManager(pTradingConnectionString);

            AccountManager = new AccountManager(pTradingConnectionString);

            TransferAgentManager = new TransferAgentManager(pTradingConnectionString);

            #region KCX

            LoadKoreConXNodeClient(pTradingConnectionString, pKCXName);

            RSAEncryption = new RSAEncryption(pRSAPrivateKeyPath, pRSAKeyEncrypted);

            RSAPrivateKeyPath = pRSAPrivateKeyPath;

            RSAPrivateKeyEncrypted = pRSAKeyEncrypted;

            KCXPublicKeyPath = pKCXPublicKeyPath;

            #endregion

            #region Solidus

            ShareholdersServiceClient = new Solidus.ServiceLayer.Client.ShareholdersServiceClient(pSolidusURL);

            #endregion

            Logger = pLogger;
        }
        
        public ManagementLogic(string pAppName,string pTradingConnectionString, string pOrderConnectionString,string pRSAPublicKey, bool pPlaidTestEnv, ILogger pLogger)
        {
            AppName = pAppName;
            
            AuditLogic = new AuditLogic(pAppName,pTradingConnectionString);
            
            ShareholderManager = new ShareholderManager(pTradingConnectionString);

            UserManager = new UserManager(pTradingConnectionString);

            AccountManager = new AccountManager(pTradingConnectionString);
            
            TransferAgentManager = new TransferAgentManager(pTradingConnectionString);
            
            RSAEncryption=new  RSAEncryption();

            RSAPublicKey = pRSAPublicKey;

            Logger = pLogger;

            PlaidLogic = new PlaidLogic(pAppName,pTradingConnectionString, pLogger, pPlaidTestEnv);
   
        }
        
        public ManagementLogic(string pAppName,string pTradingConnectionString, string pOrderConnectionString, ILogger pLogger)
        {
            AppName = pAppName;
            
            AuditLogic = new AuditLogic(pAppName,pTradingConnectionString);
            
            ShareholderManager = new ShareholderManager(pTradingConnectionString);
            
            AuditLogic = new AuditLogic(pAppName,pTradingConnectionString);

            UserManager = new UserManager(pTradingConnectionString);

            AccountManager = new AccountManager(pTradingConnectionString);
            
            TransferAgentManager = new TransferAgentManager(pTradingConnectionString);

            Logger = pLogger;
   
        }

        #endregion

        #region Private Methods
        
        #region Plaid
      

        #endregion

        #region KoreConX

        private void LoadKoreConXNodeClient(string pTradingConnectionString, string pKCXName)
        {
            KCXConnectionSettingManager = new KCXConnectionSettingManager(pTradingConnectionString);

            TransferAgent kcxAgent = TransferAgentManager.GetTransferAgent(pKCXName);

            if (kcxAgent == null)
                throw new Exception(string.Format("Could not find transfer agent row <table transfer_agents> for Kore Con X with name {0}", pKCXName));

            KoreConXConnectionSetting kcxSetting = KCXConnectionSettingManager.GetKCXConnectionSettings(kcxAgent.Id);
            
            if(kcxSetting==null)
                throw new Exception(string.Format("Could not find transfer agent settomgs <table kcx_connection_setting> for Kore Con X with name {0}", pKCXName));

            PersonsServiceClient = new PersonsServiceClient(kcxSetting.URL, kcxSetting.User, kcxSetting.Password);
        }

        private string GetATSKoreChainId(TransferAgent ta)
        {
            KoreConXConnectionSetting kcxSetting = KCXConnectionSettingManager.GetKCXConnectionSettings(ta.Id);
            
            if(kcxSetting==null)
                throw new Exception(string.Format("Could not find transfer agent settomgs <table kcx_connection_setting> for Kore Con X with Id {0}", ta.Id));

            return kcxSetting.ATSId;
        }

        private void ProcessKCXShareholderError(string koreShareholderId,PersonResponse personResp)
        {
            //TODO- Incorporate logging
            if (personResp == null)
            {
                string msg = string.Format("Critical error recovering KXC shareholder info for Kore Shareholder Id {0}: Null response", koreShareholderId);
                throw new Exception(msg);
            }
            else if (personResp.message == null && personResp.Resp == null)
            {
                string msg = string.Format("Critical error recovering KXC shareholder info for Kore Shareholder Id {0}: Null error message", koreShareholderId);
                throw new Exception(msg);

            }
            else if (personResp.message == null && personResp.Resp == null)
            {
                string msg = string.Format("Critical error recovering KXC shareholder info for Kore Shareholder Id {0}: Null error message", koreShareholderId);
                throw new Exception(msg);
            }
            else if (personResp.message == null && personResp.Resp != null)
            {

                string msg = string.Format("Critical error recovering KXC shareholder info for Kore Shareholder Id {0}: {1}", koreShareholderId, personResp.Resp);
                throw new Exception(msg);
            }
            else if (personResp.message != null)
            {
                if (personResp.message.message != null && personResp.message.message.msg != null)
                {
                    string errorMsg = string.Format("{0}-{1}", personResp.message.message.code, personResp.message.message.msg);
                    string msg = string.Format("Critical error recovering KXC shareholder info for Kore Shareholder Id {0}: {1}", koreShareholderId, errorMsg);
                    throw new Exception(msg);
                }
                else
                {
                    string msg = string.Format("Critical error recovering KXC shareholder info for Kore Shareholder Id {0}: {1}", koreShareholderId, "Unknown inner error message");
                    throw new Exception(msg);
                }
            }
            else
            {

                string msg = string.Format("Critical error recovering KXC shareholder info for Kore Shareholder Id {0}: {1}", koreShareholderId, "Unknown error message");
                throw new Exception(msg);

            }
        }

        private PersonMainInfo UnencryptShareholderPersonalDataFromKCX(string koreShareholderId, string personalData, string key, string IV)
        {
            try
            {
                string pdField = "";
                if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(IV))
                {

                    pdField = AESmanager.DecryptAES(personalData);

                }
                else//we have IV and Key --> we have to decrypt them using the RSA private key
                {
                    string decrKey = null;
                    string decrIV = null;

                    if (AESKeyEncrypted)
                    {
                        decrKey = RSAEncryption.Decrypt(key);
                        decrIV = RSAEncryption.Decrypt(IV);
                    }
                    else
                    {
                        decrKey = key;
                        decrIV = IV;
                    }

                    AESManager AESmanager = new AESManager(decrKey + decrIV, false);
                    pdField = AESmanager.DecryptAES(personalData);
                }

                try
                {

                    PersonMainInfo personUnEncr = JsonConvert.DeserializeObject<PersonMainInfo>(pdField);

                    return personUnEncr;

                }
                catch (Exception ex)
                {
                    string msg = string.Format("Critical error unencrypting KXC shareholder PERSONAL info for Kore Shareholder Id {0}: {1}", koreShareholderId, ex.Message);
                    throw new Exception(msg);
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("Critical error recovering KXC shareholder PERSONAL info for Kore Shareholder Id {0}: {1}", koreShareholderId, ex.Message);
                throw new Exception(msg);
            }
        
        }

        private void ValidateFoundShareholdersByKoreShareholderId(string koreShareholderId,List<Rialto.BusinessEntities.Shareholder> shareholdersByKoreChain)
        {

            if (shareholdersByKoreChain.Count > 1)//If there are too many shareholders for the same KoreChainId --> ERROR
            {
                string foundShareholders = "";

                shareholdersByKoreChain.ForEach(x => foundShareholders += (foundShareholders.Length == 0) ? x.GetFullName() : "," + x.GetFullName());

                throw new Exception(string.Format("CRITICAL error: The following firms were found with the same kore chain id ({0}):{1}. Please contact administrator", koreShareholderId, foundShareholders));
            }
        
        }

        private PersonMainInfo FetchAndDecryptFromKCX(string koreShareholderId, PersonResponse personResp, string key, string IV)
        {
            DoLog(AuditLogic.NEW_ONBOARDING_DECRYPTING_PD,string.Format("@ManagementLogic: Decripting shareholder (firm) for KoreChainId {0} from KoreConX", koreShareholderId));

            PersonMainInfo personUnencr = UnencryptShareholderPersonalDataFromKCX(koreShareholderId, personResp.data.pd,key,IV);

            DoLog(AuditLogic.NEW_ONBOARDING_DECRYPTED_PD,string.Format("@ManagementLogic: Building Solidus shareholder for KoreChainId {0} from KoreConX", koreShareholderId));

            return personUnencr;
        }

        private void ValidateShareholdersOnKoreChainAndTaxId(string koreShareholderId,string taxIdOrSSNNumber,
                                                            Rialto.BusinessEntities.Shareholder shareholderByKoreChain, Rialto.BusinessEntities.Shareholder shareholderByTaxId)
        {
            if (shareholderByKoreChain != null && shareholderByTaxId != null)
            {
                if (shareholderByKoreChain.Id != shareholderByTaxId.Id)
                {
                    string msg = string.Format("CRITICAL error: Found shareholder with Name '{0}' for KoreChainId {1} and another one with  Name '{2}'for tax Id {3}. This is an incosistent state. Please contact administrator",
                                   shareholderByKoreChain.GetFullName(), koreShareholderId, shareholderByTaxId.GetFullName(), taxIdOrSSNNumber);

                    throw new Exception(msg);
                }
            }
        }

        private void ManageShareholderOnExitingKoreShareholderId(string koreShareholderId, Rialto.BusinessEntities.Shareholder shByKoreChain, PersonResponse personResp, string key, string IV)
        {
            DoLog(AuditLogic.NEW_ONBOARDING_FOUND_SHAREHOLDER_FOR_KORE_CHAIN,string.Format("@ManagementLogic: Found shareholder (firm) {0} for KoreChainId {1}", shByKoreChain.GetFullName(), koreShareholderId),AuditLogic.ID_NAME_KORE_SHAREHOLDER_ID,koreShareholderId);
            
            PersonMainInfo personUnencr = FetchAndDecryptFromKCX(koreShareholderId, personResp,key,IV);

            DoLog(AuditLogic.NEW_ONBOARDING_BUILDING_SHAREHOLDER_FROM_KCX_PD,"Building KCX shareholder with downloaded pd",AuditLogic.ID_NAME_KORE_SHAREHOLDER_ID,koreShareholderId);            
            Shareholder solidusShareholder = SolidusShareholderBuilder.BuildSolidusShareholderFromKCX(personUnencr);
            DoLog(AuditLogic.NEW_ONBOARDING_BUILDED_SHAREHOLDER_FROM_KCX_PD,"KCX shareholder built with downloaded pd",AuditLogic.ID_NAME_KORE_SHAREHOLDER_ID,koreShareholderId);   
            
            DoLog(AuditLogic.NEW_ONBOARDING_FETCH_SHAREHOLDER_BY_TAXID,string.Format("@ManagementLogic: Retrieving shareholder (firm) for taxId {0} from KoreConX", solidusShareholder.taxIdOrSSNNumber),AuditLogic.ID_NAME_KORE_SHAREHOLDER_ID,koreShareholderId);  
            List<Rialto.BusinessEntities.Shareholder> shareholdersByTaxId = ShareholderManager.GetShareholders(null, solidusShareholder.taxIdOrSSNNumber);

            if (shareholdersByTaxId.Count > 0)//Exists KoreChainId AND taxId
            {
                Rialto.BusinessEntities.Shareholder shareholderByTaxId = shareholdersByTaxId.FirstOrDefault();

                shareholderByTaxId.Users= UserManager.GetUsers(shareholderByTaxId.Id);

                shareholderByTaxId.Accounts = AccountManager.GetAccounts(shareholderByTaxId.Id);

                DoLog(AuditLogic.NEW_ONBOARDING_FOUND_SHAREHOLDER_BY_TAXID,string.Format("@ManagementLogic: Found shareholder (firm) {0} for taxId {1} from KoreConX. Running validations", shareholderByTaxId.GetFullName(), solidusShareholder.taxIdOrSSNNumber),AuditLogic.ID_NAME_KORE_SHAREHOLDER_ID,koreShareholderId);  

                //2-EXISTS KoreChainId (#1) and shareholderByTaxId
                //We validate that they match!
                ValidateShareholdersOnKoreChainAndTaxId(koreShareholderId, solidusShareholder.taxIdOrSSNNumber, shByKoreChain, shareholdersByTaxId.FirstOrDefault());

                PersistShareholderAndSendToSolidus(shareholderByTaxId, solidusShareholder);
                
            }
            else//Exists KoreChainId but NOT taxId
            {
                //NEW_ONBOARDING_NOT_FOUND_SHAREHOLDER_BY_TAXID
                DoLog(AuditLogic.NEW_ONBOARDING_NOT_FOUND_SHAREHOLDER_BY_TAXID,string.Format("@ManagementLogic: NOT Found shareholder (firm) form taxId {0} from KoreConX.", solidusShareholder.taxIdOrSSNNumber),AuditLogic.ID_NAME_KORE_SHAREHOLDER_ID,koreShareholderId);

                //3- EXISTS KoreChainId (#1) but no shareholderByTaxId
                PersistShareholderAndSendToSolidus(shByKoreChain, solidusShareholder);
            }
        
        }

        private void ManageShareholdersOnNOTExistingKoreShareholderId(string koreShareholderId, PersonResponse personResp, string key, string IV)
        {
            DoLog(AuditLogic.NEW_ONBOARDING_NOT_FOUND_SHAREHOLDER_BY_KORECHAINID,string.Format("@ManagementLogic: NOT Found shareholder (firm) for KoreChainId", koreShareholderId),AuditLogic.ID_NAME_KORE_SHAREHOLDER_ID,koreShareholderId);

            PersonMainInfo personUnencr = FetchAndDecryptFromKCX(koreShareholderId, personResp,key,IV);
            
            DoLog(AuditLogic.NEW_ONBOARDING_BUILDING_SHAREHOLDER_FROM_KCX_PD,"Building KCX shareholder with downloaded pd",AuditLogic.ID_NAME_KORE_SHAREHOLDER_ID,koreShareholderId);            
            Shareholder solidusShareholder = SolidusShareholderBuilder.BuildSolidusShareholderFromKCX(personUnencr);
            DoLog(AuditLogic.NEW_ONBOARDING_BUILDED_SHAREHOLDER_FROM_KCX_PD,"KCX shareholder built with downloaded pd",AuditLogic.ID_NAME_KORE_SHAREHOLDER_ID,koreShareholderId);   

            DoLog(AuditLogic.NEW_ONBOARDING_FETCH_SHAREHOLDER_BY_TAXID,string.Format("@ManagementLogic: Retrieving shareholder (firm) for taxId {0} from KoreConX", solidusShareholder.taxIdOrSSNNumber),AuditLogic.ID_NAME_KORE_SHAREHOLDER_ID,koreShareholderId);  
            List<Rialto.BusinessEntities.Shareholder> shareholdersByTaxId = ShareholderManager.GetShareholders(null, solidusShareholder.taxIdOrSSNNumber);

            if (shareholdersByTaxId.Count > 0)
            {
                Rialto.BusinessEntities.Shareholder shareholderByTaxId = shareholdersByTaxId.FirstOrDefault();

                shareholderByTaxId.Users = UserManager.GetUsers(shareholderByTaxId.Id);
                shareholderByTaxId.Accounts = AccountManager.GetAccounts(shareholderByTaxId.Id);

                DoLog(AuditLogic.NEW_ONBOARDING_FOUND_SHAREHOLDER_BY_TAXID,string.Format("@ManagementLogic: Found shareholder (firm) {0} for taxId {1} from KoreConX.", shareholderByTaxId.GetFullName(), solidusShareholder.taxIdOrSSNNumber),AuditLogic.ID_NAME_KORE_SHAREHOLDER_ID,koreShareholderId);  

                
                //4-NOT EXISTS by KoreChainId but EXISTS by TaxId
                PersistShareholderAndSendToSolidus(shareholderByTaxId, solidusShareholder);
            }
            else
            {
                DoLog(AuditLogic.NEW_ONBOARDING_NOT_FOUND_BY_KORECHAIN_AND_TAXID,string.Format("@ManagementLogic: NOT Found shareholder (firm) '{0}' for taxId {1} from KoreConX. Creating new firms and users ", solidusShareholder.GetFullName(), solidusShareholder.taxIdOrSSNNumber),AuditLogic.ID_NAME_KORE_SHAREHOLDER_ID,koreShareholderId);

                //5-NOT EXISTS by KoreChainId and NOT by TaxId
                DoLog(AuditLogic.NEW_ONBOARDING_BUILD_SHAREHOLDER_FROM_KCX,string.Format("@ManagementLogic: Building ATS shareholder {0} from Solidus Shareholder for KoreChainId {1} and taxId {2} ", solidusShareholder.GetFullName(),koreShareholderId, solidusShareholder.taxIdOrSSNNumber),AuditLogic.ID_NAME_KORE_SHAREHOLDER_ID,koreShareholderId);
                Rialto.BusinessEntities.Shareholder rialtoShareholder = new BusinessEntities.Shareholder();
                SolidusToRialtoBuilder.BuildSolidusToRialtoShareholder(koreShareholderId, solidusShareholder,ref rialtoShareholder);
                DoLog(AuditLogic.NEW_ONBOARDING_BUILT_SHAREHOLDER_FROM_KCX,string.Format("@ManagementLogic: ATS shareholder {0} from Solidus Shareholder for KoreChainId {1} and taxId {2} successfully built", solidusShareholder.GetFullName(),koreShareholderId, solidusShareholder.taxIdOrSSNNumber),AuditLogic.ID_NAME_KORE_SHAREHOLDER_ID,koreShareholderId);

                PersistShareholderAndSendToSolidus(rialtoShareholder, solidusShareholder);
            }
        }

        private void PersistShareholderAndSendToSolidus(Rialto.BusinessEntities.Shareholder shareholder, Shareholder solidusShareholder)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    DoLog(AuditLogic.NEW_ONBOARDING_UPDATING_ONBOARDING_STATUS,string.Format("@ManagementLogic: Putting shareholder {0} into ONBOARDING state. Invoking Solidus", shareholder.GetFullName()),AuditLogic.ID_NAME_KORE_SHAREHOLDER_ID,shareholder.KoreConXShareholderId.KoreShareholderId);
                    shareholder.OnboardingStatus = Rialto.BusinessEntities.Shareholder._STATUS_ONBOARDING;

                    shareholder.Id = ShareholderManager.PersistShareholder(shareholder);

                    Rialto.BusinessEntities.User usrToPersit = shareholder.GetUser(shareholder.Email);
                    if (usrToPersit == null)
                        usrToPersit = SolidusToRialtoBuilder.BuildUserFromSolidus(solidusShareholder);

                    usrToPersit.Id = UserManager.PersistUser(shareholder.Id, usrToPersit);

                    Rialto.BusinessEntities.Account accToPersit = shareholder.GetAccount(shareholder.Email);
                    if (accToPersit == null)
                        accToPersit = SolidusToRialtoBuilder.BuildAccountFromSolidus(solidusShareholder, shareholder, usrToPersit);

                    accToPersit.Id = AccountManager.PersistAccount(accToPersit);

                    ShareholderManager.PersistKCXKoreShareholderId(shareholder.Id, shareholder.KoreConXShareholderId.KoreShareholderId);
                    DoLog(AuditLogic.NEW_ONBOARDING_UPDATED_ONBOARDING_STATUS,string.Format("@ManagementLogic: Shareholder {0} updated", shareholder.GetFullName()),AuditLogic.ID_NAME_KORE_SHAREHOLDER_ID,shareholder.KoreConXShareholderId.KoreShareholderId);

                }
                catch (Exception ex)
                {
                    string msg = string.Format("Critical error persistirng shareholder for KoreChainId {0}. The shareholder will not be sent to Solidus. Error:{1}", shareholder.KoreConXShareholderId.KoreShareholderId, ex.Message);
                    throw new Exception(msg);                
                }

                try
                {
                    DoLog(AuditLogic.NEW_ONBOARDING_SENDING_TO_SOLIDUS,string.Format("@ManagementLogic:Sending shareholder {0} to Solidus",shareholder.KoreConXShareholderId.KoreShareholderId));
                    //TODO: t-INVOKE SOLIDUS!!
                    DoLog(AuditLogic.NEW_ONBOARDING_SENT_TO_SOLIDUS,string.Format("@ManagementLogic:Shareholder {0} sent to Solidus",shareholder.KoreConXShareholderId.KoreShareholderId));

                }
                catch (Exception ex)
                {
                    string msg = string.Format("Critical error sending shareholder id for KoreChainId {0}:{1}", shareholder.KoreConXShareholderId.KoreShareholderId, ex.Message);
                    throw new Exception(msg);
                }

                scope.Complete();
            }
        
        }


        #endregion

        #endregion

        #region Public Methods

        public string OnKCXOnboardingApproved_4096(string[] param)
        {
            try
            {
                DoLog(AuditLogic.NEW_ONBOARDING_LAUNCHED, "@ManagementLogic: Received onboarding request for KoreChainId with 4096 bits encrypted data");
                if (KCXPublicKeyPath == null)
                    throw new Exception("KoreConX public key not properly loaded!");

                string decryptedToRSA = "";
                foreach (string toDecrypt in param)
                {
                    decryptedToRSA+=RSA4096Encryption.DecryptWithPublic(toDecrypt, KCXPublicKeyPath);
                }
                
                DoLog(AuditLogic.NEW_ONBOARDING_DECR_KCX_PUBLIC_KEY, "Msg decrypted using KCX public key");
                
                DoLog(AuditLogic.NEW_ONBOARDING_DECRYPTING_RIALTO_PRIVATE_KEY, "Msg decrypting using Rialto private key");
                string koreChainAndKeys = RSA4096Encryption.DecryptWithPrivate(decryptedToRSA, RSAPrivateKeyPath);

                string koreChainId = JSONExtractor.GetJSONKey("KoreChainID", koreChainAndKeys);
                string secret_Key = JSONExtractor.GetJSONKey("Secret_Key", koreChainAndKeys);
                string IV = JSONExtractor.GetJSONKey("IV", koreChainAndKeys);
                string koreCompanyId = JSONExtractor.GetJSONKey("KoreCompanyID", koreChainAndKeys);
                //We cannot use NewtonSoft library becuase the secret and IV might contains quotes (") which might make it fail
                //KoreChainAndKeysDTO korechainKeysDTO = JsonConvert.DeserializeObject<KoreChainAndKeysDTO>(koreChainAndKeys)
                //;
                DoLog(AuditLogic.NEW_ONBOARDING_DECRYPTED_RIALTO_PRIVATE_KEY,
                    String.Format("Msg decrypted  using Rialto private key: KoreChainId={0} KoreCompanyId={1}",
                        koreChainId, koreCompanyId),AuditLogic.ID_NAME_KORE_SHAREHOLDER_ID,koreChainId);

                return OnKCXOnboardingApproved(koreChainId, koreCompanyId, secret_Key, IV);
            }
            catch (Exception ex)
            {
                DoLogError(AuditLogic.ONBOARDING_FAILED,ex, string.Format("@ManagementLogic.OnKCXOnboardingApproved_4096- ERROR: {0}", ex.Message));
                throw;
            }

        }

        public string OnKCXOnboardingApproved(string koreShareholderId,string companyKoreChainId, string key, string IV)
        {
            try
            {
                //Logger.DoLog(string.Format("@ManagementLogic: Received onboarding request for KoreChainId {0}", koreShareholderId), fwk.Common.enums.MessageType.Information);

                
                //1- Download user from KCX
                TransferAgent transferAgent = TransferAgentManager.GetTransferAgent(TransferAgentManager._KCX_ID);

                if (transferAgent == null)
                    throw new Exception(string.Format("Could not find Kore Chain Id for ATS. Transfer Agent = {0}",TransferAgentManager._KCX_ID));
                
                DoLog(AuditLogic.NEW_ONBOARDING_FETCHING_SHAREHOLDER_FROM_KCX,string.Format("@ManagementLogic: Retrieving shareholder (firm) for KoreChainId {0} from KoreConX", koreShareholderId),AuditLogic.ID_NAME_KORE_SHAREHOLDER_ID,koreShareholderId);
                PersonResponse personResp = PersonsServiceClient.RequestPerson(koreShareholderId,companyKoreChainId,GetATSKoreChainId(transferAgent));

                if (personResp != null && personResp.data != null)
                {
                    if (personResp.data.pd != null)
                    {
                        DoLog(AuditLogic.NEW_ONBOARDING_RETRIEVING_SHAREHOLDER_IN_ATS,string.Format("@ManagementLogic: Looking for shareholders (firms) for KoreChainId {0} @ATS", koreShareholderId),AuditLogic.ID_NAME_KORE_SHAREHOLDER_ID,koreShareholderId);
                        List<Rialto.BusinessEntities.Shareholder> shareholdersByKoreChain = ShareholderManager.GetShareholders(koreShareholderId, null);

                        ValidateFoundShareholdersByKoreShareholderId(koreShareholderId, shareholdersByKoreChain);

                        if (shareholdersByKoreChain.Count > 0)
                        {
                            Rialto.BusinessEntities.Shareholder shByKoreChain = shareholdersByKoreChain.FirstOrDefault();

                            shByKoreChain.Users = UserManager.GetUsers(shByKoreChain.Id);

                            shByKoreChain.Accounts = AccountManager.GetAccounts(shByKoreChain.Id);

                            ManageShareholderOnExitingKoreShareholderId(koreShareholderId, shByKoreChain, personResp,key,IV);
                        }
                        else
                        {
                            ManageShareholdersOnNOTExistingKoreShareholderId(koreShareholderId, personResp, key, IV);
                        }
                    }
                    else
                    {
                        string msg = string.Format("Critical error recovering KoreConX shareholder PERSONAL info for Kore Shareholder Id {0}: Missing Personal Info", koreShareholderId);
                        throw new Exception(msg);
                    }

                }
                else
                    ProcessKCXShareholderError(koreShareholderId, personResp);

                return _KCX_ONBOARDING_APPROVED_OK;
            }
            catch (Exception ex)
            {
                //Logger.DoLog(string.Format("@ManagementLogic- ERROR: {0}", ex.Message), fwk.Common.enums.MessageType.Error);
                throw;
            }
        }

        public string OnKCXOnboardingStarted(string koreShareholderId,string companyKoreChainId)
        {
            try
            {
                TransferAgent transferAgent = TransferAgentManager.GetTransferAgent(TransferAgentManager._KCX_ID);

                if (transferAgent == null)
                    throw new Exception(string.Format("Could not find Kore Chain Id for ATS. Transfer Agent = {0}",TransferAgentManager._KCX_ID));
                
                //1-Validate that the KoreShareholderId has an onboarding started  
                Logger.DoLog(string.Format("Requesting personal data for Kore Shareholder Id {0}", koreShareholderId), fwk.Common.enums.MessageType.Information);
                PersonResponse personResp = PersonsServiceClient.RequestPerson(koreShareholderId, companyKoreChainId,GetATSKoreChainId(transferAgent));

                //Now we have to send this to Solidus --> Decrypt?

                //TODO: Upd. user at kcx_onboarding_status

                return _KCX_ONBOARDING_STARTED_OK;

            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Critical error requesting personal information of Kore Share Holder {0}:{1}", koreShareholderId, ex.Message));
            }
        }

        public string OnPlaidCredentialsLoad(string userIdentifier, string plaidAccessToken, string plaidItemId)
        {

            try
            {
                DoLog(AuditLogic.PLAID_CREDENTIALS_START, string.Format("Received Plaid Credentials for email {0}",userIdentifier));

                User user = UserManager.GetUser(userIdentifier); //User Identifier has to be the email

                if (user == null)
                    throw new Exception(string.Format("Could not find user for email (user identifier){0}",
                        userIdentifier));

                BusinessEntities.Shareholder sh = ShareholderManager.GetShareholder(user.FirmId); 
                
                if (sh == null)
                    throw new Exception(string.Format("Could not find firm for firmId (user identifier){0}",
                        user.FirmId));

                DoLog(AuditLogic.PLAID_CREDENTIALS_USER_FOUND,
                    string.Format("Found user for email {0}: Name {1} TaxId={2}", userIdentifier, sh.GetFullName(),
                        sh.FirmTaxId),AuditLogic.ID_NAME_TAX_ID, sh.FirmTaxId);
                
                string encAccessToken = RSAEncryption.EncryptToStr(RSAPublicKey, plaidAccessToken);

                PlaidCredential plaidCred = new PlaidCredential()
                {
                    User = user,
                    Shareholder = sh,
                    AccessToken = encAccessToken,
                    UserIdentifier = userIdentifier,
                    PlaidItemId = plaidItemId
                };

                return PlaidLogic.PersistCredentialsAndUpdateBalance(plaidCred);
            }
            catch (Exception ex)
            {
                DoLogError(AuditLogic.PLAID_CREDENTIALS_FAILED,ex, string.Format("@ManagementLogic.OnPlaidCredentialsLoad- ERROR: {0}", ex.Message));
                throw;
            }
        }

        public string OnApplicationApproval(string email)
        {
            try
            {
                Logger.DoLog(string.Format("Approving application for email {0}", email), fwk.Common.enums.MessageType.Information);
                User usr = UserManager.GetUser(email);

                if (usr == null)
                    throw new Exception(string.Format("Could not find user registered with email {0}", email));

                BusinessEntities.Shareholder sh = ShareholderManager.GetShareholder(usr.FirmId);


                if (sh == null)
                    throw new Exception(string.Format("Could not find shareholder (firm) for id {0} (email {1})",
                                        usr.FirmId, email));

                sh.OnboardingStatus = BusinessEntities.Shareholder._STATUS_APP_APPROVED;

                ShareholderManager.PersistShareholder(sh);
                
                Logger.DoLog(string.Format("Application for email {0} sucessfully approved", email), fwk.Common.enums.MessageType.Information);

                return _SOLIDUS_APPLICATION_APPROVAL_OK;
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Critical error on Application Approval from Solidus for email{0}:{1}",e.Message,email));
            }
        }

        #endregion
    }
}
