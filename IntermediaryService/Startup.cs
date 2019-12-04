using IntermediaryService;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: FunctionsStartup(typeof(Startup))]

namespace IntermediaryService
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {            
            //register the httpclient that does *NOT* actually send requests.
            //This is injected into Function1 to simulate http requests to www.example.com/request
            builder.Services.AddHttpClient<IThirdPartyServiceHttpClient, RealThirdPartyServiceHttpClient>(client =>
            {
                client.BaseAddress = new Uri("http://www.example.com/request");
            });

            //we can register a real httpclient that does indeed send real request when 
            //the time is ready
            //builder.Services.AddHttpClient<IThirdPartyServiceHttpClient, RealThirdPartyServiceHttpClient>(client =>
            //{
            //    client.BaseAddress = new Uri("http://www.example.com");
            //});
        }
    }
}
