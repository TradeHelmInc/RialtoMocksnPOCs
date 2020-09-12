using KoreConX.BusinessEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreConX.DataAccessLayer.Json
{

    public class PortfolioManager
    {
        #region Constructors
        
        public PortfolioManager()
        {

            Shareholders = LoadShareholders();

            Position[] positions = LoadPositions();


            foreach (Shareholder sh in Shareholders)
            {
                List<Position> shPositions = positions.Where(x => x.KoreShareholderId == sh.KoreShareholderId).ToList();

                sh.Positions.AddRange(shPositions);
            }
        
        
        }

        #endregion

        #region Protected Attributes

        protected Shareholder[] Shareholders { get; set; }

        #endregion


        #region Private Methods

        private Position[] LoadPositions()
        {
            string strPositions = File.ReadAllText(@".\input\Positions.json");

            return JsonConvert.DeserializeObject<Position[]>(strPositions);

        }

        private Shareholder[] LoadShareholders()
        {
            string strShareholders = File.ReadAllText(@".\input\Shareholders.json");

            return JsonConvert.DeserializeObject<Shareholder[]>(strShareholders);

        }


        #endregion


        #region Public Methods

        public Shareholder GetShareholder(string koreShareholderId)
        {
            return Shareholders.Where(x => x.KoreShareholderId == koreShareholderId).FirstOrDefault();
        }

        #endregion
    }
}
