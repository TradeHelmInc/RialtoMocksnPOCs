using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.BusinessEntities
{
    public class Position
    {
        #region Public Attributes

        public long Id { get; set; }

        public double? QtyOwned { get; set; }

        public double? JhiValue { get; set; }

        public Security Security { get; set; }

        public User User { get; set; }

        public DateTime? TransactionDate { get; set; }

        public string TransactedUser { get; set; }

        public string TxnType { get; set; }

        public double? ExecutedPrice { get; set; }

        #endregion
    }
}
