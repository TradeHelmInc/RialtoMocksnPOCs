using System;
using System.IO;
using System.Text;
using fwk.Common.interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Rialto.Common.DTO.Generic;
using Rialto.Common.DTO.Services;
using Rialto.ServiceLayer.service;


namespace Rialto.ServiceLayer.config
{
  public class ManagementStartup{
        
        #region Private Static Consts

        private string _ONBARDING_4096BITS_URL = "/Management/OnKCXOnboardingApproved_4096";
        
        #endregion
      
        #region Protected Attributes
        protected ManagementController Management { get; set; }
        
        #endregion
        
        public void ConfigureServices(IServiceCollection services)
        {
            Management = new ManagementController();
            services.AddMvc();
        }
        
        public void Configure(IApplicationBuilder app){
            app.Run(context =>
            {
                if (context.Request.Path.Value == _ONBARDING_4096BITS_URL)
                    return context.Response.WriteAsync(Management.Post(context.Request));
                else
                    return context.Response.WriteAsync(
                        Management.CreateStrTransactionError(string.Format("Unknown URL {0}", context.Request.Path)));
            });
            
            app.UseMvc();
        }
    }
}