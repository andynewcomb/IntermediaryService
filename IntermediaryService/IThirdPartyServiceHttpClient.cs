using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntermediaryService
{
    public interface IThirdPartyServiceHttpClient
    {
        Task<string> PostAsync();
    }
}
