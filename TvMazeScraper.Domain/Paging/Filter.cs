namespace TvMazeScraper.Domain.Paging
{
    public class Filter
    {
        private int MaxPageSize { get; } = 1000;

        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public int Take {
            get { return PageSize; }
            set { PageSize = value > MaxPageSize ? MaxPageSize : value; }
        }
    }
}
