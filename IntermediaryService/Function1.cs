using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using BusinessDomainObjects;

namespace IntermediaryService
{
    public class Function1
    {
        private readonly IThirdPartyServiceHttpClient _thirdPartyServiceHttpClient;

        public Function1(IThirdPartyServiceHttpClient thirdPartyServiceHttpClient)
        {
            _thirdPartyServiceHttpClient = thirdPartyServiceHttpClient;
        }

        [FunctionName("Function1")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "intermediaryservice/request")] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("Function1 processed a request.");

                //retrieve the body of the request which should be of type Document
                Document document;
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();                
                try
                {
                    document = JsonConvert.DeserializeObject<Document>(requestBody);
                    //Check that the clientDocument object and it's Body property are NOT null                     
                    if (document?.Body == null)
                    {
                        throw new JsonException("Object was deserialized to null");
                    }
                }
                catch (JsonException ex)
                {                    
                    log.LogInformation(ex, UserFriendlyMessages.ErrorProcessingBody,requestBody);
                    return new BadRequestObjectResult(UserFriendlyMessages.ErrorProcessingBody);
                }


                //Generate an "Intermediary Service Document" to be stored in CosmosDB
                //It will contain a unique identifier that will also be used for the callback URL
                //var callBackUrl = StorageM()



                //send to third party using the injected httpClient.                 
                var success = await _thirdPartyServiceHttpClient.PostAsyncSuccessful(document, "request", log);
                                
                return new OkObjectResult("Success");
            }            
            catch (Exception ex) //gracefully deal with an unhandled exception
            {
                log.LogError(ex, UserFriendlyMessages.UnhandledException);                
                return new StatusCodeResult(500);
            }
        }






    }
}
