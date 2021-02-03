using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Plaid.Common.DTO.Generic
{
    public class BaseResponse
    {
        public string Resp { get; set; }

        public GenericError GenericError { get; set; }
    }
}
