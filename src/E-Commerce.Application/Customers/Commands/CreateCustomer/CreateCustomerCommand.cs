using E_Commerce.Application.Customers.Dtos;
using E_Commerce.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Customers.Commands.CreateCustomer
{
    public sealed record CreateCustomerCommand(string Name, string Email, bool IsVip = false) : IRequest<Result<CustomerDto>>;
    
}
