using Mocks.KoreConX.BusinessEntities;
using Mocks.KoreConX.Common.DTO.Holdings;
using Mocks.KoreConX.Common.DTO.Securities;
using Mocks.KoreConX.DataAccessLayer.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocks.KoreConX.LogicLayer
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

        public string TransferShares(TransferSharesDTO transferDTO)
        {
            Shareholder owner = PortfolioManager.GetShareholder(transferDTO.owner_id);

            if (owner != null)
            {
                Shareholder dest = PortfolioManager.GetShareholder(transferDTO.transferred_to_id);

                if (dest != null)
                {

                    Position posOwner = owner.Positions.Where(x => x.KoreSecurityId == transferDTO.koresecurities_id).FirstOrDefault();
                    Position posDest = dest.Positions.Where(x => x.KoreSecurityId == transferDTO.koresecurities_id).FirstOrDefault();


                    if (posOwner != null)
                    {
                        if (posDest == null)
                        {
                            posDest = new Position()
                            {
                                KoreSecurityId = transferDTO.koresecurities_id,
                                KoreShareholderId = dest.KoreShareholderId,
                                OnHold = 0,
                                Shares = 0
                            };
                        }


                        if (transferDTO.total_securities > posOwner.GetAvailableShares())
                        {
                            throw new Exception(string.Format("Shareholder owner does not have enough shares in his holding!"));
                        }
                        else
                        {
                            string txtId = Guid.NewGuid().ToString();

                            posOwner.OnHold -= transferDTO.total_securities;
                            posOwner.Shares -= transferDTO.total_securities;
                            posOwner.TransferTransactionIds.Add(txtId);

                            posDest.Shares += transferDTO.total_securities;
                            posDest.TransferTransactionIds.Add(txtId);

                            return txtId;
                        }

                    }
                    else
                        throw new Exception(string.Format(" Invalid position for koreSecurityId {0}", transferDTO.koresecurities_id));
                }
                else
                    throw new Exception(string.Format(" Invalid destination koreShareholderId {0}", transferDTO.transferred_to_id));
            }
            else
                throw new Exception(string.Format(" Invalid owner koreShareholderId {0}", transferDTO.owner_id));

        
        
        }

        //Returns transactionId
        public string HoldShares(HoldSharesDTO holdDto)
        {

            Shareholder sh = PortfolioManager.GetShareholder(holdDto.securities_holder_id);

            if (sh != null)
            {
                Position pos = sh.Positions.Where(x => x.KoreSecurityId == holdDto.koresecurities_id).FirstOrDefault();

                if (pos != null)
                {
                    if (holdDto.number_of_shares > pos.GetAvailableShares())
                    {
                        throw new Exception(string.Format("Shareholder does not have enough shares in his holding!"));
                    }
                    else
                    {
                        string txtId = Guid.NewGuid().ToString();
                        pos.HoldTransactionIds.Add(txtId);
                        pos.OnHold += holdDto.number_of_shares;
                        return txtId;
                    }

                }
                else
                    throw new Exception(string.Format(" Invalid koreSecurityId {0}", holdDto.koresecurities_id));

            }
            else
                throw new Exception(string.Format(" Invalid koreShareholderId {0}", holdDto.securities_holder_id));



        }


        //Returns transactionId
        public string ReleaseShares(ReleaseSharesDTO releaseDto)
        {

            Shareholder sh = PortfolioManager.GetShareholder(releaseDto.securities_holder_id);

            if (sh != null)
            {
                Position pos = sh.Positions.Where(x => x.KoreSecurityId == releaseDto.koresecurities_id).FirstOrDefault();

                if (pos != null)
                {
                    if (releaseDto.number_of_shares > pos.OnHold)
                    {
                        throw new Exception(string.Format("Shareholder does not have enough shares on hold in his holding!"));
                    }
                    else if (!pos.HoldTransactionIds.Contains(releaseDto.ats_transaction_id))
                        throw new Exception(string.Format("Invalid tx Id {}!",releaseDto.ats_transaction_id));
                    else
                    {
                        string txtId = Guid.NewGuid().ToString();
                        pos.HoldTransactionIds.Remove(releaseDto.ats_transaction_id);
                        pos.HoldTransactionIds.Add(txtId);
                        pos.OnHold -= releaseDto.number_of_shares;
                        return txtId;
                    }

                }
                else
                    throw new Exception(string.Format(" Invalid koreSecurityId {0}", releaseDto.koresecurities_id));

            }
            else
                throw new Exception(string.Format(" Invalid koreShareholderId {0}", releaseDto.securities_holder_id));



        }

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
