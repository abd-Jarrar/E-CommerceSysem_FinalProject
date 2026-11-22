using E_Commerce.Domain.Common;
using E_Commerce.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace E_Commerce.Domain.Customers
{
    public sealed class Customer:Entity
    {
        public string Name { get; private set; } = default!;
        public string Email { get; private set; } = default!;
        public bool IsVip { get; private set; }
        private Customer() { }

        private static readonly Regex EmailRegex = new(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant,
            TimeSpan.FromMilliseconds(100));
        private Customer(Guid id, string name, string email, bool isVip)
       : base(id)
        {
            Name = name;
            Email = email;
            IsVip = isVip;
        }
        private static List<Error> Validate(string name, string email)
        {
            var errors = new List<Error>();
            if (string.IsNullOrWhiteSpace(name))
                errors.Add(CustomerErrors.NameRequired);
            if (string.IsNullOrWhiteSpace(email))
                errors.Add(CustomerErrors.EmailRequired);
            else if (!EmailRegex.IsMatch(email.Trim()))
                errors.Add(CustomerErrors.EmailInvalid);

            return errors;
        }
        public static Result<Customer> Create(string name, string email, bool isVip = false)
        {

            var errors=Validate(name, email);
            if (errors.Count > 0)
                return errors;
            else
                return new Customer(Guid.NewGuid(),name.Trim(), email.Trim().ToLowerInvariant(), isVip);
        }
        public Result<Updated> MarkAsVip()
        {
            IsVip = true;
            return Result.Updated;
        }
        public Result<Updated> RemoveVip()
        {
            IsVip = false;
            return Result.Updated;
        }
        public Result<Updated> Update(string name, string email)
        {
            var errors = Validate(name, email);
            if (errors.Count > 0)
                return errors;
            Name = name.Trim(); 
            Email = email.Trim().ToLowerInvariant();
            return Result.Updated;
        }
    }
}
