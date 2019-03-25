using AutoMapper;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using TvMazeScraper.API.Controllers;
using TvMazeScraper.Domain.Conditions;
using TvMazeScraper.Domain.Interface;
using TvMazeScraper.Domain.Model;
using TvMazeScraper.Domain.Paging;
using TvMazeScraper.Unit.Tests.Helper;

namespace TvMazeScraper.Unit.Tests.Controllers
{
    public class TvShowsControllerTests
    {
        private TvShowsController controller;
        private Mock<IUnitOfWork> unitOfWorkMock;
        private Mock<IMapper> mapperMock;

        [SetUp]
        public void SetUp()
        {
            unitOfWorkMock = new Mock<IUnitOfWork>();
            mapperMock = new Mock<IMapper>();
            controller = new TvShowsController(unitOfWorkMock.Object, mapperMock.Object);
        }

        [TestCase(0, 10)]
        [TestCase(1, 10)]
        public async Task Get_ShouldCallReadOnlyRepositoryFindAll(int skip, int take)
        {
            var filter = new Filter { PageNumber = skip, PageSize = take };

            unitOfWorkMock.Setup(mock => mock.ShowReadOnlyRepository.FindAll(It.IsAny<Filter>(), It.IsAny<Expression<Func<Show, IOrderedEnumerable<Cast>>>>(), It.IsAny<string>()))
                .ReturnsAsync(GetShows());

            var actual = await controller.Get(filter);

            unitOfWorkMock.Verify(mock => mock.ShowReadOnlyRepository.FindAll(It.IsAny<Filter>(), It.IsAny<Expression<Func<Show, IOrderedEnumerable<Cast>>>>(), It.IsAny<string>()), Times.Once);
            unitOfWorkMock.Verify(mock => mock.ShowReadOnlyRepository.FindAll(filter, PredicateBuilder.orderFunc, Includes.CastInclude));
        }

        [TestCase(0, 1)]
        [TestCase(1, 10)]
        public async Task Get_ShouldReturnPaginatedShows(int skip, int take)
        {
            var filter = new Filter { PageNumber = skip, PageSize = take };
            var entities = GetShows(skip, take);
            var expected = entities.Select(e => MapEntityToDto(e)).ToArray();

            unitOfWorkMock.Setup(mock => mock.ShowReadOnlyRepository.FindAll(It.IsAny<Filter>(), It.IsAny<Expression<Func<Show, IOrderedEnumerable<Cast>>>>(), It.IsAny<string>()))
                .ReturnsAsync(entities);
            mapperMock.Setup(mock => mock.Map<ShowDto[]>(It.IsAny<List<Show>>())).Returns(expected);

            var result = await controller.Get(filter);
            var actual = ((Microsoft.AspNetCore.Mvc.JsonResult)result.Result).Value;
            Assert.AreEqual(expected.ToJson(), actual.ToJson());
        }

        [TestCase(3)]
        [TestCase(56)]
        public async Task Get_ShouldCallReadOnlyRepositoryFind(int id)
        {
            unitOfWorkMock.Setup(mock => mock.ShowReadOnlyRepository.FindAll(It.IsAny<Filter>(), It.IsAny<Expression<Func<Show, IOrderedEnumerable<Cast>>>>(), It.IsAny<string>()))
                .ReturnsAsync(GetShows());

            var actual = await controller.Get(id);

            unitOfWorkMock.Verify(mock => mock.ShowReadOnlyRepository.Find(It.IsAny<int>(), It.IsAny<Expression<Func<Show, IOrderedEnumerable<Cast>>>>(), It.IsAny<string>()), Times.Once);
            unitOfWorkMock.Verify(mock => mock.ShowReadOnlyRepository.Find(id, PredicateBuilder.orderFunc, Includes.CastInclude));
        }

        [TestCase(3)]
        [TestCase(56)]
        public async Task Get_ShouldReturnShowById(int id)
        {
            var entity = GetShows().FirstOrDefault(s => s.ApiId == id);
            var expected = MapEntityToDto(entity);

            unitOfWorkMock.Setup(mock => mock.ShowReadOnlyRepository.Find(It.IsAny<int>(), It.IsAny<Expression<Func<Show, IOrderedEnumerable<Cast>>>>(), It.IsAny<string>()))
                .ReturnsAsync(entity);
            mapperMock.Setup(mock => mock.Map<ShowDto>(It.IsAny<Show>())).Returns(expected);

            var result = await controller.Get(id);
            var actual = ((Microsoft.AspNetCore.Mvc.JsonResult)result.Result).Value;
            Assert.AreEqual(expected.ToJson(), actual.ToJson());
        }

