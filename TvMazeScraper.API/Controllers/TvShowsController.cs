using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TvMazeScraper.Domain.Conditions;
using TvMazeScraper.Domain.Interface;
using TvMazeScraper.Domain.Model;
using TvMazeScraper.Domain.Paging;

namespace TvMazeScraper.API.Controllers
{
    [Route("api/tvshows")]
    [ApiController]
    public class TvShowsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public TvShowsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShowDto>>> Get([FromQuery]Filter filter)
        {
            var entities = await unitOfWork.ShowReadOnlyRepository.FindAll(filter, PredicateBuilder.orderFunc, Includes.CastInclude);

            var result = mapper.Map<ShowDto[]>(entities);

            return new JsonResult(result)
            {
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<ShowDto>>> Get(int id)
        {
            var entity = await unitOfWork.ShowReadOnlyRepository.Find(id, PredicateBuilder.orderFunc, Includes.CastInclude);

            if (entity == null)
            {
                return NotFound();
            }

            var result = mapper.Map<ShowDto>(entity);

            return new JsonResult(result)
            {
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ShowDto[] shows)
        {
            var entities = mapper.Map<Show[]>(shows);

            await unitOfWork.ShowWritingRepository.Add(entities);
            await unitOfWork.CompleteAsync();

            return Ok();
        }
    }
}
