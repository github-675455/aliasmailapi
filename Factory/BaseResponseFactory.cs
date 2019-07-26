using AliasMailApi.Models;

namespace aliasmailapi.Factory
{
    public static class BaseResponseFactory<T>
    {
        public static BaseResponse<T> CreateDefaultBaseResponse()
        {
            var result = new BaseResponse<T>();

            return result;
        }
        public static BaseOneResponse<T> CreateDefaultBaseOneResponse()
        {
            var result = new BaseOneResponse<T>();

            return result;
        }
    }
}