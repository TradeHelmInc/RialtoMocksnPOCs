
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FerChainMock
{
    class Program
    {
        //#region Private Static Methods

        ////This should be logged, but we will write it on the screen for simplicity
        //private static void DoLog(string message)
        //{
        //    Console.WriteLine(message);
        //}

        //#endregion


        static void Main(string[] args)
        {
            //string RESTAdddress = ConfigurationManager.AppSettings["RESTAdddress"];


            //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12
            //                                      | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;


            //try
            //{
            //    FerChainServer server = new FerChainServer(RESTAdddress);
            //    server.Start();
            //    DoLog(" Service Successfully Started...");

            //}
            //catch (Exception ex)
            //{
            //    DoLog(string.Format("Critical error initializing  Service: {0}", ex.Message));
            //}

            //Console.ReadKey();

        }
    }
}
