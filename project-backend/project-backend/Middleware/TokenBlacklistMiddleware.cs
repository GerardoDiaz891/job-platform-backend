// Middleware/TokenBlacklistMiddleware.cs
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using project_backend.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace project_backend.Middleware
{
    public class TokenBlacklistMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenBlacklistMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
        {
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (!string.IsNullOrEmpty(token))
            {
                var isInvalid = await dbContext.InvalidTokens
                    .AnyAsync(it => it.Token == token && it.ExpiresAt > DateTime.UtcNow);

                if (isInvalid)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Token inválido.");
                    return;
                }
            }

            await _next(context);
        }
    }

    public static class TokenBlacklistMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenBlacklist(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenBlacklistMiddleware>();
        }
    }
}