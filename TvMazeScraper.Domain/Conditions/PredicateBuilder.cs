using System;
using System.Linq;
using System.Linq.Expressions;
using TvMazeScraper.Domain.Model;

namespace TvMazeScraper.Domain.Conditions
{
    public static class PredicateBuilder
    {
        public static Expression<Func<Show, IOrderedEnumerable<Cast>>> orderFunc = c => c.Casts.OrderByDescending(b => b.Birthday);

        public static Expression<Func<Show, object>> keySelector = c => c.Casts.OrderByDescending(d => d.Birthday);
    }
}
