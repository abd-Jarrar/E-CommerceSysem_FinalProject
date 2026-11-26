using E_Commerce.Application.Products.Dtos;
using E_Commerce.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Products.Commands.UpdateProduct
{
    public record UpdateProductCommand(Guid Id, string Name, decimal Price) : IRequest<Result<ProductDto>>;
    
}
