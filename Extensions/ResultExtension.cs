using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using aliasmailapi.Models;
using AliasMailApi.Models;
using AliasMailApi.Models.DTO.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace aliasmailapi.Extensions
{
    public static class ResultExtension
    {
        private static IHttpContextAccessor _accessor;
        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _accessor = httpContextAccessor;
        }
        public static async Task<BaseResponse<T>> GetPagedResult<T>(this IQueryable<T> query) where T : class
        {
            var page = _accessor.HttpContext.Request.Query["page"];
            var pageSize = _accessor.HttpContext.Request.Query["pageSize"];
            var disableRowCount = _accessor.HttpContext.Request.Query["disableRowCount"];
            var orderByParamater = _accessor.HttpContext.Request.Query["orderByCreated"];

            var pageHasValue = !string.IsNullOrWhiteSpace(page);
            var pageSizeHasValue = !string.IsNullOrWhiteSpace(pageSize);
            var disableRowCountHasValue = !string.IsNullOrWhiteSpace(disableRowCount);
            var orderByParamaterHasValue = !string.IsNullOrWhiteSpace(orderByParamater);

            var pageParsed = 0;
            var pageSizeParsed = 10;
            var disableRowCountParsed = false;
            var orderByParamaterParsed = false;

            if(pageHasValue)
                pageParsed = Int32.Parse(page);

            if(pageSizeHasValue)
                pageSizeParsed = Int32.Parse(pageSize);

            if(disableRowCountHasValue)
                disableRowCountParsed = Boolean.Parse(disableRowCount);

            if(orderByParamaterHasValue)
                orderByParamaterParsed = Boolean.Parse(orderByParamater);

            var result = new BaseResponse<T>();
            result.CurrentPage = pageParsed;
            result.PageSize = pageSizeParsed;

            if(!disableRowCountParsed)
            {
                result.RowCount = query.Count();
                var pageCount = (double)result.RowCount / pageSizeParsed;
                result.PageCount = (int)Math.Ceiling(Double.IsNaN(pageCount) ? 0 : pageCount);
            }

            if(orderByParamaterParsed)
                query = query.OrderByDescending(e => (e as BaseModelTemplate).Created);

            var skipActuallyResult = (pageParsed - 1) * pageSizeParsed;
            var skip = skipActuallyResult < 1 ? 0 : skipActuallyResult;
            
            if(pageParsed < 0)
                pageSizeParsed = result.PageCount.Value - Math.Abs(pageParsed);
            
            var finalQuery = query.Skip(skip).Take(pageSizeParsed);

            result.Data = await finalQuery.ToListAsync();

            return result;
        }

        public static async Task<BaseResponse<T>> GetOneResult<T>(this IQueryable<T> query, Expression<Func<T, bool>> predicate) where T : class
        {
            var result = new BaseResponse<T>();

            result.Data = await query.Where(predicate).ToListAsync();

            return result;
        }

        public static BaseOneResponse<T> FormatOneResult<T>(this T item)
        {
            var result = new BaseOneResponse<T>();

            result.Data = item;

            return result;
        }
    }
}