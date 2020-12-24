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
    }
}
