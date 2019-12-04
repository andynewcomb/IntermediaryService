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
            Assert.IsTrue(mockLogger.GetLogs().Where(m=>m.Contains("Unhandled Exception occurred")).Count() == 1);
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
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            streamWriter.Write("");
            streamWriter.Flush();
            memoryStream.Position = 0;
            var mockHttpRequest = new Mock<HttpRequest>();
            mockHttpRequest.Setup(r => r.Body).Returns(memoryStream);
            var mockLogger = new MockLogger();

            //act
            var actionResult = await Function1.Run(mockHttpRequest.Object, mockLogger);

            //assert                                    
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestObjectResult));
            StringAssert.Contains(((BadRequestObjectResult)actionResult).Value.ToString(), "Could Not Process Body of Request");
        }

        //test for content-type json

        
    }

    

}
