using Rialto.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.LogicLayer.Builders
{
    public class SolidusToRialtoBuilder
    {
        #region Private static consts

        private static string _DEFAULT_PASSWORD = "$2a$10$n8gKFcm6QD5K9rsq3Bk.vO5Y56gOFGkGmyQqei6QXqlpvojgSmGwu";

        #endregion

        #region Private Static Methods

        private static User BuildUser(Rialto.Solidus.Common.DTO.Shareholders.Shareholder solidusSh, Rialto.BusinessEntities.Shareholder rialtoSh)
        {
            string email = solidusSh.emailAddress;

            User userInfo = new User();
            userInfo.Id = rialtoSh.GetUser(email) != null ? rialtoSh.GetUser(email).Id : 0;
            userInfo.Login = solidusSh.emailAddress;
            userInfo.PasswordHash = rialtoSh.GetUser(email) != null ? rialtoSh.GetUser(email).PasswordHash : _DEFAULT_PASSWORD;
            userInfo.FirstName = solidusSh.firstName;
            userInfo.LastName = solidusSh.lastName;
            userInfo.Email = solidusSh.emailAddress;
            userInfo.Activated = true;//Users are put back no not active
            userInfo.LangKey = rialtoSh.GetUser(email) != null ? rialtoSh.GetUser(email).LangKey : null;
            userInfo.ActivationKey = rialtoSh.GetUser(email) != null ? rialtoSh.GetUser(email).ActivationKey : null;
            userInfo.ResetKey = rialtoSh.GetUser(email) != null ? rialtoSh.GetUser(email).ResetKey : null;
            userInfo.CreatedBy = rialtoSh.GetUser(email) != null ? rialtoSh.GetUser(email).CreatedBy : User._CREATED_BY_ONBOARDING_SERVICE;
            userInfo.CreatedDate = rialtoSh.GetUser(email) != null ? rialtoSh.GetUser(email).CreatedDate : DateTime.Now;
            userInfo.ResetDate = rialtoSh.GetUser(email) != null ? rialtoSh.GetUser(email).ResetDate : null;
            userInfo.LastModifiedBy =  User._CREATED_BY_ONBOARDING_SERVICE;
            userInfo.LastModifiedDate = DateTime.Now;
            userInfo.PasswordModifyDate = rialtoSh.GetUser(email) != null ? rialtoSh.GetUser(email).PasswordModifyDate : null;
            userInfo.LastActive = rialtoSh.GetUser(email) != null ? rialtoSh.GetUser(email).LastActive : null;
            userInfo.BuyingPower = rialtoSh.GetUser(email) != null ? rialtoSh.GetUser(email).BuyingPower : 0;
            userInfo.UsedLimit = rialtoSh.GetUser(email) != null ? rialtoSh.GetUser(email).UsedLimit : 0;
            userInfo.Phone = !string.IsNullOrEmpty(solidusSh.GetPhone()) ? solidusSh.GetPhone() : (rialtoSh.GetUser(email) != null ? rialtoSh.GetUser(email).Phone : null);
            userInfo.Disclaimer = rialtoSh.GetUser(email) != null ? rialtoSh.GetUser(email).Disclaimer : true;
            userInfo.UsedLimit = rialtoSh.GetUser(email) != null ? rialtoSh.GetUser(email).UsedLimit : 0;
            userInfo.TradeLimit = rialtoSh.GetUser(email) != null ? rialtoSh.GetUser(email).TradeLimit : 0;
            userInfo.RegulatoryFlag = rialtoSh.GetUser(email) != null ? rialtoSh.GetUser(email).RegulatoryFlag : false;
            userInfo.POCFirmId = rialtoSh.GetUser(email) != null ? rialtoSh.GetUser(email).POCFirmId : null;
            userInfo.Capacity = rialtoSh.GetUser(email) != null ? rialtoSh.GetUser(email).Capacity : null;
            userInfo.CashOnHand = rialtoSh.GetUser(email) != null ? rialtoSh.GetUser(email).CashOnHand : 0;
            userInfo.PledgedFund = rialtoSh.GetUser(email) != null ? rialtoSh.GetUser(email).PledgedFund : 0;

            userInfo.Enabled = true;

            return userInfo;

        }

        #endregion

        #region Public Static Methods

        public static void BuildSolidusToRialtoShareholder(string koreShareholderId,Rialto.Solidus.Common.DTO.Shareholders.Shareholder solidusSh,ref Rialto.BusinessEntities.Shareholder rialtoSh)
        {

            //We build the firm
            rialtoSh.Name = solidusSh.GetFullName();

            rialtoSh.Address = solidusSh.GetAddress();

            rialtoSh.Phone = solidusSh.GetPhone();

            rialtoSh.Email = solidusSh.emailAddress;

            rialtoSh.FirmLimit = 0;

            rialtoSh.Enabled = true;

            rialtoSh.FirmAccountNumber = null;

            rialtoSh.Principal = null;

            rialtoSh.FirmTaxId = solidusSh.taxIdOrSSNNumber;

            rialtoSh.PepStatus = null;

            rialtoSh.InsiderStatus = null;

            rialtoSh.FeeMatrix = null;

            rialtoSh.IssuerFirmCheckbox = false;

            rialtoSh.LargeTraderId = null;

            rialtoSh.UniqueId = null;

            rialtoSh.LargeTraderFlag = false;

            rialtoSh.Status = false;

            rialtoSh.FirmType = Rialto.BusinessEntities.Shareholder._FIRM_TYPE_INDIV_RETAIL ;

            rialtoSh.Users.Add(BuildUserFromSolidus(solidusSh));

            rialtoSh.KoreConXShareholderId = new KoreConXShareholderId(){KoreShareholderId=koreShareholderId};
        }


        public static User BuildUserFromSolidus(Rialto.Solidus.Common.DTO.Shareholders.Shareholder solidusSh)
        {
            string email = solidusSh.emailAddress;

            User userInfo = new User();
            userInfo.Id = 0;
            userInfo.Login = solidusSh.emailAddress;
            userInfo.PasswordHash = _DEFAULT_PASSWORD;
            userInfo.FirstName = solidusSh.firstName;
            userInfo.LastName = solidusSh.lastName;
            userInfo.Email = solidusSh.emailAddress;
            userInfo.Activated = true;//Users are put back no not active
            userInfo.LangKey =  null;
            userInfo.ActivationKey = null;
            userInfo.ResetKey = null;
            userInfo.CreatedBy = User._CREATED_BY_ONBOARDING_SERVICE;
            userInfo.CreatedDate = DateTime.Now;
            userInfo.ResetDate = null;
            userInfo.LastModifiedBy = User._CREATED_BY_ONBOARDING_SERVICE;
            userInfo.LastModifiedDate = DateTime.Now;
            userInfo.PasswordModifyDate =null;
            userInfo.LastActive = null;
            userInfo.BuyingPower =  0;
            userInfo.UsedLimit =  0;
            userInfo.Phone =  null;
            userInfo.Disclaimer = true;
            userInfo.UsedLimit =  0;
            userInfo.TradeLimit =  0;
            userInfo.RegulatoryFlag = false;
            userInfo.POCFirmId = null;
            userInfo.Capacity = null;
            userInfo.CashOnHand =  0;
            userInfo.PledgedFund =  0;
            userInfo.Enabled = true;

            return userInfo;

        }

        #endregion
    }
}