        [Test]
        public async Task Get_ShouldReturnNotFountIfShowIdDontExist()
        {
            unitOfWorkMock.Setup(mock => mock.ShowReadOnlyRepository.Find(It.IsAny<int>(), It.IsAny<Expression<Func<Show, IOrderedEnumerable<Cast>>>>(), It.IsAny<string>()))
                .ReturnsAsync(default(Show));

            var result = await controller.Get(99);
            var actual = ((Microsoft.AspNetCore.Mvc.StatusCodeResult)result.Result).StatusCode;

            Assert.AreEqual((int)HttpStatusCode.NotFound, actual);
        }

        [Test]
        public async Task Post_ShouldCallShowWritingRepositoryAdd()
        {
            var expected = GetShows().ToArray();
            var dtos = expected.Select(e => MapEntityToDto(e)).ToArray();

            unitOfWorkMock.Setup(mock => mock.ShowWritingRepository.Add(It.IsAny<List<Show>>()));
            mapperMock.Setup(mock => mock.Map<Show[]>(It.IsAny<ShowDto[]>())).Returns(expected);

            await controller.Post(dtos);

            unitOfWorkMock.Verify(mock => mock.ShowWritingRepository.Add(It.IsAny<Show[]>()), Times.Once);
            unitOfWorkMock.Verify(mock => mock.ShowWritingRepository.Add(expected));
        }

        private ShowDto MapEntityToDto(Show expected)
        {
            return new ShowDto
            {
                Id = expected.ApiId,
                Name = expected.Name,
                CastsDto = expected.Casts.Select(c =>
                    new CastDto
                    {
                        Id = c.ApiId,
                        Name = c.Name,
                        Birthday = c.Birthday
                    }).ToList()
            };
        }

        private IEnumerable<Show> GetShows(int skip = 0, int take = 10)
        {
            var shows = new List<Show>();
            var show1 = new Show
            {
                Id = 1,
                ApiId = 3,
                Name = "Bitten",
                Casts = new List<Cast> {
                       new Cast {
                           ApiId =  176,
                           Name =  "Steve Lund",
                           Birthday = Convert.ToDateTime("1989-01-09")
                       },
                        new Cast {
                            ApiId =  192,
                            Name =  "Michael Xavier",
                            Birthday =  Convert.ToDateTime("0001-01-01")
                        },
                        new Cast {
                            ApiId =  175,
                            Name =  "Paul Greene",
                            Birthday =  Convert.ToDateTime("1974-06-02")
                        },
                        new Cast {
                            ApiId =  174,
                            Name =  "Greg Bryk",
                            Birthday =  Convert.ToDateTime("1972-01-01")
                        },
                        new Cast {
                            ApiId =  172,
                            Name =  "Laura Vandervoort",
                            Birthday =  Convert.ToDateTime("1984-09-22")
                        },
                        new Cast {
                            ApiId =  173,
                            Name =  "Greyston Holt",
                            Birthday =  Convert.ToDateTime("1985-09-30")
                        }
                   },
            };

            var show2 = new Show
            {
                Id = 2,
                ApiId = 494,
                Name = "V",
                Casts = new List<Cast> {
                    new Cast {
                        ApiId = 46644,
                        Name = "Logan Huffman",
                        Birthday = Convert.ToDateTime("0001-01-01")
                    },
                    new Cast {
                        ApiId = 10794,
                        Name = "Lourdes Benedicto",
                        Birthday = Convert.ToDateTime("1974-11-12")
                    },
                    new Cast {
                        ApiId = 3328,
                        Name = "Charles Mesure",
                        Birthday = Convert.ToDateTime("1970-08-12")
                    },
                }
            };

            var show3 = new Show
            {
                Id = 3,
                ApiId = 56,
                Name = "Chicago P.D.",
                Casts = new List<Cast>
                {
                }
            };

            if (skip == 0)
            {
                shows.Add(show1);
            }
            shows.Add(show2);

            if (take != 1)
            {
                shows.Add(show3);
            }

            return shows;
        }
    }
}


