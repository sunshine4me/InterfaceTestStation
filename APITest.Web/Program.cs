using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace APITest.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //dotnet run --server.urls "http://localhost:5001;http://localhost:5002;http://*:5003"
            
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) {

            var config = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();

            return WebHost.CreateDefaultBuilder(args)
               .UseConfiguration(config)
               .UseStartup<Startup>()
               .Build();
        }
    }
           
}
