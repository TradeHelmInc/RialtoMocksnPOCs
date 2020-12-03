using fwk.Common.interfaces;
using Rialto.DataAccessLayer;
using Rialto.KoreConX.Common.DTO.Shareholders;
using Rialto.KoreConX.ServiceLayer.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        #region KoreConX

        public static PersonsServiceClient PersonsServiceClient { get; set; }

        #endregion

        #endregion

        #region Constructors

        public ManagementLogic(string pTradingConnectionString, string pOrderConnectionString, string pKCXURL, ILogger pLogger)
        {
            ShareholderManager = new ShareholderManager(pTradingConnectionString);

            PersonsServiceClient = new PersonsServiceClient(pKCXURL);

            Logger = pLogger;
        }


        #endregion

        #region Public Methods

        public string OnKCXOnboardingApproved(string SSN, string koreShareholderId)
        { 
            //1- Recuperar el usuario por SSN
            //2- NEW TABLE kcx_onboarding_status -->  


            return _KCX_ONBOARDING_APPROVED_OK;
        
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
