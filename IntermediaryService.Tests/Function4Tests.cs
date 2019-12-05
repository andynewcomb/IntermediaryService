using BusinessDomainObjects;
using IntermediaryService.Tests.HelperClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IntermediaryService.Tests
{
    [TestClass]
    public class Function4Tests
    {   
        private MockLogger _mockLogger;
        private dynamic _cosmosDocument;
        private Mock<HttpRequest> _mockHttpRequest;

        [TestInitialize]
        public void InitializeTest()
        {               
            _mockLogger = new MockLogger();
            _mockHttpRequest = new Mock<HttpRequest>();
        }

        [TestMethod]
        public async Task Run_CosmosDbReturnsNullCollectionOfDocuments_Return404NotFound()
        {
            //arrange            
            List<IntermediaryServiceDocument> docs = null;

            //act            
            var actionResult = await Function4.Run(_mockHttpRequest.Object, docs, _mockLogger);

            //assert 
            Assert.IsTrue(_mockLogger.GetLogs().Where(m => m.Contains(UserFriendlyMessages.DocumentNotFound)).Count() == 1);
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundObjectResult));
            StringAssert.Contains(((NotFoundObjectResult)actionResult).Value.ToString(), UserFriendlyMessages.DocumentNotFound);
        }

        [TestMethod]
        public async Task Run_CosmosDbReturnsEmptyCollectionOfDocuments_Return404NotFound()
        {
            //arrange            
            var docs = new List<IntermediaryServiceDocument>();

            //act            
            var actionResult = await Function4.Run(_mockHttpRequest.Object, docs, _mockLogger);

            //assert 
            Assert.IsTrue(_mockLogger.GetLogs().Where(m => m.Contains(UserFriendlyMessages.DocumentNotFound)).Count() == 1);
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundObjectResult));
            StringAssert.Contains(((NotFoundObjectResult)actionResult).Value.ToString(), UserFriendlyMessages.DocumentNotFound);
        }

        [TestMethod]
        public async Task Run_DocumentFound_Return200OKSuccessWithDocument()
        {
            //arrange   
            var documentId = Guid.NewGuid().ToString();
            List<IntermediaryServiceDocument> docs = new List<IntermediaryServiceDocument>()
            {
                    new IntermediaryServiceDocument()
                    {
                        id = documentId   
                    }
            };

            //act            
            var actionResult = await Function4.Run(_mockHttpRequest.Object, docs, _mockLogger);            

            //assert             
            Assert.IsInstanceOfType(actionResult, typeof(OkObjectResult));
            Assert.IsTrue(string.Equals(documentId, ((IntermediaryServiceDocument)((OkObjectResult)actionResult).Value).id));
        }

    }



}
