using System.Threading.Tasks;
using TvMazeScraper.Domain.Model;

namespace TvMazeScraper.Domain.Interface
{
    public interface IUnitOfWork
    {
        IReadOnlyRepository<Show> ShowReadOnlyRepository { get; }

        IWritingRepository<Show> ShowWritingRepository { get; }

        Task<int> CompleteAsync();
        int Complete();
    }
}
