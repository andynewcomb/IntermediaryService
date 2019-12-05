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
using System.Collections.Generic;
using System.Linq;

namespace IntermediaryService
{
    public static class Function3
    {
        [FunctionName("Function3")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "{documentId:guid}/{serviceName}")] HttpRequest req,
            [CosmosDB(databaseName:"IntermediaryServiceDb", collectionName:"IntermediaryService",
                ConnectionStringSetting = "CosmosDBConnection",Id="{documentId}", PartitionKey ="{serviceName}")]
                IntermediaryServiceDocument intermediaryServiceDocument,
            ILogger log)
        {
            try
            {
                log.LogInformation($"Function 3 processed a request");

                //validate that a document was actually retrieved from CosmosDb
                if (intermediaryServiceDocument == null)
                {
                    log.LogWarning(UserFriendlyMessages.DocumentNotFound, req);
                    return new NotFoundObjectResult(UserFriendlyMessages.DocumentNotFound);
                }

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                ThirdPartyStatus thirdPartyStatus;
                try
                {
                    thirdPartyStatus = JsonConvert.DeserializeObject<ThirdPartyStatus>(requestBody);
                    //Check that the clientDocument object and it's Body property are NOT null                     
                    if (thirdPartyStatus?.Status == null)
                    {
                        throw new JsonException("Object was deserialized to null");
                    }
                }
                catch (JsonException ex)
                {
                    log.LogInformation(ex, UserFriendlyMessages.ErrorProcessingBody, requestBody);
                    return new BadRequestObjectResult(UserFriendlyMessages.ErrorProcessingBody);
                }

                //Check that the status is one of the three
                var statusOptions = new List<string>() { "PROCESSED", "COMPLETED", "ERROR" };
                if (!statusOptions.Any(s => String.Equals(s, thirdPartyStatus.Status)))
                {
                    var logMessage = $"Expected PROCESSED, COMPLETED, ERROR but received {thirdPartyStatus.Status}";
                    log.LogWarning(logMessage, requestBody);
                    return new BadRequestObjectResult(UserFriendlyMessages.UnexpectedBodyContent);
                }

                //Update the document                
                intermediaryServiceDocument.Status = new Status(thirdPartyStatus);

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
