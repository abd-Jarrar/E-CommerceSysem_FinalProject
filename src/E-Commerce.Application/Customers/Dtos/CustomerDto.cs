using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Customers.Dtos
{
    public sealed record CustomerDto(string Name, string Email, bool IsVip);
    
}
