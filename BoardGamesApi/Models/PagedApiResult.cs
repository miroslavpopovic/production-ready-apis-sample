using System;

namespace BoardGamesApi.Models
{
    public class PagedApiResult : ApiResult
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public int TotalPages => (int) Math.Ceiling((double) TotalCount / PageSize);
    }
}
