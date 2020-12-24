using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Plaid.Common.DTO.Generic.Auth
{
    public class AuthReq
    {
        #region Public Attributes

        public string access_token { get; set; }

        public string client_id { get; set; }

        public string secret { get; set; }

        #endregion
    }
}
