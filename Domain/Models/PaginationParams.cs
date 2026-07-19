namespace Domain.Models
{
    /// <summary>
    /// Represents pagination parameters for paginated queries.
    /// Used to specify which page and how many items per page are requested.
    /// </summary>
    public class PaginationParams
    {
        private int _pageNumber = 1;
        private int _pageSize = 10;

        /// <summary>
        /// The page number to retrieve (1-based indexing).
        /// Default: 1 (first page)
        /// </summary>
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value > 0 ? value : 1;
        }

        /// <summary>
        /// The number of items per page.
        /// Default: 10
        /// Clamped to MaxPageSize if exceeded.
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (value <= 0)
                    _pageSize = 10;
                else if (value > MaxPageSize)
                    _pageSize = MaxPageSize;
                else
                    _pageSize = value;
            }
        }

        /// <summary>
        /// Maximum allowed page size to prevent performance issues.
        /// Default: 100 items per page max.
        /// </summary>
        public const int MaxPageSize = 100;

        /// <summary>
        /// Calculates the number of records to skip based on current page and page size.
        /// Formula: (PageNumber - 1) * PageSize
        /// 
        /// Examples:
        /// - Page 1, Size 10: Skip = 0
        /// - Page 2, Size 10: Skip = 10
        /// - Page 3, Size 10: Skip = 20
        /// </summary>
        public int CalculateSkip()
        {
            return (PageNumber - 1) * PageSize;
        }

        /// <summary>
        /// Creates a new PaginationParams with specified page number and size.
        /// </summary>
        public PaginationParams(int pageNumber = 1, int pageSize = 10)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        /// <summary>
        /// Validates pagination parameters.
        /// Returns true if parameters are valid.
        /// </summary>
        public bool IsValid()
        {
            return PageNumber > 0 && PageSize > 0 && PageSize <= MaxPageSize;
        }
    }
}
