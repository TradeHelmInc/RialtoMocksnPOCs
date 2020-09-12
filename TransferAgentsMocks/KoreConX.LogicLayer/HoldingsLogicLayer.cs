using KoreConX.BusinessEntities;
using KoreConX.DataAccessLayer.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreConX.LogicLayer
{
    public class HoldingsLogicLayer
    {
        #region Protected Attributes

        protected PortfolioManager PortfolioManager { get; set; }

        protected SecurityManager SecurityManager { get; set; }

        #endregion


        #region Constructor

        public HoldingsLogicLayer()
        {
            PortfolioManager = new PortfolioManager();

            SecurityManager = new SecurityManager();
        
        }


        #endregion


        #region Public Mehtods


        public bool HasEnoughShares(string koreShareholderId, string koreSecurityId, int qtyToValidate)
        {
            Shareholder sh = PortfolioManager.GetShareholder(koreShareholderId);


            if (sh != null)
            {
                Position pos = sh.Positions.Where(x => x.KoreSecurityId == koreSecurityId).FirstOrDefault();

                if (pos != null)
                {
                    return pos.GetAvailableShares() > qtyToValidate;
                
                }
                else
                    throw new Exception(string.Format(" Invalid koreSecurityId {0}", koreSecurityId));

            }
            else 
                throw new Exception(string.Format(" Invalid koreShareholderId {0}", koreShareholderId));
        
        
        }



        #endregion
    }
}
