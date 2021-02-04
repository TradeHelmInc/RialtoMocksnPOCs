using fwk.Common.interfaces;
using Rialto.LogicLayer;
using Rialto.ServiceLayer.service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fwk.Common.util.logger;
using Rialto.ServiceLayer.config;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Rialto.Common.DTO.Generic;
using Rialto.Common.DTO.Services;


namespace Rialto.ServiceLayer
{
    
  
    public class ManagementService
    {

        #region Protected Attributes

        protected static string ManagementServiceURL { get; set; }

        protected ManagementLogic ManagementLogic { get; set; }

        //protected static HttpSelfHostServer Server { get; set; }

        protected ILogger Logger { get; set; }

        #endregion

        #region Constructors
        
        public ManagementService(string pTradingCS, string pOrderCS, string pManagementServiceURL, string pKcxName, string pKCXPublicKeyPath,
            string pRASPrivateKeyPath, bool pRSAKeyEncrypted, string pSolidusURL, ILogger pLogger)
        {

            ManagementServiceURL = pManagementServiceURL;

            ManagementLogic = new ManagementLogic(pTradingCS, pOrderCS, pKcxName, pKCXPublicKeyPath, pRASPrivateKeyPath, pRSAKeyEncrypted, pSolidusURL, pLogger);

            Logger = pLogger;

        }
        
        public ManagementService(string pTradingCS, string pOrderCS, string pManagementServiceURL,  ILogger pLogger)
        {

            ManagementServiceURL = pManagementServiceURL;

            ManagementLogic = new ManagementLogic(pTradingCS, pOrderCS, pLogger);

            Logger = pLogger;

        }
        
        #endregion

        #region Protected Methods

        protected string OnKCXOnboardingApproved(string koreShareholderId,string companyKoreChainId, string key, string IV)
        {
            return ManagementLogic.OnKCXOnboardingApproved(koreShareholderId, companyKoreChainId, key, IV);
        }
        
        protected string OnKCXOnboardingApproved_4096(string[] param)
        {
            return ManagementLogic.OnKCXOnboardingApproved_4096(param);
        }
        
        protected string OnApplicationApproval(string email)
        {
            return ManagementLogic.OnApplicationApproval(email);
        }

        protected string OnKCXOnboardingStarted(string koreShareholderId,string companyKoreChainId)
        {
            return ManagementLogic.OnKCXOnboardingStarted(koreShareholderId, companyKoreChainId);
        }

        #endregion
        
        #region Protected Static Methods
        
        #endregion

        #region Public Methods

        public void Run()
        {
            ManagementController.OnKCXOnboardingApproved += OnKCXOnboardingApproved;
            ManagementController.OnKCXOnboardingApproved_4096 += OnKCXOnboardingApproved_4096;
            ManagementController.OnApplicationApproval += OnApplicationApproval;
            //ManagementController.OnKCXOnboardingStarted += OnKCXOnboardingStarted;

            ManagementController.Logger = Logger;
            //ManagementStartup.Logger = Logger;
            
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls(ManagementServiceURL)
                .UseStartup<ManagementStartup>()
                .Build();

            host.RunAsync();
            //host.Run();
            
            //SwaggerConfig.Register(config);
            //WebApiConfig.Register(config);
        }

        #endregion
    }
}
