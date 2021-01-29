using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

namespace TestSelfHost2
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
    
    class Program
    {
        static void Main(string[] args)
        {
            

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}