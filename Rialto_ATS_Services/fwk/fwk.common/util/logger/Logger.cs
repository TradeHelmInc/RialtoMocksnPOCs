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
        #region Protected Static Consts

        protected static string _INFO = "INFO";
        protected static string _DEBUG = "DEBUG";

        #endregion
        
        #region Protected Attributes
        
        protected string DebugLevel { get; set; }
        
        #endregion
        
        #region Constructors

        public Logger(string pDebugLevel)
        {
            DebugLevel = pDebugLevel;

        }

        #endregion

        #region Protected Static Methods

        //This should be logged, but we will write it on the screen for simplicity
        public  void DoLog(string message,MessageType type)
        {
            if (type == MessageType.Debug && DebugLevel != _DEBUG)
                return;

            if (type == MessageType.Debug)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                //Logger.Debug(msg, type);
            }
            else if (type == MessageType.Information)
            {
                //Logger.Debug(msg, type);
            }
            else if (type == MessageType.Error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                //Logger.Alert(msg, type);
            }
            else if (type == MessageType.EndLog)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                //Logger.Debug(msg, type);
            }
            else if (type == MessageType.Exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                //Logger.Debug(msg, type);
            }

            Console.WriteLine(message);

            Console.ResetColor();
        }

        #endregion
    }
}
