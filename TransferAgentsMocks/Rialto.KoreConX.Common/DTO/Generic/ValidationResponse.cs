using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.KoreConX.Common.DTO.Generic
{
    public class ValidationResponse : BaseResponse
    {
        public ExistsEntity data { get; set; }
    }
}
