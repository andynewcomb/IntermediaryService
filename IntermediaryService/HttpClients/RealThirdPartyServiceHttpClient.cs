using BusinessDomainObjects;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntermediaryService
{
    /// <summary>
    /// The real httpclient that would actually send requests to example.com
    /// Will not be using this for this exercise, but this is to show that we 
    /// can easily create httpclients and swap them into our intermediary service
    /// using dependency injection
    /// </summary>
    class RealThirdPartyServiceHttpClient : IThirdPartyServiceHttpClient
    {
        public Task<bool> PostAsyncSuccessful(Document document, string callBackUrl, ILogger log)
        {
            throw new NotImplementedException();
        }
    }
}


