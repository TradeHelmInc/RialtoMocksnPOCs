using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Rialto.ServiceLayer.service;

namespace Rialto.ServiceLayer.config
{
    public class PlaidStartup
    {
        #region Private Static Consts

        private string _ON_PLAID_CREDENTIALS_LOAD_URL = "/Plaid/OnPlaidCredentialsLoad";
        
        #endregion
      
        #region Protected Attributes
        protected PlaidController Plaid { get; set; }
        
        #endregion
        
        public void ConfigureServices(IServiceCollection services)
        {
            Plaid = new PlaidController();
            services.AddMvc();
        }
        
        public void Configure(IApplicationBuilder app){
            app.Run(context =>
            {
                if (context.Request.Path.Value == _ON_PLAID_CREDENTIALS_LOAD_URL)
                    return context.Response.WriteAsync(Plaid.Post(context.Request));
                else
                    return context.Response.WriteAsync(
                        Plaid.CreateStrTransactionError(string.Format("Unknown URL {0}", context.Request.Path)));
            });
            
            app.UseMvc();
        }
    }
}