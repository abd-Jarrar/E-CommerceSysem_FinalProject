using E_Commerce.Application.Products.Dtos;
using E_Commerce.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Products.Commands.CreateProduct
{
    public sealed record CreateProductCommand(string Name,
        string Sku,
        int Stock,
        decimal Price):IRequest<Result<ProductDto>>;
    
}
