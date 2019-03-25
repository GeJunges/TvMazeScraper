using System;
using System.Collections.Generic;

namespace TvMazeScraper.Domain.Model
{
    public class ShowDto
    {
        public ShowDto()
        {
            CastsDto = new List<CastDto>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public IReadOnlyList<CastDto> CastsDto { get; set; }
    }
}
