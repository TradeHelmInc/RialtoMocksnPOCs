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

        #endregion

        #region Public Attributes

        #region JHI Attributes

        public int Id { get; set; }

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
    }
}
