using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

namespace Rialto.ServiceLayer.config
{
    public class Startup{

        public  void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }
        
        public  void Configure(IApplicationBuilder app){
            app.Run(context => {
                
                return context.Response.WriteAsync("Hello world");
            });
            
            app.UseMvc();
        }
    }
    
    
}