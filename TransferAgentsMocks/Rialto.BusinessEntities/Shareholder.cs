using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.BusinessEntities
{
    public class Shareholder
    {
        #region Public Methods

        public int Id { get; set; }

        public string Name { get; set; }

        public ShareholderType ShareholderType { get; set; }

        public KoreConXShareholderId KoreConXShareholderId { get; set; }

        #endregion
    }
}
