using fwk.Common.interfaces;
using Newtonsoft.Json;
using Rialto.DataAccessLayer;
using Rialto.KoreConX.Common.DTO.Shareholders;
using Rialto.KoreConX.Common.Util;
using Rialto.KoreConX.ServiceLayer.Client;
using Rialto.LogicLayer.Builders;
using Rialto.Solidus.Common.DTO.Shareholders;
using Rialto.Solidus.Common.Util.Builders;
using Rialto.Solidus.ServiceLayer.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Rialto.LogicLayer
{
    public class ManagementLogic : BaseLayer
    {
        #region Protected Static Conts

        protected static string _KCX_ONBOARDING_APPROVED_OK = "OK";

        protected static string _KCX_ONBOARDING_STARTED_OK = "OK";

        #endregion

        #region Protected Attributes

        protected ShareholderManager ShareholderManager { get; set; }

        protected UserManager UserManager { get; set; }

        protected AESManager AESmanager { get; set; }

        #region KoreConX

        public static PersonsServiceClient PersonsServiceClient { get; set; }

        public ShareholdersServiceClient ShareholdersServiceClient { get; set; }

        #endregion

        #endregion

        #region Constructors

        public ManagementLogic(string pTradingConnectionString, string pOrderConnectionString,
                                string pKCXURL, string pKCXKeyAndIV,string pSolidusURL,ILogger pLogger)
        {
            ShareholderManager = new ShareholderManager(pTradingConnectionString);

            UserManager = new UserManager(pTradingConnectionString);


            #region KCX

            PersonsServiceClient = new PersonsServiceClient(pKCXURL);

            AESmanager = new AESManager(pKCXKeyAndIV);

            #endregion

            #region Solidus

            ShareholdersServiceClient = new Solidus.ServiceLayer.Client.ShareholdersServiceClient(pSolidusURL);

            #endregion

            Logger = pLogger;
        }


        #endregion

        #region Private Methods

        #region KoreConX

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

        private PersonMainInfo UnencryptShareholderPersonalDataFromKCX(string koreShareholderId, string personalData)
        {
            try
            {
                string pdField = AESmanager.DecryptAES(personalData);

                try
                {
                    PersonMainInfo personUnEncr = JsonConvert.DeserializeObject<PersonMainInfo>(pdField);

                    return personUnEncr;

                }
                catch (Exception ex)
                {
                    //TODO- Incorporate logging
                    string msg = string.Format("Critical error unencrypting KXC shareholder PERSONAL info for Kore Shareholder Id {0}: {1}", koreShareholderId, ex.Message);
                    throw new Exception(msg);
                }
            }
            catch (Exception ex)
            {
                //TODO- Incorporate logging
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


        private PersonMainInfo FetchAndDecryptFromKCX(string koreShareholderId, PersonResponse personResp)
        {

            Logger.DoLog(string.Format("@ManagementLogic: Decripting shareholder (firm) for KoreChainId {0} from KoreConX", koreShareholderId), fwk.Common.enums.MessageType.Information);

            PersonMainInfo personUnencr = UnencryptShareholderPersonalDataFromKCX(koreShareholderId, personResp.data.pd);

            Logger.DoLog(string.Format("@ManagementLogic: Building solidus shareholder for KoreChainId {0} from KoreConX", koreShareholderId), fwk.Common.enums.MessageType.Information);

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


        private void ManageShareholderOnExitingKoreShareholderId(string koreShareholderId, Rialto.BusinessEntities.Shareholder shByKoreChain, PersonResponse personResp)
        {
            Logger.DoLog(string.Format("@ManagementLogic: Found shareholder (firm) {0} for KoreChainId {1}", shByKoreChain.GetFullName(), koreShareholderId), fwk.Common.enums.MessageType.Information);

            PersonMainInfo personUnencr = FetchAndDecryptFromKCX(koreShareholderId, personResp);

            Shareholder solidusShareholder = SolidusShareholderBuilder.BuildSolidusShareholderFromKCX(personUnencr);

            Logger.DoLog(string.Format("@ManagementLogic: Retrieving shareholder (firm) form taxId {0} from KoreConX", solidusShareholder.taxIdOrSSNNumber), fwk.Common.enums.MessageType.Information);

            List<Rialto.BusinessEntities.Shareholder> shareholdersByTaxId = ShareholderManager.GetShareholders(null, solidusShareholder.taxIdOrSSNNumber);

            if (shareholdersByTaxId.Count > 0)//Exists KoreChainId AND taxId
            {
                Rialto.BusinessEntities.Shareholder shareholderByTaxId = shareholdersByTaxId.FirstOrDefault();

                shareholderByTaxId.Users= UserManager.GetUsers(shareholderByTaxId.Id);

                Logger.DoLog(string.Format("@ManagementLogic: Found shareholder (firm) {0} for taxId {1} from KoreConX. Running validations", shareholderByTaxId.GetFullName(), solidusShareholder.taxIdOrSSNNumber), fwk.Common.enums.MessageType.Information);

                //2-EXISTS KoreChainId (#1) and shareholderByTaxId
                //We validate that they match!
                ValidateShareholdersOnKoreChainAndTaxId(koreShareholderId, solidusShareholder.taxIdOrSSNNumber, shByKoreChain, shareholdersByTaxId.FirstOrDefault());

                Logger.DoLog(string.Format("@ManagementLogic: Putting shareholder {0} into ONBOARDING state. Invoking Solidus!", shByKoreChain.GetFullName()), fwk.Common.enums.MessageType.Information);
                PersistShareholderAndSendToSolidus(shareholderByTaxId, solidusShareholder);
                Logger.DoLog(string.Format("@ManagementLogic: Shareholder {0} updated and sent to SOLIDUS", shByKoreChain.GetFullName()), fwk.Common.enums.MessageType.Information);


            }
            else//Exists KoreChainId but NOT taxId
            {
                Logger.DoLog(string.Format("@ManagementLogic: NOT Found shareholder (firm) form taxId {0} from KoreConX.", solidusShareholder.taxIdOrSSNNumber), fwk.Common.enums.MessageType.Information);

                //3- EXISTS KoreChainId (#1) but no shareholderByTaxId

                Logger.DoLog(string.Format("@ManagementLogic: Putting shareholder {0} into ONBOARDING state and assigning TaxId {1}. Invoking Solidus", shByKoreChain.Name, solidusShareholder.taxIdOrSSNNumber), fwk.Common.enums.MessageType.Information);
                PersistShareholderAndSendToSolidus(shByKoreChain, solidusShareholder);
                Logger.DoLog(string.Format("@ManagementLogic: Shareholder {0} updated and sent to Solidus", shByKoreChain.GetFullName()), fwk.Common.enums.MessageType.Information);

                
            }
        
        }

        private void ManageShareholdersOnNOTExistingKoreShareholderId(string koreShareholderId, PersonResponse personResp)
        {
            Logger.DoLog(string.Format("@ManagementLogic: NOT Found shareholder (firm) for KoreChainId", koreShareholderId), fwk.Common.enums.MessageType.Information);

            PersonMainInfo personUnencr = FetchAndDecryptFromKCX(koreShareholderId, personResp);
            
            Shareholder solidusShareholder = SolidusShareholderBuilder.BuildSolidusShareholderFromKCX(personUnencr);

            Logger.DoLog(string.Format("@ManagementLogic: Retrieving shareholder (firm) form taxId {0} from KoreConX", solidusShareholder.taxIdOrSSNNumber), fwk.Common.enums.MessageType.Information);

            List<Rialto.BusinessEntities.Shareholder> shareholdersByTaxId = ShareholderManager.GetShareholders(null, solidusShareholder.taxIdOrSSNNumber);

            if (shareholdersByTaxId.Count > 0)
            {
                Rialto.BusinessEntities.Shareholder shareholderByTaxId = shareholdersByTaxId.FirstOrDefault();

                Logger.DoLog(string.Format("@ManagementLogic: Found shareholder (firm) {0} for taxId {1} from KoreConX.", shareholderByTaxId.GetFullName(), solidusShareholder.taxIdOrSSNNumber), fwk.Common.enums.MessageType.Information);

                //4-NOT EXISTS by KoreChainId but EXISTS by TaxId
                Logger.DoLog(string.Format("@ManagementLogic: Putting shareholder {0} into ONBOARDING state. Assigning KoreChainId.. Invoking Solidus.", shareholderByTaxId.GetFullName()), fwk.Common.enums.MessageType.Information);
                PersistShareholderAndSendToSolidus(shareholderByTaxId, solidusShareholder);
                Logger.DoLog(string.Format("@ManagementLogic: Shareholder {0} successfully updated and sent to SOLIDUS.", shareholderByTaxId.GetFullName()), fwk.Common.enums.MessageType.Information);

            }
            else
            {
                Logger.DoLog(string.Format("@ManagementLogic: NOT Found shareholder (firm) '{0}' for taxId {1} from KoreConX. Creating new firms and users ", solidusShareholder.GetFullName(), solidusShareholder.taxIdOrSSNNumber), fwk.Common.enums.MessageType.Information);

                //5-NOT EXISTS by KoreChainId and NOT by TaxId
                Rialto.BusinessEntities.Shareholder rialtoShareholder = new BusinessEntities.Shareholder();
                SolidusToRialtoBuilder.BuildSolidusToRialtoShareholder(koreShareholderId, solidusShareholder,ref rialtoShareholder);
                PersistShareholderAndSendToSolidus(rialtoShareholder, solidusShareholder);
                Logger.DoLog(string.Format("@ManagementLogic: Shareholder {0} successfully created and sent to SOLIDUS.", solidusShareholder.GetFullName()), fwk.Common.enums.MessageType.Information);
            }
        }

        private void PersistShareholderAndSendToSolidus(Rialto.BusinessEntities.Shareholder shareholder, Shareholder solidusShareholder)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    shareholder.OnboardinStatus = Rialto.BusinessEntities.Shareholder._STATUS_ONBOARDING;

                    shareholder.Id = ShareholderManager.PersistShareholder(shareholder);

                    Rialto.BusinessEntities.User usrToPersit = shareholder.GetUser(shareholder.Email);

                    if (usrToPersit == null)
                        usrToPersit = SolidusToRialtoBuilder.BuildUserFromSolidus(solidusShareholder);

                    usrToPersit.Id = UserManager.PersistUser(shareholder.Id, usrToPersit);

                    ShareholderManager.PersistKCXKoreShareholderId(shareholder.Id, shareholder.KoreConXShareholderId.KoreShareholderId);

                    PersonsServiceClient.RequestPerson("");
                }
                catch (Exception ex)
                {
                    string msg = string.Format("Critical error persistirng shareholder for KoreChainId {0}. The shareholder will not be sent to Solidus. Error:{1}", shareholder.KoreConXShareholderId.KoreShareholderId, ex.Message);
                    throw new Exception(msg);                
                }

                try
                {
                    //TODO: t-INVOKE SOLIDUS!!
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

        public string OnKCXOnboardingApproved( string koreShareholderId)
        {
            try
            {
                Logger.DoLog(string.Format("@ManagementLogic: Received onboarding request for KoreChainId {0}", koreShareholderId), fwk.Common.enums.MessageType.Information);

                //1- Download user from KCX
                Logger.DoLog(string.Format("@ManagementLogic: Retrieving shareholder (firm) for KoreChainId {0} from KoreConX", koreShareholderId), fwk.Common.enums.MessageType.Information);

                PersonResponse personResp = PersonsServiceClient.RequestPerson(koreShareholderId);

                if (personResp != null && personResp.data != null)
                {
                    if (personResp.data.pd != null)
                    {

                        Logger.DoLog(string.Format("@ManagementLogic: Looking for shareholders (firms) for KoreChainId {0}", koreShareholderId), fwk.Common.enums.MessageType.Information);

                        List<Rialto.BusinessEntities.Shareholder> shareholdersByKoreChain = ShareholderManager.GetShareholders(koreShareholderId, null);

                        ValidateFoundShareholdersByKoreShareholderId(koreShareholderId, shareholdersByKoreChain);

                        if (shareholdersByKoreChain.Count > 0)
                        {
                            Rialto.BusinessEntities.Shareholder shByKoreChain = shareholdersByKoreChain.FirstOrDefault();

                            shByKoreChain.Users = UserManager.GetUsers(shByKoreChain.Id);

                            ManageShareholderOnExitingKoreShareholderId(koreShareholderId, shByKoreChain, personResp);
                        }
                        else
                        {
                            ManageShareholdersOnNOTExistingKoreShareholderId(koreShareholderId, personResp);
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

                Logger.DoLog(string.Format("@ManagementLogic- ERROR: {0}", ex.Message), fwk.Common.enums.MessageType.Error);
                throw;
            }
        }

        public string OnKCXOnboardingStarted(string koreShareholderId)
        {
            try
            {
                //1-Validate that the KoreShareholderId has an onboarding started  
                Logger.DoLog(string.Format("Requesting personal data for Kore Shareholder Id {0}", koreShareholderId), fwk.Common.enums.MessageType.Information);
                PersonResponse personResp = PersonsServiceClient.RequestPerson(koreShareholderId);

                //Now we have to send this to Solidus --> Decrypt?

                //TODO: Upd. user at kcx_onboarding_status

                return _KCX_ONBOARDING_STARTED_OK;

            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Critical error requesting personal information of Kore Share Holder {0}:{1}", koreShareholderId, ex.Message));
            }
        }
        #endregion
    }
}
