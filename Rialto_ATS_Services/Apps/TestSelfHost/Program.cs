using System;
using System.Web.Http;
using Owin;
using Microsoft.Owin.Hosting;

using System.Net.Http;

namespace TestSelfHost
{
    
    
    public class Startup
    {
        Type demoControllerType = typeof(TestSelfHost.DemoController);
        
        public void Configuration(IAppBuilder app)
        {
            // Configure Web API for self-host. 
            var config = new HttpConfiguration();
            
            //  Enable attribute based routing
            //  http://www.asp.net/web-api/overview/web-api-routing-and-actions/attribute-routing-in-web-api-2
            config.MapHttpAttributeRoutes();
            
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            
            app.UseWebApi(config); 
        }
    }
    
    public class Program
    {
      
        
        static void Main(string[] args)
        {
            using (WebApp.Start<Startup>("http://localhost:5000"))
            {
                Console.WriteLine("Web Server is running.");
                Console.WriteLine("Press any key to quit.");
                
                Console.ReadLine();
            }
        }
    }
}