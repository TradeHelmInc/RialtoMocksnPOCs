using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Plaid.Common.DTO.Generic.Auth
{
    public class PublicTokenExchangeResp : BaseResponse
    {
        #region Public Attributes

        public string access_token { get; set; }

        public string item_id { get; set; }

        public string request_id { get; set; }

        #endregion
    }
}
