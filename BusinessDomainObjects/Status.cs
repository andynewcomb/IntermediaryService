using System;

namespace BusinessDomainObjects
{
    public class Status
    {
        public Status() {}

        public Status(ThirdPartyStatus thirdPartyStatus)
        {
            StatusCode = thirdPartyStatus.Status;
            Detail = thirdPartyStatus.Detail;
            TimeStamp = DateTime.UtcNow.ToLongTimeString();
        }

        public string StatusCode { get; set; }
        public string Detail { get; set; }
        public string TimeStamp { get; set; }
    }
}