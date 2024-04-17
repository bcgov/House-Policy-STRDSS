namespace StrDss.Model
{
    public class PageInfo
    {
        public static int DefaultPageSize => 10;

        private int _pageNumber = 1;
        public int PageNumber
        {
            get { return _pageNumber; }
            set
            {
                _pageNumber = (value <= 0 ? 1 : value);
            }
        }

        public int PageSize { get; set; }
        public int ItemCount { get; set; }
        public int TotalCount { get; set; }
        public int PageCount => PageSize == 0 ? 1 : ((int)(TotalCount / PageSize) + (TotalCount % PageSize == 0 ? 0 : 1));
        public bool HasPreviousPage => PageNumber != 1;
        public bool HasNextPage => PageNumber < PageCount;
        public string OrderBy { get; set; } = "";
        public string Direction { get; set; } = "";
    }

    public class PagedDto<T>
    {
        public IEnumerable<T> SourceList { get; set; } = new List<T>();
        public PageInfo PageInfo { get; set; } = null!;
    }
}
