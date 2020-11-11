using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using Marten.Pagination;

namespace Brutus.User
{
    public class QueryHandler
    {
        private readonly IDocumentSession _session;

        public QueryHandler(IDocumentSession session)
        {
            _session = session;
        }
        
        public async Task<ReadModels.User> Query(Queries.GetUserById query)
        {
            return await _session
                .Query<Domain.User>()
                .Where(user => user.Id == query.UserId)
                .Select(usr => new ReadModels.User(usr.Id, usr.FirstName, usr.LastName, usr.Email, usr.Status))
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ReadModels.User>> Query(Queries.GetAllUsers query)
        {
            return await _session
                .Query<Domain.User>()
                .Select(usr => new ReadModels.User(usr.Id, usr.FirstName, usr.LastName, usr.Email, usr.Status))
                .ToPagedListAsync(query.Page, query.PageSize);
        }
    }
}