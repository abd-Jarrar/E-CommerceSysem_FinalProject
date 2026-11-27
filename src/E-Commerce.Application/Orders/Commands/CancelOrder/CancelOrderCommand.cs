using E_Commerce.Domain.Common.Results;
using E_Commerce.Domain.Orders;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Orders.Commands.CancelOrder
{
    public sealed record CancelOrderCommand(Guid OrderId) : IRequest<Result<Updated>>;
    
}
