using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntermediaryService
{
    public class ThirdPartyServiceHttpClient : IThirdPartyServiceHttpClient
    {
        public Task<string> PostAsync()
        {
            throw new NotImplementedException();
        }
    }
}
