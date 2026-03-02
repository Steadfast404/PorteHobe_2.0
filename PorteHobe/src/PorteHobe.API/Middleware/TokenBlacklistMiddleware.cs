using Portehobe.Infrastructure.Services;
using System.Net.Http.Headers;

namespace Portehobe.API.Middleware
{
    public class TokenBlacklistMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenBlacklistMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITokenBlacklistService blacklistService)
        {
            string? token = null;
            var authHeader = context.Request.Headers["Authorization"].ToString();

            if (AuthenticationHeaderValue.TryParse(authHeader, out var headerValue) && 
                string.Equals(headerValue.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase))
            {
                token = headerValue.Parameter;
            }

            if (!string.IsNullOrEmpty(token) && blacklistService.IsTokenBlacklisted(token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Token is blacklisted");
                return;
            }

            await _next(context);
        }
    }
}
