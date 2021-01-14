using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Common.DTO.Services.Plaid
{
    public class OnPlaidCredentialsLoadDTO
    {
        #region Public Attributes

        public string UserIdentifier { get; set; }

        public string PlaidAccessToken { get; set; }

        public string PlaidItemId { get; set; }

        #endregion
    }
}
