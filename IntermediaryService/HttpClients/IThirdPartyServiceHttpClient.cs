using BusinessDomainObjects;
using IntermediaryService.HttpClients;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntermediaryService
{
    public interface IThirdPartyServiceHttpClient
    {
        Task<bool> PostAsyncSuccessful(Document document, string callBackUrl, ILogger log);
    }
}
