using Rialto.Plaid.Common.DTO.Generic.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Plaid.Common.DTO.Generic.Balance
{
    public class GetBalanceResp : BaseResponse
    {
        #region Protected Attributes

        public Account[] accounts { get; set; }

        public Item item { get; set; }

        public string request_id { get; set; }

        #endregion

        #region Public Methods


        public double GetBalanceForTrading()
        {
            if (accounts != null)
            {
                decimal balance = 0;
                foreach (Account acc in accounts)
                {
                    if (acc.type == Account._ACCOUNT_TYPE_DEPOSITORY && acc.subtype == Account._ACCOUNT_SUBTYPE_SAVINGS)
                    {
                        if (acc.balances != null && acc.balances.available.HasValue && acc.balances.iso_currency_code == Account._ACCOUNT_CURRENCY_USD)
                            balance += acc.balances.available.Value;
                    }
                
                }

                return Convert.ToDouble(balance);
            }
            else
                return 0;
        }

        public string GetBalanceCurrency()
        {
            return Account._ACCOUNT_CURRENCY_USD;
        }


        #endregion
    }
}
