namespace AliasMailApi.Models
{
    public class BaseResponse<T>
    {
        public bool Success;
        public T Data;
        public ApiError[] Errors;
    }
}