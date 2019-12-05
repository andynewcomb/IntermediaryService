using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IntermediaryService.Tests.HelperClasses
{
    static class MockHttpRequestGenerator
    {
        public static Mock<HttpRequest> CreateWithBodyString(string body)
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

    }
}
