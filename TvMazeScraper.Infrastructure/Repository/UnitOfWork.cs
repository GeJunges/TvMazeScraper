using System.Threading.Tasks;
using TvMazeScraper.Domain.Interface;
using TvMazeScraper.Domain.Model;
using TvMazeScraper.Infrastructure.Settings;

namespace TvMazeScraper.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TvMazeScraperContext context;

        public UnitOfWork(TvMazeScraperContext context)
        {
            this.context = context;
        }

        private readonly IReadOnlyRepository<Show> showReadOnlyRepository;

        private readonly IWritingRepository<Show> showWritingRepository;

        public IReadOnlyRepository<Show> ShowReadOnlyRepository => showReadOnlyRepository ?? new ReadOnlyRepository<Show>(context);

        public IWritingRepository<Show> ShowWritingRepository => showWritingRepository ?? new WritingRepository<Show>(context);

        public async Task<int> CompleteAsync()
        {
            return await context.SaveChangesAsync();
        }

        public int Complete()
        {
            return context.SaveChanges();
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
