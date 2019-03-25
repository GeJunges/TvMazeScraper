using System;

namespace TvMazeScraper.Domain.Model
{
    public class Cast : IEntiTy
    {
        public string Name { get; set; }
        public DateTime Birthday { get; set; }

        public int ShowId { get; set; }
        public Show Show { get; set; }
    }
}
