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
                log.LogInformation("Function1 processed a request.");

                //retrieve the body of the request which should be a ClientDocument
                ClientDocument clientDocument;
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();                
                try
                {
                    clientDocument = JsonConvert.DeserializeObject<ClientDocument>(requestBody);
                    //Check that the clientDocument object and it's Body property are NOT null                     
                    if (clientDocument?.Body == null)
                    {
                        throw new JsonException("Object was deserialized to null");
                    }
                }
                catch (JsonException ex)
                {                    
                    log.LogInformation(ex, UserFriendlyMessages.ErrorProcessingBody,requestBody);
                    return new BadRequestObjectResult(UserFriendlyMessages.ErrorProcessingBody);
                }

                


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
