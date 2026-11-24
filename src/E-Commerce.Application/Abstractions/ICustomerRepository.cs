using E_Commerce.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Abstractions
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct);
        Task AddAsync(Customer customer,CancellationToken ct);
        Task<bool> EmailExistsAsync(string email, CancellationToken ct);
    }
}
