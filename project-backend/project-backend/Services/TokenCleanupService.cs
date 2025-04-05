using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using project_backend.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace project_backend.Services
{
    public class TokenCleanupService : BackgroundService
    {
        private readonly IServiceProvider _services;

        public TokenCleanupService(IServiceProvider services)
        {
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var expiredTokens = await dbContext.InvalidTokens
                        .Where(t => t.ExpiresAt < DateTime.UtcNow)
                        .ToListAsync();

                    if (expiredTokens.Any())
                    {
                        dbContext.InvalidTokens.RemoveRange(expiredTokens);
                        await dbContext.SaveChangesAsync();
                    }
                }

                // Esperar 24 horas antes de volver a verificar
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}