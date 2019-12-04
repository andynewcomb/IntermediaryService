using IntermediaryService.Tests.HelperMockClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IntermediaryService.Tests
{
    [TestClass]
    public class Function1Tests
    {

        

        [TestMethod]
        public async Task Run_CatchAnyExceptionsTricklingUpToTopLevelFunctionCode()
        {
            //arrange            
            var mockHttpRequest = new Mock<HttpRequest>();
            var mockLogger = new MockLogger();

            //act
            var actionResult = await Function1.Run(mockHttpRequest.Object, mockLogger);


            //assert                        
            Assert.IsTrue(mockLogger.GetLogs().Where(m=>m.Contains(UserFriendlyMessages.UnhandledException)).Any());
            Assert.IsInstanceOfType(actionResult, typeof(StatusCodeResult));
            Assert.IsTrue(String.Equals(((StatusCodeResult)actionResult).StatusCode, 500));
            
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
            var mockHttpRequest = CreateMockHttpRequestWithSpecifiedBody("");                        
            var mockLogger = new MockLogger();

            //act
            var actionResult = await Function1.Run(mockHttpRequest.Object, mockLogger);

            //assert                                    
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestObjectResult));
            StringAssert.Contains(((BadRequestObjectResult)actionResult).Value.ToString(), UserFriendlyMessages.ErrorProcessingBody);
        }

        [DataTestMethod]        
        [DataRow("blabla")]        
        [DataRow("%&*")]
        public async Task Run_MalformedBodyInRequest_Return400(string body)
        {
            //arrange            
            var mockHttpRequest = CreateMockHttpRequestWithSpecifiedBody(body);
            var mockLogger = new MockLogger();

            //act
            var actionResult = await Function1.Run(mockHttpRequest.Object, mockLogger);

            //assert 
            Assert.IsTrue(mockLogger.GetLogs().Where(m => m.Contains(UserFriendlyMessages.ErrorProcessingBody)).Count() == 1);
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestObjectResult));
            StringAssert.Contains(((BadRequestObjectResult)actionResult).Value.ToString(), UserFriendlyMessages.ErrorProcessingBody);
        }

        [DataTestMethod]
        [DataRow("{}")]
        [DataRow("{\"buddy\":\"some value\"}")]
        public async Task Run_JsonObjectDeserialized_BodyPropertyNull_Return400(string body)
        {
            //arrange            
            var mockHttpRequest = CreateMockHttpRequestWithSpecifiedBody(body);
            var mockLogger = new MockLogger();

            //act
            var actionResult = await Function1.Run(mockHttpRequest.Object, mockLogger);

            //assert 
            Assert.IsTrue(mockLogger.GetLogs().Where(m => m.Contains(UserFriendlyMessages.ErrorProcessingBody)).Any());
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestObjectResult));
            StringAssert.Contains(((BadRequestObjectResult)actionResult).Value.ToString(), UserFriendlyMessages.ErrorProcessingBody);
        }

        private Mock<HttpRequest> CreateMockHttpRequestWithSpecifiedBody(string body)
        {            
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            streamWriter.Write(body);
            streamWriter.Flush();
            memoryStream.Position = 0;
            
            var mockHttpRequest = new Mock<HttpRequest>();
            mockHttpRequest.Setup(r => r.Body).Returns(memoryStream);
            return mockHttpRequest;
        }       
        
        //test for content-type json
    }

    

}
