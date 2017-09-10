using System;
using System.Collections.Generic;

namespace BoardGamesApi.Models
{
    /// <inheritdoc />
    /// <summary>
    /// Encapsulates an API result with success flag, errors (if any), result list and paging information.
    /// </summary>
    /// <typeparam name="T">Type of the result.</typeparam>
    public class PagedApiResult<T> : ApiResult<IEnumerable<T>>
    {
        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets the page size.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the total item count.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets the total page count.
        /// </summary>
        public int TotalPages => (int) Math.Ceiling((double) TotalCount / PageSize);
    }
}
