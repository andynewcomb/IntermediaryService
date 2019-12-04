using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessDomainObjects
{
    public class IntermediaryServiceDocument
    {
        public Document Document { get; set; }
        public string CallBackUrl { get; set; }
        public Status Status { get; set; }
    }
}
