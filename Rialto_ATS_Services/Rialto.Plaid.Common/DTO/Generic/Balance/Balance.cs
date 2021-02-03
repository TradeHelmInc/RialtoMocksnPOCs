using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Plaid.Common.DTO.Generic.Balance
{
    //https://plaid.com/docs/api/accounts/
    public class Balance
    {
        #region Public Attributes

        public decimal? available { get; set; }//The amount of funds available to be withdrawn from the account
        public decimal current { get; set; }//The total amount of funds in or owed by the account.
        public decimal? limit { get; set; }//For credit-type accounts, this represents the credit limit.
        public string iso_currency_code { get; set; }//The ISO-4217 currency code of the balance
        public string unofficial_currency_code { get; set; }//The unofficial currency code associated with the balance. 


        #endregion
    }
}
