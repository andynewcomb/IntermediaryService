using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessDomainObjects
{
    public class IntermediaryServiceDocument
    {
        public string id { get; set; }
        public Document Document { get; set; }        
        public Status Status { get; set; }
    }
}
