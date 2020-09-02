using System;
using System.Threading.Tasks;

namespace Brutus.Core
{
    public interface IRepository<T> where T: class, IAggregate, new()
    {
        Task<T> Find(Guid id);
        Task Add(T aggregate);
        Task Update(T aggregate);
    }
}