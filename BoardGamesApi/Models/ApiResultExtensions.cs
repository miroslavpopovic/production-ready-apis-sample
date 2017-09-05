using System;

namespace BoardGamesApi.Models
{
    public static class ApiResultExtensions
    {
        public static ApiResult WrapData(this object data)
        {
            return new ApiResult {Data = data};
        }

        public static ApiResult WrapError(this string error)
        {
            var result = new ApiResult();
            result.AddError(error);
            return result;
        }

        public static ApiResult WrapException(this Exception ex)
        {
            var innerMostException = ex;
            while (innerMostException.InnerException != null)
                innerMostException = innerMostException.InnerException;

            var result = new ApiResult();
            result.AddError(innerMostException.Message);

            return result;
        }

        public static ApiResult WrapPagedList<T>(this PagedList<T> pagedList)
        {
            return new PagedApiResult
            {
                Data = pagedList.Items,
                Page = pagedList.Page,
                PageSize = pagedList.PageSize,
                TotalCount = pagedList.TotalCount
            };
        }
    }
}
