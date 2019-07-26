using System.Collections.Generic;
using AliasMailApi.Models.DTO.Response;

namespace AliasMailApi.Models
{
    public class BaseResponse<T>: PagedResultBase
    {
        public IList<T> Data = new List<T>();
        public ICollection<ApiError> Errors = new List<ApiError>();
    }

    public class BaseOneResponse<T>: PagedResultBase
    {
        public T Data;
        public ICollection<ApiError> Errors = new List<ApiError>();
    }
}