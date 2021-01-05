using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.KoreConX.Common.DTO.Generic
{
    public class BaseGetResponse
    {
        public string Resp { get; set; }

        public GenericGetError message { get; set; }
    }
}
