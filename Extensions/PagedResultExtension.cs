using System;
using System.Linq;
using System.Threading.Tasks;
using AliasMailApi.Models.DTO.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace aliasmailapi.Extensions
{
    public static class PagedResultExtension
    {
        private static IHttpContextAccessor _accessor;
        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _accessor = httpContextAccessor;
        }
        public static async Task<PagedResult<T>> GetPaged<T>(this IQueryable<T> query) where T : class
        {
            var page = _accessor.HttpContext.Request.Query["page"];
            var pageSize = _accessor.HttpContext.Request.Query["pageSize"];
            var disableRowCount = _accessor.HttpContext.Request.Query["disableRowCount"];

            var pageHasValue = !string.IsNullOrWhiteSpace(page);
            var pageSizeHasValue = !string.IsNullOrWhiteSpace(pageSize);
            var disableRowCountHasValue = !string.IsNullOrWhiteSpace(disableRowCount);

            var pageParsed = 0;
            var pageSizeParsed = 10;
            var disableRowCountParsed = false;

            if(pageHasValue)
                pageParsed = Int32.Parse(page);

            if(pageSizeHasValue)
                pageSizeParsed = Int32.Parse(pageSize);

            if(disableRowCountHasValue)
                disableRowCountParsed = Boolean.Parse(disableRowCount);

            var result = new PagedResult<T>();
            result.CurrentPage = pageParsed;
            result.PageSize = pageSizeParsed;

            if(!disableRowCountParsed)
            {
                result.RowCount = query.Count();
                var pageCount = (double)result.RowCount / pageSizeParsed;
                result.PageCount = (int)Math.Ceiling(Double.IsNaN(pageCount) ? 0 : pageCount);
            }

            var skipActuallyResult = (pageParsed - 1) * pageSizeParsed;
            var skip = skipActuallyResult < 1 ? 0 : skipActuallyResult;
            result.Results = await query.Skip(skip).Take(pageSizeParsed).ToListAsync();

            return result;
        }
    }
}