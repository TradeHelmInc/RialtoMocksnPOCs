using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Rialto.Common.DTO.Generic
{
    public class GetResponse
    {
        public bool Success { get; set; }

        public object data { get; set; }

        public ErrorMessage Error { get; set; }
    }
}
