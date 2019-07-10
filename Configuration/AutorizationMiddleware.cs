using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace AliasMailApi.Configuration
{
    public class AutorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppOptions _options;

        public AutorizationMiddleware(RequestDelegate next, IOptions<AppOptions> options)
        {
            _next = next;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var route = context.GetRouteData();

            var controller = route.Values["controller"];
            
            if(controller != null)
            {
                var controllerSanitized = controller.ToString().ToLower();
                if(controllerSanitized.Equals("webhook"))
                {
                    await _next(context);
                    return;
                }
            }


            if (context.Request.Headers["Authorization"] != _options.consumerToken)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }
            await _next(context);
        }
    }
}