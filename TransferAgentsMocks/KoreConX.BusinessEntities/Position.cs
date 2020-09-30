using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocks.KoreConX.BusinessEntities
{
    public class Position
    {
        #region Constructors

        public Position()
        {
            HoldTransactionIds = new List<string>();
            TransferTransactionIds = new List<string>();
        }


        #endregion

        #region Public Attributes

        public string KoreSecurityId { get; set; }

        public string KoreShareholderId { get; set; }

        public int Shares { get; set; }

        public int OnHold { get; set; }

        public List<string> HoldTransactionIds { get; set; }

        public List<string> TransferTransactionIds { get; set; }
        
        #endregion


        #region Public Methods

        public int GetAvailableShares()
        {
            return Shares - OnHold;
        }


        #endregion
    }
}
