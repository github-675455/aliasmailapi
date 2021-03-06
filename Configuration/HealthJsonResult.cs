using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Options;

namespace AliasMailApi.Configuration
{
    public class HealthJsonResult : HealthCheckOptions
    {
        public static string buildInfo;
        public HealthJsonResult(IOptions<AppOptions> options)
        {
            base.ResponseWriter = WriteResponse;
            buildInfo = options.Value.buildInfo;
        }

        private static Task WriteResponse(HttpContext httpContext, HealthReport result)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            httpContext.Response.Headers["Access-Control-Allow-Methods"] = "GET";
            httpContext.Response.Headers["Access-Control-Allow-Headers"] = "Content-Type, Authorization";

            var json = new JObject(
                new JProperty("status", result.Status.ToString()),
                new JProperty("results", new JObject(result.Entries.Select(pair =>
                    new JProperty(pair.Key, new JObject(
                        new JProperty("status", pair.Value.Status.ToString()),
                        new JProperty("description", pair.Value.Description),
                        new JProperty("data", new JObject(pair.Value.Data.Select(p => new JProperty(p.Key, p.Value))))))))),
		        new JProperty("buildInfo", buildInfo));
            return httpContext.Response.WriteAsync(json.ToString(Formatting.Indented));
        }
    }
}
