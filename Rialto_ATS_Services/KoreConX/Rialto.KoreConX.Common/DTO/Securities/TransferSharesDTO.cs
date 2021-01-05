using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.KoreConX.Common.DTO.Securities
{
    public class TransferSharesDTO
    {
        #region Public Attributes


        public string company_id { get; set; }

        public string koresecurities_id { get; set; }

        public string owner_id { get; set; }

        public string transferred_to_id { get; set; }

        public string transfer_authorization_transaction_id { get; set; }

        public int total_securities { get; set; }

        public DateTime effective_date { get; set; }

        #endregion
    }
}
