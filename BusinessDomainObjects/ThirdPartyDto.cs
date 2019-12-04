using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessDomainObjects
{
    public class ThirdPartyDto
    {
        public ThirdPartyDto(Document document, string callBackUrl)
        {
            body = document.Body;
            callback = callBackUrl;
        }

        public string body { get; set; }
        public string callback { get; set; }
    }
}
