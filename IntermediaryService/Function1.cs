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
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                if (String.IsNullOrWhiteSpace(requestBody))
                {                    
                    return new BadRequestObjectResult("Could Not Process Body of Request");
                }

                
                try
                {
                    var data = JsonConvert.DeserializeObject<ClientDocument>(requestBody);
                }
                catch (JsonException ex)
                {
                    var errorMessage = "Could Not Process Body of Request";
                    log.LogInformation(ex, errorMessage);
                    return new BadRequestObjectResult("Could Not Process Body of Request");
                }


                return new OkObjectResult("Success");
            }            
            catch (Exception ex) //gracefully deal with an unhandled exception
            {
                log.LogError(ex, "Unhandled Exception occurred");
                return new StatusCodeResult(500);
            }
        }
    }
}
