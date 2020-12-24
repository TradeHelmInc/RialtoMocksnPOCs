using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Plaid.Common.DTO.Generic.Auth
{
    public class CreateLinkToken
    {

        #region Public Attributes

        public string client_id { get; set; }

        public string secret { get; set; }

        public User user { get; set; }

        public string client_name { get; set; }

        public string[] products { get; set; }

        public string[] country_codes { get; set; }

        public string language { get; set; }

        public string webhook { get; set; }

        public AccountFilter account_filters { get; set; }

        #endregion
    }
}
