using System.Collections.Generic;

namespace TvMazeScraper.Domain.Model
{
    public class Show : IEntiTy
    {
        public Show()
        {
            Casts = new List<Cast>();
        }

        public string Name { get; set; }
        public IReadOnlyList<Cast> Casts { get; set; }
    }
}
