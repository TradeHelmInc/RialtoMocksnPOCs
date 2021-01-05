using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Common.DTO.Services
{
    public class OnOrderCancelledOrExpiredDTO
    {
        public int SellOrderId { get; set; }

        public double ReleaseQty { get; set; }
    }
}
