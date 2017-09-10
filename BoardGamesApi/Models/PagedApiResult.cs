using System;
using System.Collections.Generic;

namespace BoardGamesApi.Models
{
    public class PagedApiResult<T> : ApiResult<IEnumerable<T>>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public int TotalPages => (int) Math.Ceiling((double) TotalCount / PageSize);
    }
}
