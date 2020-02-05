using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tracing.Shared
{
    public class LogHeadersMiddleware
    {
        public static readonly List<string> RequestHeaders = new List<string>();
        public static readonly List<string> ResponseHeaders = new List<string>();
        private readonly RequestDelegate next;
        private readonly ILogger<LogHeadersMiddleware> logger;

        public LogHeadersMiddleware(RequestDelegate next, ILogger<LogHeadersMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var uniqueRequestHeaders = context.Request.Headers
                .Where(x => RequestHeaders.All(r => r != x.Key))
                .Select(x => x.Key);
            RequestHeaders.AddRange(uniqueRequestHeaders);

            await this.next.Invoke(context);
            var uniqueResponseHeaders = context.Response.Headers
                .Where(x => ResponseHeaders.All(r => r != x.Key))
                .Select(x => x.Key);
            ResponseHeaders.AddRange(uniqueResponseHeaders);

            logger.LogInformation("Parent TraceId: " + context.Request.Headers["traceparent"].ToString());
        }
    }
}