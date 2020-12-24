using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Plaid.Common.DTO.Generic.Balance
{
    public class Account
    {
        #region Public Attributes

        public string account_id { get; set; }
        public Balance balances { get; set; }
        public string mask { get; set; }
        public string name { get; set; }
        public string official_name { get; set; }
        public string type { get; set; }
        public string sybtype { get; set; }
        public string loan { get; set; }

        #endregion
    }
}
