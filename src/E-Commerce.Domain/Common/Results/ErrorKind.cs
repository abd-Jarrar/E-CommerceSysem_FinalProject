using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Domain.Common.Results
{
    public enum ErrorKind
    {
        Failure,
        Unexpected,
        Validation,
        Conflict,
        NotFound,
        Unauthorized,
        Forbidden
    }
}
