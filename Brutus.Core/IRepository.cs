using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Brutus.Core
{
    public interface IRepository<T> where T: Aggregate
    {
        Task<T> FindAsync(Guid id);
        Task<ICollection<object>> AddAsync(T aggregate);
        Task<ICollection<object>> UpdateAsync(T aggregate);
    }
}