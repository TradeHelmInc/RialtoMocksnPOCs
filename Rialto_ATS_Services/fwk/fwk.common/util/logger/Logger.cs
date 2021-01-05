using fwk.Common.enums;
using fwk.Common.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fwk.Common.util.logger
{
    public class Logger : ILogger
    {
        #region Constructors

        public Logger()
        {

            
        }

        #endregion

        #region Protected Static Methods

        //This should be logged, but we will write it on the screen for simplicity
        public  void DoLog(string message,MessageType type)
        {
            Console.WriteLine(message);
        }

        #endregion
    }
}
