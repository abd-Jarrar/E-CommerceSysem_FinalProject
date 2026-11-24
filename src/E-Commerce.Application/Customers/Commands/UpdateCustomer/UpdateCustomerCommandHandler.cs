using E_Commerce.Application.Abstractions;
using E_Commerce.Application.Customers.Dtos;
using E_Commerce.Application.Customers.Mappers;
using E_Commerce.Domain.Common.Results;
using E_Commerce.Domain.Customers;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Customers.Commands.UpdateCustomer
{
    public sealed class UpdateCustomerCommandHandler(IUnitOfWork unitOfWork, ICustomerRepository customerRepository, ILogger<UpdateCustomerCommandHandler> logger, HybridCache cache) : IRequestHandler<UpdateCustomerCommand, Result<CustomerDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ICustomerRepository _customerRepository = customerRepository;
        private readonly ILogger<UpdateCustomerCommandHandler> _logger = logger;
        private readonly HybridCache _cache = cache;

        public async Task<Result<CustomerDto>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await customerRepository.GetByIdAsync(request.Id, cancellationToken);
            if (customer is null)
            {
                logger.LogInformation("Customer update rejected: customer {CustomerId} not found.", request.Id);
                return CustomerErrors.NotFound(request.Id);
            }

            var newEmail = (request.Email ?? string.Empty).Trim().ToLowerInvariant();

            if (newEmail != customer.Email
                && await customerRepository.EmailExistsAsync(newEmail, cancellationToken))
            {
                logger.LogInformation("Customer update rejected: email already exists.");
                return CustomerErrors.EmailExists;
            }

            var updated = customer.Update(request.Name, request.Email!);
            if (updated.IsError)
                return updated.Errors;

            await unitOfWork.SaveChangesAsync(cancellationToken);
            try
            {
                await _cache.RemoveByTagAsync("customer", cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Cache invalidation failed for customer {CustomerId}.", request.Id);
            }
            return customer.ToDto();
        }
    }
}
