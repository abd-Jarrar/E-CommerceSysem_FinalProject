using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Abstractions
{
    public interface IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken ct);
        public Task CommitTransactionAsync(CancellationToken ct);
        public Task BeginTransactionAsync(CancellationToken ct);
        public Task RollbackTransactionAsync(CancellationToken ct);
    }
}
