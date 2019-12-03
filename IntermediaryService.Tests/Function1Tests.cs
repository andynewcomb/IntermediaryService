using IntermediaryService.Tests.HelperMockClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
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

        
    }

    

}
