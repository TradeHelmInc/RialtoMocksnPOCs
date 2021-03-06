﻿using fwk.Common.enums;
using fwk.Common.interfaces;
using fwk.Common.util.encryption.common;
using Rialto.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fwk.Common.util.logger;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
//using ToolsShared.Logging;

namespace OnboardingApp
{
    public class Startup{

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }
        
        public void Configure(IApplicationBuilder app){
            app.Run(context => {
                Console.WriteLine("Aca:"+DateTime.Now);
                return context.Response.WriteAsync("Hello world2");
            });
            
            app.UseMvc();
            
            
        }
    }
    
    class Program : ILogger
    {
        #region Protected Attributes
        protected Logger Logger { get; set; }

        #endregion

        #region Public  Methods
        public void DoLog(string msg, MessageType type)
        {
            Logger.DoLog(msg,type);
        }

        #endregion

        static void Main(string[] args)
        {
            Program logger = new Program() {Logger = new Logger(ConfigurationManager.AppSettings["DebugLevel"])};

            try
            {
                var host = new WebHostBuilder()
                    .UseKestrel()
                    //.UseUrls(ManagementServiceURL)
                    .UseStartup<Startup>()
                    .Build();

                //host.RunAsync();
                host.Run();
                
                /*string tradingCS = ConfigurationManager.AppSettings["TradingDBConnectionString"];
                string orderCS = ConfigurationManager.AppSettings["OrdersDBConnectionString"];
                string kcxURL = ConfigurationManager.AppSettings["KCXURL"];
                string solidusURL = ConfigurationManager.AppSettings["SolidusURL"];
                string onboardingServiceURL = ConfigurationManager.AppSettings["OnboardingServiceURL"];
                //string kcxKeyAndIVPath = ConfigurationManager.AppSettings["KCXKeyAndIVPath"];
                
                // string kcxEncryptedKeyAndIVPath = ConfigurationManager.AppSettings["KCXEncryptedKeyAndIVPath"];
                // string kcxDeccryptedKeyAndIVPath = ConfigurationManager.AppSettings["KCXDecryptedKeyAndIVPath"];
                // bool AESKeyEncrypted = Convert.ToBoolean(ConfigurationManager.AppSettings["AESSKeyEncrypted"]);
                // string kcxKeyAndIV = AESKeyEncrypted ? FileLoader.GetFileContent(kcxEncryptedKeyAndIVPath) : FileLoader.GetFileContent(kcxDeccryptedKeyAndIVPath);

                string kcxEncryptedRSAPrivateKeyPath = ConfigurationManager.AppSettings["KCXEncryptedRSAPrivateKeyPath"];
                string kcxDeccryptedRSAPrivateKeyPath = ConfigurationManager.AppSettings["KCXDeccryptedRSAPrivateKeyPath"];
                string kcxPublicKeyPath = ConfigurationManager.AppSettings["KCXPublicKeyPath"];

                
                bool RSAKeyEncrypted = Convert.ToBoolean(ConfigurationManager.AppSettings["RSAPrivateKeyEncrypted"]);

                logger.DoLog("Initializing Onboarding app service", MessageType.Information);
                logger.DoLog("Extracting AES key", MessageType.Information);
                

                string kcxRSAPrivateKeyPath = RSAKeyEncrypted ? kcxEncryptedRSAPrivateKeyPath : kcxDeccryptedRSAPrivateKeyPath;
                
                //string kcxKeyAndIV = FileLoader.GetFileContent(kcxKeyAndIVPath);

                ManagementService transService = new ManagementService(tradingCS, orderCS, onboardingServiceURL, kcxURL,kcxPublicKeyPath,
                                                                        kcxRSAPrivateKeyPath, RSAKeyEncrypted, solidusURL, logger);

                transService.Run();

                logger.DoLog(string.Format("Onboarding Service successfully initialized at {0}", onboardingServiceURL), MessageType.Information);

                Console.ReadKey();*/
            }
            catch (Exception ex)
            {
                logger.DoLog(string.Format("Critical error initializing Onboarding Service:{0}", ex.Message), MessageType.Error);
            }
        }
    }
}
