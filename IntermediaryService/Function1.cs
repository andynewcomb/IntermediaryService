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
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "request")] HttpRequest req,
            [CosmosDB(databaseName: "IntermediaryServiceDb", collectionName: "IntermediaryService", ConnectionStringSetting = "CosmosDBConnection")]out dynamic intermediaryDocument,
            ILogger log)
        {   
            try
            {   
                log.LogInformation("Function1 processed a request.");

                //retrieve the body of the request which should be of type Document
                Document document;
                string requestBody = new StreamReader(req.Body).ReadToEndAsync().Result;
                //Note: I initially used "await" - however adding CosmosDB out binding in signature meant I can't have an async method anymore.
                //I may consider using a static cosmosDb client instead of the output binding if I need/want async
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
                    intermediaryDocument = null;
                    return new BadRequestObjectResult(UserFriendlyMessages.ErrorProcessingBody);
                }

                var uniqueId = Guid.NewGuid().ToString();
                //send to third party using the injected httpClient.
                //Note: I initually used "await" - however when I added the Cosmos output binding the method could no longer be "async"
                //Perhaps will use static CosmosDb client instead if "async" is necessary.
                var success = _thirdPartyServiceHttpClient.PostAsyncSuccessful(document, uniqueId, log).Result;

                //Now Generate an "Intermediary Service Document" to be stored in CosmosDB
                //It will keep status information and the document body 
                intermediaryDocument = new IntermediaryServiceDocument
                {
                    id = uniqueId,
                    Document = document,
                    Status = new Status
                    {
                        StatusCode = "Sent",
                        Detail = "",
                        TimeStamp = DateTime.UtcNow.ToString()
                    }
                };
                
                return new NoContentResult();
            }            
            catch (Exception ex) //gracefully deal with an unhandled exception
            {
                intermediaryDocument = null;
                log.LogError(ex, UserFriendlyMessages.UnhandledException);                
                return new StatusCodeResult(500);
            }
        }






    }
}
