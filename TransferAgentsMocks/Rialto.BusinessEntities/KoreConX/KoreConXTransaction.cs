using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.BusinessEntities.KoreConX
{
    public class KoreConXTransaction
    {
        #region Public Static Consts

        public static string _STATE_KCX_HOLD_SHARES_SUCCESS = "HOLD";
        public static string _STATE_KCX_RELEASE_SHARES_SUCCESS = "RELEASE";
        public static string _STATE_KCX_TRANSFER_SHARES_REQUEST = "KCX_TRANSFER_SHARES_REQUEST";
        public static string _STATE_KCX_TRANSFER_SHARES_SUCCESS = "KCX_TRANSFER_SHARES_SUCCESS";
        public static string _STATE_KCX_TRANSFER_SHARES_REJECTED = "KCX_TRANSFER_SHARES_REJECTED";
        public static string _STATE_KCX_TRANSFER_SHARES_CRITICAL = "KCX_TRANSFER_SHARES_CRITICAL";
        public static string _STATE_KCX_REQ_RELEASE_SHARES = "KCX_REQ_RELEASE_SHARES";
        public static string _STATE_KCX_RELEASE_SHARES_SUCCESS_FULL = "KCX_RELEASE_SHARES_SUCCESS";
        public static string _STATE_KCX_RELEASE_SHARES_REJECTED = "KCX_RELEASE_SHARES_REJECTED";


        #endregion

        #region Public Attributes

        public int Id { get; set; }

        public KoreConXTransactionState State { get; set; }

        public string TransactionId { get; set; }

        public int NumberOfShares { get; set; }

        public string KoreSecurityId { get; set; }

        public string KoreShareholderId { get; set; }

        public string ATSId { get; set; }

        public int OrderId { get; set; }

        #endregion

        #region Public Static Methods

        public static KoreConXTransactionState GetKoreConXTransactionStateFromStr(string state)
        {
            if (state == _STATE_KCX_HOLD_SHARES_SUCCESS)
                return KoreConXTransactionState.KCX_HOLD_SHARES_SUCCESS;
            else if (state == _STATE_KCX_RELEASE_SHARES_SUCCESS)
                return KoreConXTransactionState.KCX_RELEASE_SHARES_SUCCESS;
            else if (state == _STATE_KCX_RELEASE_SHARES_SUCCESS_FULL)
                return KoreConXTransactionState.KCX_RELEASE_SHARES_SUCCESS_FULL;
            else if (state == _STATE_KCX_TRANSFER_SHARES_REQUEST)
                return KoreConXTransactionState.KCX_TRANSFER_SHARES_REQUEST;
            else if (state == _STATE_KCX_TRANSFER_SHARES_SUCCESS)
                return KoreConXTransactionState.KCX_TRANSFER_SHARES_SUCCESS;
            else if (state == _STATE_KCX_TRANSFER_SHARES_REJECTED)
                return KoreConXTransactionState.KCX_TRANSFER_SHARES_REJECTED;
            else if (state == _STATE_KCX_TRANSFER_SHARES_CRITICAL)
                return KoreConXTransactionState.KCX_TRANSFER_SHARES_CRITICAL;
            else if (state == _STATE_KCX_REQ_RELEASE_SHARES)
                return KoreConXTransactionState.KCX_REQ_RELEASE_SHARES;
            else if (state == _STATE_KCX_RELEASE_SHARES_REJECTED)
                return KoreConXTransactionState.KCX_RELEASE_SHARES_REJECTED;
            else
                throw new Exception(string.Format(" Unknown KCX Transaction State {0}",state));
        }

        public string GetKoreConXTransactionStateToStr()
        {
            if (State == KoreConXTransactionState.KCX_HOLD_SHARES_SUCCESS)
                return _STATE_KCX_HOLD_SHARES_SUCCESS;
            else if (State == KoreConXTransactionState.KCX_RELEASE_SHARES_SUCCESS)
                return _STATE_KCX_RELEASE_SHARES_SUCCESS;
            else if (State == KoreConXTransactionState.KCX_RELEASE_SHARES_SUCCESS_FULL)
                return _STATE_KCX_RELEASE_SHARES_SUCCESS_FULL;
            else if (State == KoreConXTransactionState.KCX_TRANSFER_SHARES_REQUEST)
                return _STATE_KCX_TRANSFER_SHARES_REQUEST;
            else if (State == KoreConXTransactionState.KCX_TRANSFER_SHARES_SUCCESS)
                return _STATE_KCX_TRANSFER_SHARES_SUCCESS;
            else if (State == KoreConXTransactionState.KCX_TRANSFER_SHARES_REJECTED)
                return _STATE_KCX_TRANSFER_SHARES_REJECTED;
            else if (State == KoreConXTransactionState.KCX_TRANSFER_SHARES_CRITICAL)
                return _STATE_KCX_TRANSFER_SHARES_CRITICAL;
            else if (State == KoreConXTransactionState.KCX_REQ_RELEASE_SHARES)
                return _STATE_KCX_REQ_RELEASE_SHARES;
            else if (State == KoreConXTransactionState.KCX_RELEASE_SHARES_REJECTED)
                return _STATE_KCX_RELEASE_SHARES_REJECTED;
            else
                throw new Exception(string.Format(" Cannot convert to string Transaction State {0}", State));
        }


        public KoreConXTransaction Clone()
        {
            return new KoreConXTransaction()
            {
                Id = Id,
                State = State,
                TransactionId = TransactionId,
                NumberOfShares = NumberOfShares,
                KoreSecurityId = KoreSecurityId,
                KoreShareholderId = KoreShareholderId,
                ATSId = ATSId,
                OrderId = OrderId
            };
        
        
        }


        #endregion
    }
}
