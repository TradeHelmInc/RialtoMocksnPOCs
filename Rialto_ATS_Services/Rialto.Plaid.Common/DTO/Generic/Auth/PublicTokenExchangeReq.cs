using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Plaid.Common.DTO.Generic.Auth
{
    public class PublicTokenExchangeReq
    {
        #region Public Attributes

        public string client_id { get; set; }

        public string secret { get; set; }

        public string public_token { get; set; }


        #endregion
    }
}
