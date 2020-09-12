using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolsShared.Logging;

namespace fwk.DataAccessLayer
{
    public enum MessageType { Information, Debug, Error, Exception, EndLog };

    public class BaseDataAccessLayer
    {
        #region Protected Attributes

        protected ILogSource Logger;

        #endregion

        #region Constructors

        public BaseDataAccessLayer()
        {

            Logger = new PerDayFileLogSource(Directory.GetCurrentDirectory() + "\\Log", Directory.GetCurrentDirectory() + "\\Log\\Backup")
            {
                FilePattern = "Log.{0:yyyy-MM-dd}.log",
                DeleteDays = 20
            };

            DoLog("Initializing Mock Server...", MessageType.Information);
        
        }


        #endregion

        #region Protected Methods

        #region Util Methods

        protected void DoLog(string msg, MessageType type)
        {
            Logger.Debug(msg, type);
        }

        #endregion


        #endregion

    }
}
