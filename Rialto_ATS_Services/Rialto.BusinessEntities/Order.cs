using Rialto.BusinessEntities.KoreConX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.BusinessEntities
{
    public class Order
    {

        #region Pricate Static Const

        public static string _SIDE_BUY = "BUY";
        public static string _SIDE_SELL = "SELL";

        public static string _STATE_OPEN="OPEN";
        public static string _STATE_PORTFOLIO = "PORTFOLIO";
        public static string _STATE_EXPIRED = "EXPIRED";
        public static string _STATE_CLOSED = "CLOSED";
        public static string _STATE_PARTIAL = "PARTIAL";
        public static string _STATE_EXECUTED = "EXECUTED";
        public static string _STATE_OTHER = "OTHER";

        #endregion

        #region Constructors

        public Order()
        {
            KoreConXTransactions = new List<KoreConXTransaction>();
        }

       

        #endregion

        #region Public Attributes

        public int Id { get; set; }

        public string Symbol { get; set; }

        public int Qty { get; set; }//OrdQty

        public Side Side { get; set; }

        public double? Price { get; set; }

        public State State { get; set; }//OrdStatus

        public double? LeavesQty { get; set; }

        public double? ExecutedQty { get; set; }//CumQty

        public double? TradePrice { get; set; }

        public DateTime DateTimeReceived { get; set; }//CreationTime

        public int ShareholderId { get; set; }//

        public DateTime? ExecutionTime { get; set; }

        public string MatchingId { get; set; } //TradeId

        #region Transfer Agents Fields

        public List<KoreConXTransaction> KoreConXTransactions { get; set; }

        #endregion

        #endregion

        #region Public Static Methods

        public bool IsLiveOrder()
        {
            return State == State.OPEN || State == State.PARTIAL;

        }


        public static Side GetSideFromStr(string side)
        {
            if (side == _SIDE_BUY)
                return Side.Buy;
            else if (side == _SIDE_SELL)
                return Side.Sell;
            else
                throw new Exception(string.Format("Could not reconize side {0}", side));
        }


        public static State GetStateFromStr(string state)
        {
            if (state == _STATE_PORTFOLIO)
                return State.PORTFOLIO;
            else if (state == _STATE_OPEN)
                return State.OPEN;
            else if (state == _STATE_EXPIRED)
                return State.EXPIRED;
            else if (state == _STATE_CLOSED)
                return State.CLOSED;
            else if (state == _STATE_PARTIAL)
                return State.PARTIAL;
            else if (state == _STATE_EXECUTED)
                return State.EXECUTED;
            else if (state == _STATE_OTHER)
                return State.OTHER;
            else
                throw new Exception(string.Format("Could not reconize state {0}", state));
        }

        public KoreConXTransaction GetHoldTransaction()
        {
            return KoreConXTransactions.Where(x => x.State == KoreConXTransactionState.KCX_HOLD_SHARES_SUCCESS).FirstOrDefault();
        
        }

        #endregion
    }
}
