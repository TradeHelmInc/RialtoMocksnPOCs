using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.BusinessEntities
{
    public class User
    {
        #region Public Static Consts

        public static string _CREATED_BY_ONBOARDING_SERVICE = "onboarding_service";

        public static string _ROLE_USER = "ROLE_USER";

        public static string _ROLE_ADMIN = "ROLE_ADMIN";

        #endregion

        #region Public Attributes

        #region JHI Attributes

        public long Id { get; set; }

        public string Login { get; set; }

        public string PasswordHash { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public bool Activated { get; set; }

        public string LangKey { get; set; }

        public string ActivationKey { get; set; }

        public string ResetKey { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ResetDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public DateTime? PasswordModifyDate { get; set; }

        public DateTime? LastActive { get; set; }

        public string Role { get; set; }

        #endregion

        #region InfoAttributes

        public double BuyingPower { get; set; }

        public double UsedLimit { get; set; }

        public bool Enabled { get; set; }

        public string Phone { get; set; }

        public bool Disclaimer { get; set; }

        public double TradeLimit { get; set; }

        public bool RegulatoryFlag { get; set; }

        public int FirmId { get; set; }

        public long? POCFirmId { get; set; }

        public string Capacity { get; set; }

        public double CashOnHand { get; set; }

        public double PledgedFund { get; set; }

        #endregion


        #endregion
        
        #region Public Methods

        public string GetFullName()
        {
            string name = "";

            name += FirstName != null ? FirstName + " " : "";
            name += LastName != null ? LastName : "";

            return name;
        }

        #endregion
    }
}
