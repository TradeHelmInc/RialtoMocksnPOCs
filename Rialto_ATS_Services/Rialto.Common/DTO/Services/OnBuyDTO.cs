using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Common.DTO.Services
{
    public class OnBuyDTO
    {
        public int BuyShareholderId { get; set; }

        public int SecurityId { get; set; }

        public double OrderQty { get; set; }
    }
}
