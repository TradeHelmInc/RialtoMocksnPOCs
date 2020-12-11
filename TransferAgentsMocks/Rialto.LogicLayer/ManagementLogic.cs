using fwk.Common.interfaces;
using Newtonsoft.Json;
using Rialto.DataAccessLayer;
using Rialto.KoreConX.Common.DTO.Shareholders;
using Rialto.KoreConX.Common.Util;
using Rialto.KoreConX.ServiceLayer.Client;
using Rialto.Solidus.Common.DTO.Shareholders;
using Rialto.Solidus.Common.Util.Builders;
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

        protected AESManager AESmanager { get; set; }

        #region KoreConX

        public static PersonsServiceClient PersonsServiceClient { get; set; }

        #endregion

        #endregion

        #region Constructors

        public ManagementLogic(string pTradingConnectionString, string pOrderConnectionString,
                                string pKCXURL, string pKCXKeyAndIV,ILogger pLogger)
        {
            ShareholderManager = new ShareholderManager(pTradingConnectionString);


            #region KCX

            PersonsServiceClient = new PersonsServiceClient(pKCXURL);

            AESmanager = new AESManager(pKCXKeyAndIV);

            #endregion

            Logger = pLogger;
        }


        #endregion

        #region Private Methods

        private void ProcessShareholderError(string koreShareholderId,PersonResponse personResp)
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

        private PersonMainInfo UnencryptShareholderPersonalData(string koreShareholderId, string personalData)
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

        #endregion

        #region Public Methods

        public string OnKCXOnboardingApproved( string koreShareholderId)
        {

            //1- Download user from KCX
            PersonResponse personResp = PersonsServiceClient.RequestPerson(koreShareholderId);

            if (personResp != null && personResp.data != null)
            {
                if (personResp.data.pd != null)
                {
                    PersonMainInfo personUnencr = UnencryptShareholderPersonalData(koreShareholderId, personResp.data.pd);

                    Shareholder solidusShareholder = SolidusShareholderBuilder.BuildSolidusShareholderFromKCX(personUnencr);

                    List<Rialto.BusinessEntities.Shareholder> shareholders = ShareholderManager.GetShareholdersByKoreChainId(koreShareholderId);

                    //1-Recover user by SSN // validate emails

                        //2-Exists--> meterlo en estado PRE ONBOARDING

                        //3-No Existe--> Crearlo en estado pre onboarding

                    //4- Invocar el servicio de solidus
                }
                else
                {
                    //TODO- Incorporate logging
                    string msg = string.Format("Critical error recovering KXC shareholder PERSONAL info for Kore Shareholder Id {0}: Missing Personal Info", koreShareholderId);
                    throw new Exception(msg);
                }

            }
            else
                ProcessShareholderError(koreShareholderId, personResp);

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
