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
    public static class Function2
    {
        [FunctionName("Function2")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "{documentId:guid}/{serviceName}")] HttpRequest req,
            [CosmosDB(databaseName:"IntermediaryServiceDb", collectionName:"IntermediaryService",
                ConnectionStringSetting = "CosmosDBConnection",Id="{documentId}", PartitionKey ="{serviceName}")]
                IntermediaryServiceDocument intermediaryServiceDocument,
            ILogger log)
        {
            try
            {
                log.LogInformation($"Function 2 processed a request");

                //validate that a document was actually retrieved from CosmosDb
                if (intermediaryServiceDocument == null)
                {                    
                    log.LogWarning(UserFriendlyMessages.DocumentNotFound, req);
                    return new NotFoundObjectResult(UserFriendlyMessages.DocumentNotFound);
                }

                //validate the string is "STARTED"
                string requestBody = new StreamReader(req.Body).ReadToEndAsync().Result;
                if (!String.Equals(requestBody,"STARTED"))
                {
                    log.LogWarning(UserFriendlyMessages.UnexpectedBodyContent, req);
                    return new BadRequestObjectResult(UserFriendlyMessages.UnexpectedBodyContent);
                }

                //Update the document
                var status = new Status()
                {
                    StatusCode = "STARTED",
                    TimeStamp = DateTime.UtcNow.ToString()
                };
                intermediaryServiceDocument.Status = status;

                return new NoContentResult();
            }            

            catch (Exception ex) //gracefully deal with an unhandled exception
            {                
                log.LogError(ex, UserFriendlyMessages.UnhandledException);
                return new StatusCodeResult(500);
            }
        }
    }
}
