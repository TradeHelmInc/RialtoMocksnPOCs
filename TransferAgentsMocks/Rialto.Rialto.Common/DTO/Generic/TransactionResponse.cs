using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Rialto.Common.DTO.Generic
{
    public class TransactionResponse
    {
        public bool Success { get; set; }

        public IdEntity Id { get; set; }

        public ErrorMessage Error { get; set; }
    }
}
