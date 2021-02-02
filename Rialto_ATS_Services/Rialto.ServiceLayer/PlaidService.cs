using fwk.Common.interfaces;
using Rialto.LogicLayer;
using Rialto.ServiceLayer.service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Rialto.ServiceLayer.config;

namespace Rialto.ServiceLayer
{
    public class PlaidService
    {
        
        #region Protected Attributes

        protected static string PlaidServiceURL { get; set; }

        protected ManagementLogic ManagementLogic { get; set; }

        //protected static HttpSelfHostServer Server { get; set; }

        protected ILogger Logger { get; set; }

        #endregion
        
        
        #region Constructors
        //tradingCS, orderCS,plaidCredentialsLoadServiceURL,RSAPublicKeyPath , logger
        public PlaidService(string pTradingCS, string pOrderCS,string pPlaidCredentialsLoadServiceURL,string pRSAPublicKey, ILogger pLogger)
        {

            PlaidServiceURL = pPlaidCredentialsLoadServiceURL;

            ManagementLogic = new ManagementLogic(pTradingCS, pOrderCS,pRSAPublicKey, pLogger);

            Logger = pLogger;

        }

        #endregion

        
        #region Protected Methods


        protected string OnPlaidCredentialsLoad(string userIdentifier,string plaidAccessToken,string plaidItemId)
        {
            return ManagementLogic.OnPlaidCredentialsLoad( userIdentifier, plaidAccessToken, plaidItemId);
            
        }

        #endregion
        
        #region Public Methods

        public void Run()
        {
            PlaidController.OnPlaidCredentialsLoad += OnPlaidCredentialsLoad;
            //ManagementController.OnKCXOnboardingStarted += OnKCXOnboardingStarted;

            PlaidController.Logger = Logger;
            
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls(PlaidServiceURL)
                .UseStartup<PlaidStartup>()
                .Build();
                
                

            host.RunAsync();
      
            //SwaggerConfig.Register(config);
            //WebApiConfig.Register(config);
        }

        #endregion
    }
}