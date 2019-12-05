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
    public class Function1Tests
    {

        private Mock<IThirdPartyServiceHttpClient> _mockHttpClient;        
        private Function1 _function1; 
        private MockLogger _mockLogger;
        private dynamic _cosmosDocument;

        [TestInitialize]
        public void InitializeTest()
        {
            _mockHttpClient = new Mock<IThirdPartyServiceHttpClient>();
            _function1 = new Function1(_mockHttpClient.Object);
            _mockLogger = new MockLogger();
        }
        


        [TestMethod]
        public async Task Run_CatchAnyExceptionsTricklingUpToTopLevelFunctionCode()
        {
            //arrange            
            var mockHttpRequest = new Mock<HttpRequest>();

            //act
            var actionResult = _function1.Run(mockHttpRequest.Object, out _cosmosDocument, _mockLogger);

            //assert                        
            Assert.IsTrue(_mockLogger.GetLogs().Where(m=>m.Contains(UserFriendlyMessages.UnhandledException)).Any());
            Assert.IsInstanceOfType(actionResult, typeof(StatusCodeResult));
            Assert.IsTrue(String.Equals(((StatusCodeResult)actionResult).StatusCode, 500));
            Assert.IsNull(_cosmosDocument); //Make sure cosmos document won't be saved
        }

        [TestMethod]
        public async Task Run_NotPost_Return400()
        {        
            //we may want to setup a friendlier response rather than the default, blank 404 error
            //that Azure functions returns when it doesn't recognize the HTTP Method
        }

        [TestMethod]
        public async Task Run_NoBodyInRequest_Return400()
        {
            //arrange
            var mockHttpRequest = MockHttpRequestGenerator.CreateWithBodyString("");                                    

            //act
            var actionResult = _function1.Run(mockHttpRequest.Object, out _cosmosDocument, _mockLogger);

            //assert                                    
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestObjectResult));
            StringAssert.Contains(((BadRequestObjectResult)actionResult).Value.ToString(), UserFriendlyMessages.ErrorProcessingBody);
            Assert.IsNull(_cosmosDocument); //Make sure cosmos document won't be saved
        }

        [DataTestMethod]        
        [DataRow("blabla")]        
        [DataRow("%&*")]
        public async Task Run_MalformedBodyInRequest_Return400(string body)
        {
            //arrange            
            var mockHttpRequest = MockHttpRequestGenerator.CreateWithBodyString(body);            

            //act
            var actionResult = _function1.Run(mockHttpRequest.Object, out _cosmosDocument, _mockLogger);

            //assert 
            Assert.IsTrue(_mockLogger.GetLogs().Where(m => m.Contains(UserFriendlyMessages.ErrorProcessingBody)).Count() == 1);
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestObjectResult));
            StringAssert.Contains(((BadRequestObjectResult)actionResult).Value.ToString(), UserFriendlyMessages.ErrorProcessingBody);
            Assert.IsNull(_cosmosDocument); //Make sure cosmos document won't be saved
        }

        [DataTestMethod]
        [DataRow("{}")]
        [DataRow("{\"buddy\":\"some value\"}")]
        public async Task Run_JsonObjectDeserialized_BodyPropertyNull_Return400(string body)
        {
            //arrange            
            var mockHttpRequest = MockHttpRequestGenerator.CreateWithBodyString(body);            

            //act
            var actionResult = _function1.Run(mockHttpRequest.Object, out _cosmosDocument, _mockLogger);

            //assert 
            Assert.IsTrue(_mockLogger.GetLogs().Where(m => m.Contains(UserFriendlyMessages.ErrorProcessingBody)).Any());
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestObjectResult));
            StringAssert.Contains(((BadRequestObjectResult)actionResult).Value.ToString(), UserFriendlyMessages.ErrorProcessingBody);
            Assert.IsNull(_cosmosDocument); //Make sure cosmos document won't be saved
        }

        [TestMethod]
        public async Task Run_SendsHttpRequestToExampleDotCom_CosmosDocumentPopulated()
        {
            //arrange            
            var goodJson = "{\"body\":\"Some Text\"}";
            var mockHttpRequest = MockHttpRequestGenerator.CreateWithBodyString(goodJson);
            _mockHttpClient.Setup(c => c.PostAsyncSuccessful(It.IsAny<Document>(), It.IsAny<string>(),It.IsAny<ILogger>())).Returns(Task.FromResult(true)).Verifiable();

            //act
            var actionResult = _function1.Run(mockHttpRequest.Object, out _cosmosDocument, _mockLogger);

            //assert
            _mockHttpClient.Verify();
            Assert.IsInstanceOfType(actionResult, typeof(OkObjectResult));
            Assert.IsInstanceOfType(_cosmosDocument, typeof(IntermediaryServiceDocument));
        }

        [TestMethod]
        public async Task Run_GoodRequest_ReturnsUniqueId()
        {
            //arrange
            var document = new Document() { Body = "Some Text" };
            var documentAsJson = JsonConvert.SerializeObject(document);
            var mockHttpRequest = MockHttpRequestGenerator.CreateWithBodyString(documentAsJson);
            _mockHttpClient.Setup(c => c.PostAsyncSuccessful(It.IsAny<Document>(), It.IsAny<string>(), It.IsAny<ILogger>())).Returns(Task.FromResult(true)).Verifiable();

            //act
            var actionResult = _function1.Run(mockHttpRequest.Object, out _cosmosDocument, _mockLogger);

            //assert
            _mockHttpClient.Verify();
            Assert.IsInstanceOfType(actionResult, typeof(OkObjectResult));
            Assert.IsInstanceOfType(_cosmosDocument, typeof(IntermediaryServiceDocument));
            var returnValue = ((OkObjectResult)actionResult).Value;

            Guid guidOutput;
            Assert.IsTrue(Guid.TryParse((string)returnValue, out guidOutput));
        }


        //test for content-type json (content negotiation if desired)

    }

    

}
