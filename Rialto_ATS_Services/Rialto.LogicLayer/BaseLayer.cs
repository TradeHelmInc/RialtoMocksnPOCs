using fwk.Common.enums;
using fwk.Common.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet.Messages;

namespace Rialto.LogicLayer
{
    public class BaseLayer
    {
        #region Protected Attributes

        protected ILogger Logger { get; set; }
        
        protected AuditLogic AuditLogic { get; set; }

        #endregion

        #region Protected Methods


        protected void DoLog(string msg, MessageType type)
        {
            Logger.DoLog(msg, type);
        
        }
        
        protected void DoLog(string evType,string msg,string idName=null,string idValue=null)
        {
            AuditLogic.AuditMessage(evType,msg,idName,idValue);
            Logger.DoLog(msg, MessageType.Information);
        
        }
        
        protected void DoLogError(string evType,Exception ex,string msg)
        {
            AuditLogic.AuditException(evType,ex,msg);
            Logger.DoLog(msg, MessageType.Error);
        
        }


        #endregion
    }
}
