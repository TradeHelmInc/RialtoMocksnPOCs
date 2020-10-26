using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.KoreConX.Common.DTO.Generic
{

    public class DataResponse : BaseGetResponse
    {
        #region Public Attributes

        public DataEntity data { get; set; }

        #endregion
    }
}
