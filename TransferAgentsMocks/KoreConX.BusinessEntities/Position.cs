using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreConX.BusinessEntities
{
    public class Position
    {
        #region Public Attributes

        public string KoreSecurityId { get; set; }

        public string KoreShareholderId { get; set; }

        public int Shares { get; set; }

        public int OnHold { get; set; }
        
        #endregion


        #region Public Methods

        public int GetAvailableShares()
        {
            return Shares - OnHold;
        }


        #endregion
    }
}
