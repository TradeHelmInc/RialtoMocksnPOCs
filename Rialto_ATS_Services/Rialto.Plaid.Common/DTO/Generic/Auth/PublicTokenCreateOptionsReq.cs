using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Plaid.Common.DTO.Generic.Auth
{
    public class PublicTokenCreateOptionsReq
    {
        public string webhook { get; set; }

        public string override_username { get; set; }

        public string override_password { get; set; }
    }
}
