﻿using fwk.Common.interfaces;
using Rialto.LogicLayer;
using Rialto.ServiceLayer.service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace Rialto.ServiceLayer
{
    public class ManagementService
    {

        #region Protected Attributes

        protected static string ManagementServiceURL { get; set; }

        protected ManagementLogic ManagementLogic { get; set; }

        protected static HttpSelfHostServer Server { get; set; }

        protected ILogger Logger { get; set; }

        #endregion

        #region Constructors

        public ManagementService(string pTradingCS, string pOrderCS, string pManagementServiceURL, string pKcxURL, string kcxKeyAndIV, bool pAESKeyEncrypted,
                                string pRASPrivateKeyPath, bool pRSAKeyEncrypted, string pSolidusURL, ILogger pLogger)
        {

            ManagementServiceURL = pManagementServiceURL;

            ManagementLogic = new ManagementLogic(pTradingCS, pOrderCS, pKcxURL, kcxKeyAndIV, pAESKeyEncrypted, pRASPrivateKeyPath, pRSAKeyEncrypted, pSolidusURL, pLogger);

            Logger = pLogger;

        }

        #endregion

        #region Protected Methods

        protected string OnKCXOnboardingApproved(string koreShareholderId, string key, string IV)
        {
            return ManagementLogic.OnKCXOnboardingApproved(koreShareholderId, key, IV);
        }

        protected string OnKCXOnboardingStarted(string koreShareholderId)
        {
            return ManagementLogic.OnKCXOnboardingStarted(koreShareholderId);
        }

        #endregion

        #region Public Methods

        public void Run()
        {

            HttpSelfHostConfiguration config = new HttpSelfHostConfiguration(ManagementServiceURL);

            //SwaggerConfig.Register(config);
            //WebApiConfig.Register(config);
          
            ManagementController.OnKCXOnboardingApproved += OnKCXOnboardingApproved;
            //ManagementController.OnKCXOnboardingStarted += OnKCXOnboardingStarted;

            ManagementController.Logger = Logger;

            config.Routes.MapHttpRoute(name: "DefaultApi",
                                       routeTemplate: "{controller}/{action}",
                                       defaults: new { id = RouteParameter.Optional });

          

            Server = new HttpSelfHostServer(config);
            Server.OpenAsync().Wait();

        }

        #endregion
    }
}
