using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Products.Dtos
{
    public sealed record ProductDto(Guid Id,
        string Name, 
        string SKU,
        int Quantity,
        decimal Price,
        bool IsActive);
    
}
