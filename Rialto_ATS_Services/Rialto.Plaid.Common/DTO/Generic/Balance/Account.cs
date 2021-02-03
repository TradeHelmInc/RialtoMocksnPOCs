using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Plaid.Common.DTO.Generic.Balance
{
    public class Account
    {
        #region Public static consts

        public static string _ACCOUNT_TYPE_DEPOSITORY = "depository";
        public static string _ACCOUNT_SUBTYPE_SAVINGS = "savings";

        public static string _ACCOUNT_CURRENCY_USD = "USD";

        #endregion

        #region Public Attributes

        public string account_id { get; set; }
        public Balance balances { get; set; }
        public string mask { get; set; }
        public string name { get; set; }
        public string official_name { get; set; }
        public string type { get; set; }
        public string subtype { get; set; }
        public string loan { get; set; }

        #endregion

    }
}
