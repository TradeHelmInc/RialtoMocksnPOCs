using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.BusinessEntities
{
    public class Account
    {
        #region Public Static Attributes

        public static string _ACCOUNT_PREFIX = "USAUS";

        #endregion

        #region Public Attributes

        public long Id { get; set; }

        public string AccountNumber { get; set; }

        public string AccountName { get; set; }

        public bool AccountStatus { get; set; }

        public string SettlementInstructions { get; set; }

        public string ClientContact { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string AccountType { get; set; }

        public DateTime? ActivationDate { get; set; }

        public string AddressStreet { get; set; }

        public string AddressCity { get; set; }

        public string AddressState { get; set; }

        public string AddressCountry { get; set; }

        public string AddressZipCode { get; set; }

        public string TaxDomicile { get; set; }

        public string TaxFormType { get; set; }

        public string TaxFormStatus { get; set; }

        public string SSNTinEin { get; set; }

        public string PepStatus { get; set; }

        public string InsiderStatus { get; set; }

        public string FinraRegistration { get; set; }

        public string FeeMatrix { get; set; }

        public string TopAccount { get; set; }

        public Shareholder Shareholder { get; set; }

        public int? UserCount { get; set; }

        public double? SecFees { get; set; }

        public double? Commission { get; set; }

        public User DefaultUser { get; set; }

        #endregion
    }
}
