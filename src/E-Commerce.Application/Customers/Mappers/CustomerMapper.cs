using E_Commerce.Application.Customers.Dtos;
using E_Commerce.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Customers.Mappers
{
    public static class CustomerMapper
    {
        extension(Customer customer)
        {
            public CustomerDto ToDto()
            {
                ArgumentNullException.ThrowIfNull(customer); 
                return new CustomerDto(customer.Name, customer.Email, customer.IsVip);
            }
        }
        extension(IEnumerable<Customer> customers)
        {
            public List<CustomerDto> ToDtos()
            {
                return customers.Select(c => c.ToDto()).ToList();
            } 
        }
    }
}
