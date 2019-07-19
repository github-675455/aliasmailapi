using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System;

namespace AliasMailApi.Extensions
{
    public static class QueryRequestPaggingExtension
    {
        public static IQueryable<T> AddPagging<T>(this IQueryable<T> query, HttpContext httpContext)
        {
            var skip = httpContext.Request.Query["skip"];
            var take = httpContext.Request.Query["take"];

            var skipHasValue = !string.IsNullOrWhiteSpace(skip);
            var takeHasValue = !string.IsNullOrWhiteSpace(take);

            var skipParsed = 0;
            var takeParsed = 0;

            if(skipHasValue)
                skipParsed = Int32.Parse(skip);

            if(takeHasValue)
                takeParsed = Int32.Parse(take);

            if(skipHasValue && !takeHasValue)
                return query.Skip(skipParsed);
                
            if(takeHasValue && !skipHasValue)
                return query.Take(takeParsed);

            if(takeHasValue && skipHasValue)
                return query.Skip(skipParsed).Take(takeParsed);
            
            return query;
        }
}
}