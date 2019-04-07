using System.Collections.Generic;

namespace AliasMailApi.Models
{
    public class BaseResponse<T>
    {
        public bool Success;
        public T Data;
        public ICollection<ApiError> Errors = new List<ApiError>();
    }
}