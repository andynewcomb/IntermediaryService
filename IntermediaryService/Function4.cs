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
    public static class Function4
    {
        [FunctionName("Function4")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "status/{documentId:guid}")] HttpRequest req,
            [CosmosDB(databaseName:"IntermediaryServiceDb", collectionName:"IntermediaryService",
                ConnectionStringSetting = "CosmosDBConnection",SqlQuery ="select * from IntermediaryService s where s.id = {documentId}")]
                IEnumerable<IntermediaryServiceDocument> documents,
            ILogger log)
        {
            try
            {
                log.LogInformation($"Function 4 processed a request");

                var intermediaryServiceDocument = documents?.SingleOrDefault();
                //validate that a document was actually retrieved from CosmosDb
                if (intermediaryServiceDocument == null)
                {
                    log.LogWarning(UserFriendlyMessages.DocumentNotFound, req);
                    return new NotFoundObjectResult(UserFriendlyMessages.DocumentNotFound);
                }

                return new OkObjectResult(intermediaryServiceDocument);
            }

            catch (Exception ex) //gracefully deal with an unhandled exception
            {
                log.LogError(ex, UserFriendlyMessages.UnhandledException);
                return new StatusCodeResult(500);
            }
        }
    }
}
