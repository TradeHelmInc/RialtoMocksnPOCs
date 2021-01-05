using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.BusinessEntities
{
    public class Trade
    {
        #region Public Attributes

        public DateTime ExecutionDate { get; set; }

        public string MatchingId { get; set; }

        public int SecurityId { get; set; }

        public string Symbol { get; set; }

        public int BuyerId { get; set; }

        public string BuyerName { get; set; }

        public int SellerId { get; set; }

        public string SellerName { get; set; }

        public int TradeSize { get; set; }

        public double TradePrice { get; set; }

        public double TradeNotional { get; set; }

        public int BuyOrderId { get; set; }

        public int SellOrderId { get; set; }

        public string Status { get; set; }

        #endregion
    }
}
