using E_Commerce.Application.Customers.Dtos;
using E_Commerce.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Customers.Queries
{
    public sealed record GetCustomerQuery(Guid Id) : IRequest<Result<CustomerDto>>;
    
}
