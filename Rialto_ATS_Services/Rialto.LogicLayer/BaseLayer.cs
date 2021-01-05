using fwk.Common.enums;
using fwk.Common.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.LogicLayer
{
    public class BaseLayer
    {
        #region Protected Attributes

        protected ILogger Logger { get; set; }

        #endregion

        #region Protected Methods


        protected void DoLog(string msg, MessageType type)
        {
            Logger.DoLog(msg, type);
        
        }


        #endregion
    }
}
