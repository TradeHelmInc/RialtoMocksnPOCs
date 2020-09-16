using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocks.KoreConX.Common.DTO.Holdings
{
    public class HoldSharesDTO
    {
        public string last_updated_at { get; set; }

        public string securities_holder_id { get; set; }

        public string koresecurities_id { get; set; }

        public int number_of_shares { get; set; }

        public string ats_id { get; set; }

        public string reason_code { get; set; }

        public string ats_transaction_id { get; set; }
    }
}
