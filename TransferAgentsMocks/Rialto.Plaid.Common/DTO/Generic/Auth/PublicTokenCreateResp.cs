using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Plaid.Common.DTO.Generic.Auth
{
    public class PublicTokenCreateResp : BaseResponse
    {
        public string public_token { get; set; }

        public string request_id { get; set; }
    }
}
