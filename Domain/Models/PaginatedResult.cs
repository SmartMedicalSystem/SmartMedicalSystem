using System.Collections.Generic;
using System.Linq;

namespace Domain.Models
{
    /// <summary>
    /// Generic wrapper for paginated query results.
    /// Contains the page of items along with pagination metadata.
    /// </summary>
    /// <typeparam name="T">The type of items in the paginated result</typeparam>
    public class PaginatedResult<T>
    {
        /// <summary>
        /// The collection of items for the current page.
        /// </summary>
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// The current page number (1-based).
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// The number of items on the current page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// The total number of items across all pages.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// The total number of pages.
        /// Calculated as: ceil(TotalCount / PageSize)
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Indicates whether there are more pages after the current page.
        /// </summary>
        public bool HasNextPage => PageNumber < TotalPages;

        /// <summary>
        /// Indicates whether there are pages before the current page.
        /// </summary>
        public bool HasPreviousPage => PageNumber > 1;

        /// <summary>
        /// The index of the first item on the current page.
        /// Used for displaying "Showing X to Y of Z" style messages.
        /// </summary>
        public int FirstItemIndex => ((PageNumber - 1) * PageSize) + 1;

        /// <summary>
        /// The index of the last item on the current page.
        /// </summary>
        public int LastItemIndex => FirstItemIndex + Items.Count() - 1;

        /// <summary>
        /// Creates a new paginated result with default empty collection.
        /// </summary>
        public PaginatedResult()
        {
            Items = Enumerable.Empty<T>();
        }

        /// <summary>
        /// Creates a new paginated result with specified parameters.
        /// </summary>
        /// <param name="items">The collection of items for this page</param>
        /// <param name="totalCount">The total number of items across all pages</param>
        /// <param name="pageNumber">The current page number (1-based)</param>
        /// <param name="pageSize">The number of items per page</param>
        public PaginatedResult(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;

            // Calculate total pages: ceil(totalCount / pageSize)
            TotalPages = (totalCount + pageSize - 1) / pageSize;

            // Handle edge case: if totalCount is 0, there's 1 page (empty page)
            if (TotalPages == 0)
                TotalPages = 1;
        }

        /// <summary>
        /// Creates a paginated result from a list and pagination parameters.
        /// Automatically calculates pagination metadata.
        /// </summary>
        /// <param name="items">The items to paginate</param>
        /// <param name="totalCount">The total count of all items (before pagination)</param>
        /// <param name="pagination">The pagination parameters used</param>
        /// <returns>A new PaginatedResult with calculated metadata</returns>
        public static PaginatedResult<T> Create(IEnumerable<T> items, int totalCount, PaginationParams pagination)
        {
            return new PaginatedResult<T>(items, totalCount, pagination.PageNumber, pagination.PageSize);
        }

        /// <summary>
        /// Creates an empty paginated result (no items, page 0 of 0).
        /// </summary>
        /// <returns>An empty paginated result</returns>
        public static PaginatedResult<T> Empty()
        {
            return new PaginatedResult<T>(Enumerable.Empty<T>(), 0, 1, 10);
        }

        /// <summary>
        /// Gets a summary string showing the range of items displayed.
        /// Example: "Showing 1 to 10 of 250"
        /// </summary>
        public string GetSummary()
        {
            if (TotalCount == 0)
                return "No items to display";

            return $"Showing {FirstItemIndex} to {LastItemIndex} of {TotalCount}";
        }
    }
}
