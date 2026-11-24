using E_Commerce.Application.Customers.Dtos;
using E_Commerce.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Customers.Commands.UpdateCustomer
{
    public sealed record UpdateCustomerCommand(Guid Id,string Name, string Email, bool IsVip) : IRequest<Result<CustomerDto>>;
}
