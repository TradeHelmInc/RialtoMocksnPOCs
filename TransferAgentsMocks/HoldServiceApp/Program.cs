﻿using fwk.Common.enums;
using fwk.Common.interfaces;
using Rialto.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolsShared.Logging;

namespace OnSellApp
{
    class Program : ILogger
    {
        #region Protected Attributes

        protected PerDayFileLogSource Logger { get; set; }

        protected string DebugLevel { get; set; }

        #endregion

        #region Protected Static Consts

        protected static string _INFO = "INFO";
        protected static string _DEBUG = "DEBUG";

        #endregion

        #region Public  Methods


        public void DoLog(string msg, MessageType type)
        {

            if (type == MessageType.Debug && DebugLevel != _DEBUG)
                return;

            if (type == MessageType.Debug)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Logger.Debug(msg, type);
            }
            else if (type == MessageType.Information)
                Logger.Alert(msg, type);
            else if (type == MessageType.Error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Logger.Debug(msg, type);
            }
            else if (type == MessageType.EndLog)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Logger.Debug(msg, type);
            }
            else if (type == MessageType.Exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Logger.Debug(msg, type);
            }

            Console.WriteLine(msg);

            Console.ResetColor();


        }


        #endregion

        static void Main(string[] args)
        {
            Program logger = new Program() { DebugLevel = ConfigurationManager.AppSettings["DebugLevel"] };

            try
            {
                
                string tradingCS = ConfigurationManager.AppSettings["TradingDBConnectionString"];
                string orderCS = ConfigurationManager.AppSettings["OrdersDBConnectionString"];
                string kcxURL = ConfigurationManager.AppSettings["KCXURL"];
                string holdingServiceURL = ConfigurationManager.AppSettings["HoldingsServiceURL"];

                

                logger.Logger = new PerDayFileLogSource(Directory.GetCurrentDirectory() + "\\Log", Directory.GetCurrentDirectory() + "\\Log\\Backup")
                {
                    FilePattern = "Log.{0:yyyy-MM-dd}.log",
                    DeleteDays = 20
                };

                logger.DoLog("Initializing OnSell Service",MessageType.Information);

                HoldingsService holdingsService = new HoldingsService(tradingCS, orderCS, kcxURL, holdingServiceURL, logger);

                holdingsService.Run();

                logger.DoLog(string.Format("OnSell Service successfully initialized at {0}", holdingServiceURL), MessageType.Information);

                Console.ReadKey();
            }
            catch (Exception ex)
            {
                logger.DoLog(string.Format("Critical error initializing OnSell Service:{0}", ex.Message), MessageType.Error);

            }
        }
    }
}
