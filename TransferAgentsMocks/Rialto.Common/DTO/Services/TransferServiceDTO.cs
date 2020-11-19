using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Common.DTO.Services
{
    public class TransferSharesDTO
    {
        public int BuyShareholderId { get; set; }

        public int SellShareholderId { get; set; }

        public double TradeQuantity { get; set; }

        public int SecurityId { get; set; }

        public int SellOrderId { get; set; }

    }
}
