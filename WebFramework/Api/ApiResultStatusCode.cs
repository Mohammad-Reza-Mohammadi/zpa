using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFramework.Api
{
    public enum ApiResultStatusCode
    {
        Success = 0,
        ServerError = 1,
        BadRequest = 2,
        NotFound = 3,
        ListEmpty = 4,

    }
}
