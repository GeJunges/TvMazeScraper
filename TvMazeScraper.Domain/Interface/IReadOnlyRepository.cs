using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TvMazeScraper.Domain.Model;
using TvMazeScraper.Domain.Paging;

namespace TvMazeScraper.Domain.Interface
{
    public interface IReadOnlyRepository<T> where T  : IEntiTy
    {
        Task<T> Find(int id, Expression<Func<T, IOrderedEnumerable<Cast>>> predicate, string toInclude);
        Task<IEnumerable<T>> FindAll(Filter filter, Expression<Func<T, IOrderedEnumerable<Cast>>> predicate, string toInclude);
    }
}
