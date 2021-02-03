using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Plaid.Common.DTO.Generic
{
    public class GenericError
    {
        public string display_message { get; set; }

        public string documentation_url { get; set; }

        public string error_code { get; set; }

        public string error_message { get; set; }

        public string error_type { get; set; }

        public string request_id { get; set; }

        public string suggested_action { get; set; }
    }
}
