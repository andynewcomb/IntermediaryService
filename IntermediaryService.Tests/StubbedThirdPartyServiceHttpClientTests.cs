using BusinessDomainObjects;
using IntermediaryService.Tests.HelperMockClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace IntermediaryService.Tests
{
    [TestClass]
    public class StubbedThirdPartyServiceHttpClientTests
    {
        private Mock<HttpClient> _mockHttpClient;
        private MockLogger _mockLogger;

        [TestInitialize]
        public void InitializeTest()
        {
            _mockHttpClient = new Mock<HttpClient>();
            _mockLogger = new MockLogger();
        }

        [TestMethod]
        public async Task PostAsync_ThrowsException_Logged()
        {
            //arrange
            var stubbedThirdPartyServiceHttpClient = new StubbedThirdPartyServiceHttpClient(_mockHttpClient.Object);
            var document = new Document { Body = "Some text" };
            
            //act            
            var result = await stubbedThirdPartyServiceHttpClient.PostAsync(document, "request/1",_mockLogger);

            //assert
            Assert.IsFalse(result.Success);
            Assert.IsTrue(_mockLogger.GetLogs().Any(m => m.Contains(UserFriendlyMessages.ThirdPartyCommunicationFailure)));

        }

        //test for when non-200 (or non-204) response is returned
        //Log and implement a retry strategy

    }
}
