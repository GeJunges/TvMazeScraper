using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TvMazeScraper.Domain.Interface;
using TvMazeScraper.Domain.Model;
using TvMazeScraper.Domain.Paging;
using TvMazeScraper.Infrastructure.Settings;

namespace TvMazeScraper.Infrastructure.Repository
{
    public class ReadOnlyRepository<T> : IReadOnlyRepository<T> where T : IEntiTy
    {
        private readonly TvMazeScraperContext context;
        public ReadOnlyRepository(TvMazeScraperContext context)
        {
            this.context = context;
        }

        public async Task<T> Find(int id, Expression<Func<T, IOrderedEnumerable<Cast>>> predicate, string toInclude)
        {
            return await context.Set<T>()
                                .Include(toInclude)
                                .OrderByDescending(predicate)
                                .Where(t=>t.ApiId == id)
                                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> FindAll(Filter filter, Expression<Func<T, IOrderedEnumerable<Cast>>> predicate, string toInclude)
        {
            return await context.Set<T>()
                                .Include(toInclude)
                                .AsNoTracking()
                                .OrderBy(e => e.ApiId)
                                .ThenByDescending(t => predicate)
                                .Skip(filter.PageNumber)
                                .Take(filter.Take)
                                .ToListAsync();
        }
    }
}
