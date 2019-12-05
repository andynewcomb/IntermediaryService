using BusinessDomainObjects;
using IntermediaryService.Tests.HelperMockClasses;
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

        //[TestMethod]
        //public async Task Run_CatchAnyExceptionsTricklingUpToTopLevelFunctionCode()
        //{
        //    //arrange            
        //    var mockHttpRequest = new Mock<HttpRequest>();
        //    var guid = Guid.NewGuid();
        //    var mockLogger = new Mock<MockLogger>();
        //    mockLogger.Setup(l => l.LogInformation(It.IsAny<string>())).Throws(new Exception("DANGER")).Verifiable();

        //    //act
        //    var actionResult = await Function2.Run(mockHttpRequest.Object, guid, _mockLogger);

        //    //assert
        //    mockLogger.Verify();
        //    Assert.IsTrue(_mockLogger.GetLogs().Where(m => m.Contains(UserFriendlyMessages.UnhandledException)).Any());
        //    Assert.IsInstanceOfType(actionResult, typeof(StatusCodeResult));
        //    Assert.IsTrue(String.Equals(((StatusCodeResult)actionResult).StatusCode, 500));
        //    //Assert.IsNull(_cosmosDocument); //Make sure cosmos document won't be saved
        //}

        





        //private Mock<HttpRequest> CreateMockHttpRequestWithSpecifiedBody(string body)
        //{
        //    var memoryStream = new MemoryStream();
        //    var streamWriter = new StreamWriter(memoryStream);
        //    streamWriter.Write(body);
        //    streamWriter.Flush();
        //    memoryStream.Position = 0;

        //    var mockHttpRequest = new Mock<HttpRequest>();
        //    mockHttpRequest.Setup(r => r.Body).Returns(memoryStream);
        //    return mockHttpRequest;
        //}





    }



}
