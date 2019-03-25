using System.Collections.Generic;
using System.Threading.Tasks;
using TvMazeScraper.Domain.Model;

namespace TvMazeScraper.Domain.Interface
{
    public interface IWritingRepository<T> where T : IEntiTy
    {
        void Add(T entity);
        Task Add(IReadOnlyList<T> entities);
    }
}
