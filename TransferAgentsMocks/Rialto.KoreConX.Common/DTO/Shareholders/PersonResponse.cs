using Rialto.KoreConX.Common.DTO.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.KoreConX.Common.DTO.Shareholders
{
    public class PersonResponse: BaseGetResponse
    {
        public PersonMainInfo data { get; set; }
    }
}
