using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreConX.BusinessEntities
{
    public class Shareholder
    {

        #region Constructor

        public Shareholder() 
        {
            Positions = new List<Position>();
        }


        #endregion

        #region Public Attributes

        public string KoreShareholderId { get; set; }

        public string Name { get; set; }

        public List<Position> Positions { get; set; }

        #endregion
    }
}
