using System.Collections.Generic;
using AliasMailApi.Models.DTO.Response;

namespace AliasMailApi.Models
{
    public class BaseResponse<T>: PagedResultBase where T : class
    {
        public IList<T> Data = new List<T>();
        public ICollection<ApiError> Errors = new List<ApiError>();
    }
}