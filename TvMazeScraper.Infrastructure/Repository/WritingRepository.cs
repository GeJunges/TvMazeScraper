using System.Collections.Generic;
using System.Threading.Tasks;
using TvMazeScraper.Domain.Interface;
using TvMazeScraper.Domain.Model;
using TvMazeScraper.Infrastructure.Settings;

namespace TvMazeScraper.Infrastructure.Repository
{
    public class WritingRepository<T> : IWritingRepository<T> where T : IEntiTy
    {
        private readonly TvMazeScraperContext context;

        public WritingRepository(TvMazeScraperContext context)
        {
            this.context = context;
        }

        public void Add(T entity)
        {
            context.Set<T>().Add(entity);
        }

        public async Task Add(IReadOnlyList<T> entities)
        {
            await context.Set<T>().AddRangeAsync(entities);
        }
    }
}
