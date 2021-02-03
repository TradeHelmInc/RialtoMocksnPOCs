using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Plaid.Common.DTO.Generic.Common
{
    public class Item
    {
        #region Public Attributes

        public string[] available_products { get; set; }

        public string[] billed_products { get; set; }

        public string consent_expiration_time { get; set; }

        public string error { get; set; }

        public string institution_id { get; set; }

        public string item_id { get; set; }

        public string webhook { get; set; }


        #endregion
    }
}
