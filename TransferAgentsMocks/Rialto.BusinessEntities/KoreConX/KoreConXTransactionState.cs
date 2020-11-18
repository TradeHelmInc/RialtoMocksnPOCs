using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.BusinessEntities
{
    public enum KoreConXTransactionState
    {
        KCX_HOLD_SHARES_SUCCESS,
        KCX_RELEASE_SHARES_SUCCESS,
        KCX_TRANSFER_SHARES_REQUEST,
        KCX_TRANSFER_SHARES_SUCCESS,
        KCX_TRANSFER_SHARES_REJECTED,
        KCX_TRANSFER_SHARES_CRITICAL
    }
}
