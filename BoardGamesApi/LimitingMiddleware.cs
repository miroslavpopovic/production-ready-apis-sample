using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoardGamesApi.Models;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BoardGamesApi
{
    public class LimitingMiddleware
    {
        private static readonly IDictionary<string, DateTime> TokenAccess = new Dictionary<string, DateTime>();

        private readonly RequestDelegate _next;
        private readonly ILogger<LimitingMiddleware> _logger;

        public LimitingMiddleware(RequestDelegate next, ILogger<LimitingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var request = context.Request;

            if (request.GetUri().AbsoluteUri.ToLowerInvariant().Contains("/api/games"))
            {
                var token = request.Headers["Authorization"].FirstOrDefault()?.ToLowerInvariant().Replace("bearer ", "");

                if (token != null)
                {
                    if (!TokenAccess.ContainsKey(token))
                        TokenAccess.Add(token, DateTime.UtcNow);
                    else
                    {
                        var lastAccess = TokenAccess[token];
                        TokenAccess[token] = DateTime.UtcNow;

                        if (lastAccess.AddSeconds(5) >= DateTime.UtcNow)
                        {
                            const string message = "Token limit reached, operation cancelled";

                            _logger.LogInformation(message);

                            var result = JsonConvert.SerializeObject(
                                message.WrapError(),
                                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                            context.Response.ContentType = "application/json";

                            await context.Response.WriteAsync(result);

                            return;
                        }
                    }
                }
            }

            await _next(context);
        }
    }
}
