using BusinessDomainObjects;
using IntermediaryService.Tests.HelperClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IntermediaryService.Tests
{
    [TestClass]
    public class Function3Tests
    {   
        private MockLogger _mockLogger;
        private dynamic _cosmosDocument;

        [TestInitialize]
        public void InitializeTest()
        {               
            _mockLogger = new MockLogger();
        }

        [DataTestMethod]
        [DataRow("blabla")]
        [DataRow("%&*")]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("Started")]
        public async Task Run_BodyDoesNotContainOneOfTheThree_Return400BadRequest(string body)
        {
            //arrange            
            var mockHttpRequest = MockHttpRequestGenerator.CreateWithBodyString(body);

            //act            
            var actionResult = await Function3.Run(mockHttpRequest.Object, new IntermediaryServiceDocument(), _mockLogger);

            //assert 
            Assert.IsTrue(_mockLogger.GetLogs().Where(m => m.Contains(UserFriendlyMessages.ErrorProcessingBody)).Count() == 1);
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestObjectResult));
            StringAssert.Contains(((BadRequestObjectResult)actionResult).Value.ToString(), UserFriendlyMessages.ErrorProcessingBody);
        }

        [TestMethod]
        public async Task Run_CosmosDbReturnsNoDocument_Return404NotFound()
        {
            //arrange            
            var mockHttpRequest = MockHttpRequestGenerator.CreateWithBodyString("COMPLETED");

            //act            
            var actionResult = await Function3.Run(mockHttpRequest.Object, null, _mockLogger);

            //assert 
            Assert.IsTrue(_mockLogger.GetLogs().Where(m => m.Contains(UserFriendlyMessages.DocumentNotFound)).Count() == 1);
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundObjectResult));
            StringAssert.Contains(((NotFoundObjectResult)actionResult).Value.ToString(), UserFriendlyMessages.DocumentNotFound);
        }

        [DataTestMethod]
        [DataRow("COMPLETED")]
        [DataRow("PROCESSED")]
        [DataRow("ERROR")]
        public async Task Run_StatusCompleted_CosmosDbDocumentUpdated_Return204Success(string status)
        {
            //arrange            
            var thirdPartyStatus = new ThirdPartyStatus()
            {
                Detail = "",
                Status = status
            };
            string json = JsonConvert.SerializeObject(thirdPartyStatus);
            var mockHttpRequest = MockHttpRequestGenerator.CreateWithBodyString(json);
            var intermediaryDocument = new IntermediaryServiceDocument();

            //act            
            var actionResult = await Function3.Run(mockHttpRequest.Object, intermediaryDocument, _mockLogger);

            //assert             
            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));
            Assert.IsTrue(string.Equals(status, intermediaryDocument.Status.StatusCode));
        }
    }



}
