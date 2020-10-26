using KoreConX.DataAccessLayer;
using Mocks.KoreConX.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreConX.LogicLayer
{
    public class SecuritiesLogicLayer
    {
        #region Public Attributes

        public ShareholderManager ShareholderManager { get; set; }

        #endregion

        #region COnstructors

        public SecuritiesLogicLayer()
        {
            ShareholderManager = new ShareholderManager();

        }

        #endregion

        #region Public Methods


        public Shareholder[] GetShareholders(string koreSecurityId)
        {
            return ShareholderManager.GetShareholders(koreSecurityId);
        }


        #endregion
    }
}
