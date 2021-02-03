using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Plaid.Common.DTO.Generic.Auth
{
    public class PublicTokenCreateReq
    {
        #region Public Attributes

        public string client_id { get; set; }

        public string secret { get; set; }

        public string institution_id { get; set; }

        public string[] initial_products { get; set; }

        public PublicTokenCreateOptionsReq options { get; set; }

        #endregion
    }
}
