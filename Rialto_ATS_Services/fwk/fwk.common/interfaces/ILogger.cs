using fwk.Common.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fwk.Common.interfaces
{
    public interface ILogger
    {
        void DoLog(string message, MessageType type);
    }
}
