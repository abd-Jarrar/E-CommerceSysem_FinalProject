using E_Commerce.Application.Abstractions;
using E_Commerce.Application.Customers.Commands.UpdateCustomer;
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

namespace E_Commerce.Application.Customers.Queries
{
    public sealed class GetCustomerQueryHandler(ICustomerRepository customerRepository, ILogger<GetCustomerQueryHandler> logger, HybridCache cache) : IRequestHandler<GetCustomerQuery, Result<CustomerDto>>
    {
        private readonly ICustomerRepository _customerRepository = customerRepository;
        private readonly ILogger<GetCustomerQueryHandler> _logger = logger;
        private readonly HybridCache _cache = cache;
        public async Task<Result<CustomerDto>> Handle(GetCustomerQuery request,CancellationToken cancellationToken)
        {
            var key = $"customer:{request.Id}";
            var isMiss = false;

            try
            {
                var dto = await _cache.GetOrCreateAsync(
                    key,
                    async ct =>
                    {
                        isMiss = true;

                        var customer = await _customerRepository
                            .GetByIdAsync(request.Id, ct);

                        if (customer is null)
                        {
                            throw new Exception("Customer not found.");
                        }

                        return customer.ToDto();
                    },
                    new HybridCacheEntryOptions
                    {
                        Expiration = TimeSpan.FromMinutes(5)
                    },
                    [key],
                    cancellationToken);

                if (isMiss)
                {
                    _logger.LogInformation("Cache MISS for {CacheKey}.", key);
                }
                else
                {
                    _logger.LogInformation("Cache HIT for {CacheKey}.", key);
                }

                return dto;
            }
            catch (Exception ex) when (ex.Message == "Customer not found.")
            {
                _logger.LogInformation("Cache MISS for {CacheKey}.", key);
                return CustomerErrors.NotFound(request.Id);
            }
        }
    }
}
