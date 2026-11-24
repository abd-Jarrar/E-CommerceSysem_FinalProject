using E_Commerce.Application.Abstractions;
using E_Commerce.Application.Customers.Dtos;
using E_Commerce.Application.Customers.Mappers;
using E_Commerce.Domain.Common.Results;
using E_Commerce.Domain.Customers;
using MediatR;
using Microsoft.Extensions.Caching;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Customers.Commands.CreateCustomer
{
    public sealed class CreateCustomerCommandHandler(IUnitOfWork unitOfWork,ICustomerRepository customerRepository, ILogger<CreateCustomerCommandHandler> logger, HybridCache cache)
        : IRequestHandler<CreateCustomerCommand, Result<CustomerDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ICustomerRepository _customerRepository = customerRepository;
        private readonly ILogger<CreateCustomerCommandHandler> _logger = logger;
        private readonly HybridCache _cache = cache;
        public async Task<Result<CustomerDto>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            var email = (request.Email ?? string.Empty).Trim().ToLowerInvariant();
            if (await _customerRepository.EmailExistsAsync(email, cancellationToken))
            {
                _logger.LogWarning("Customer creation aborted. Email already exists.");
                return CustomerErrors.EmailExists;
            }
            var createCustomerResult = Customer.Create(request.Name.Trim(), request.Email!.Trim(), request.IsVip);
            if (createCustomerResult.IsError)
            {
                return createCustomerResult.Errors;
            }
            await _customerRepository.AddAsync(createCustomerResult.Value, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            var customer = createCustomerResult.Value;
            _logger.LogInformation("Customer created successfully. Id: {customer.Id}", customer.Id);
            await _cache.RemoveByTagAsync("customer", cancellationToken);
            return customer.ToDto();
        }
    }
}
