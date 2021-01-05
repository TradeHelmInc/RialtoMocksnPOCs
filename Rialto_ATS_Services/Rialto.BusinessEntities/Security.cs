using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.BusinessEntities
{
    public class Security
    {
        #region Public Attriutes

        public int Id { get; set; }

        public string Name { get; set; }

        public string CUSIP { get; set; }

        public string Symbol { get; set; }

        public KoreConXSecurityId KoreConXSecurityId { get; set; }

        #endregion
    }
}
