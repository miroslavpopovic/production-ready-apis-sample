using System;
using System.Collections.Generic;

namespace BoardGamesApi.Models
{
    public static class ApiResultExtensions
    {
        public static ApiResult<T> WrapData<T>(this T data)
        {
            return new ApiResult<T> {Data = data};
        }

        public static ApiResult<object> WrapError(this string error)
        {
            var result = new ApiResult<object>();
            result.AddError(error);
            return result;
        }

        public static ApiResult<object> WrapException(this Exception ex)
        {
            var innerMostException = ex;
            while (innerMostException.InnerException != null)
                innerMostException = innerMostException.InnerException;

            var result = new ApiResult<object>();
            result.AddError(innerMostException.Message);

            return result;
        }

        public static ApiResult<IEnumerable<T>> WrapPagedList<T>(this PagedList<T> pagedList)
        {
            return new PagedApiResult<T>
            {
                Data = pagedList.Items,
                Page = pagedList.Page,
                PageSize = pagedList.PageSize,
                TotalCount = pagedList.TotalCount
            };
        }
    }
}
