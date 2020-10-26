using Mocks.KoreConX.BusinessEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreConX.DataAccessLayer
{
    public class ShareholderManager
    {
        #region Constructors

        public ShareholderManager()
        {
            Position[] Positions = LoadPositions();

            CompanyShareholders = new Dictionary<string, List<Shareholder>>();

            foreach (Position pos in Positions)
            {

                if (!CompanyShareholders.ContainsKey(pos.KoreSecurityId))
                {
                    CompanyShareholders.Add(pos.KoreSecurityId, new List<Shareholder>());
                }

                CompanyShareholders[pos.KoreSecurityId].Add(new Shareholder() { KoreShareholderId = pos.KoreShareholderId });
            }

        }

        #endregion

        #region Protected Attributes

     

        protected Dictionary<string, List<Shareholder>> CompanyShareholders { get; set; }

        #endregion

        #region Private Methods

        private Position[] LoadPositions()
        {
            string strPositions = File.ReadAllText(@".\input\Positions.json");

            return JsonConvert.DeserializeObject<Position[]>(strPositions);

        }

        #endregion

        #region Public Methods

        public Shareholder[] GetShareholders(string koreSecurityId)
        {
            if (CompanyShareholders.ContainsKey(koreSecurityId))
                return CompanyShareholders[koreSecurityId].ToArray();
            else
                return new List<Shareholder>().ToArray();
        }

        #endregion
    }
}
