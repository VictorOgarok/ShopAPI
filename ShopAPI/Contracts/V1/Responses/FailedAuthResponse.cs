using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopAPI.Contracts.V1.Responses
{
    public class FailedAuthResponse
    {
        public IEnumerable<string> ErrorMessages { get; set; }
    }
}
