using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Extensions
{
    public static class MigrationExtensions
    {
        public static void ApplyMigration(IServiceProvider serviceProvider, bool isDevelopment = false)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<StoreDbContext>>();

            try
            {
                logger.LogInformation("Starting database migration check...");

                // Check if database exists
                var canConnect = context.Database.CanConnect();
                if (!canConnect)
                {
                    logger.LogInformation("Database does not exist. Creating database...");
                    context.Database.EnsureCreated();
                    logger.LogInformation("Database created successfully.");
                }

                // Get pending migrations
                var pendingMigrations = context.Database.GetPendingMigrations().ToList();

                if (pendingMigrations.Any())
                {
                    logger.LogInformation("Found {Count} pending migrations. Applying migrations...", pendingMigrations.Count);

                    foreach (var migration in pendingMigrations)
                    {
                        logger.LogInformation("Pending migration: {Migration}", migration);
                    }

                    context.Database.Migrate();
                    logger.LogInformation("All migrations applied successfully.");
                }
                else
                {
                    logger.LogInformation("Database is up to date. No pending migrations.");
                }

                // Log applied migrations
                var appliedMigrations = context.Database.GetAppliedMigrations().ToList();
                logger.LogInformation("Total applied migrations: {Count}", appliedMigrations.Count);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating the database.");

                if (!isDevelopment)
                {
                    throw;
                }
            }
        }
    }
}
