using BusinessDomainObjects;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IntermediaryService
{
    public class StubbedThirdPartyServiceHttpClient : IThirdPartyServiceHttpClient
    {
        private readonly HttpClient _httpClient;
        public StubbedThirdPartyServiceHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<bool> PostAsyncSuccessful(Document document, string callBackUrl, ILogger log)
        {
            try
            {
                var thirdPartyDto = new ThirdPartyDto(document, callBackUrl);
                var jsonString = JsonConvert.SerializeObject(thirdPartyDto);
                var result = await _httpClient.PostAsync("request", new StringContent(jsonString));

                //Check that the HttpResponseMessage returned the expected http status code (204, 200OK, etc.). 
                //Log any non-200 status codes along with the HttpRequestMessage that was sent.
                //Use polly for retries
                //To keep things simple, just throw an exception if not success status code
                result.EnsureSuccessStatusCode();

                //otherwise return success.
                return true;

            }
            catch (Exception ex)
            {
                log.LogError(ex, UserFriendlyMessages.ThirdPartyCommunicationFailure);
                throw;
            }
            
        }
    }
}
