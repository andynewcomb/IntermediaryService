using BusinessDomainObjects;
using IntermediaryService.Tests.HelperClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IntermediaryService.Tests
{
    [TestClass]
    public class Function2Tests
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
        public async Task Run_BodyDoesNotContainSTARTED_Return400BadRequest(string body)
        {
            //arrange            
            var mockHttpRequest = MockHttpRequestGenerator.CreateWithBodyString(body);

            //act            
            var actionResult = await Function2.Run(mockHttpRequest.Object, new IntermediaryServiceDocument(), _mockLogger);

            //assert 
            Assert.IsTrue(_mockLogger.GetLogs().Where(m => m.Contains(UserFriendlyMessages.UnexpectedBodyContent)).Count() == 1);
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestObjectResult));
            StringAssert.Contains(((BadRequestObjectResult)actionResult).Value.ToString(), UserFriendlyMessages.UnexpectedBodyContent);
        }

        [TestMethod]
        public async Task Run_CosmosDbReturnsNoDocument_Return404NotFound()
        {
            //arrange            
            var mockHttpRequest = MockHttpRequestGenerator.CreateWithBodyString("STARTED");

            //act            
            var actionResult = await Function2.Run(mockHttpRequest.Object, null, _mockLogger);

            //assert 
            Assert.IsTrue(_mockLogger.GetLogs().Where(m => m.Contains(UserFriendlyMessages.DocumentNotFound)).Count() == 1);
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundObjectResult));
            StringAssert.Contains(((NotFoundObjectResult)actionResult).Value.ToString(), UserFriendlyMessages.DocumentNotFound);
        }
    }



}
