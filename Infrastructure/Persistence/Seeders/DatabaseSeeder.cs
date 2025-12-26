using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Infrastructure.Persistence.Context;
using Application.Services;

namespace StackBuldAssessment.Infrastructure.Persistence.Seeders;

/// <summary>
/// Handles seeding of initial data into the database
/// </summary>
public class DatabaseSeeder(
    StoreDbContext context,
    IPasswordHasher passwordHasher,
    IConfiguration configuration,
    ILogger<DatabaseSeeder> logger)
{
    /// <summary>
    /// Seeds all initial data
    /// </summary>
    public async Task SeedAsync()
    {
        try
        {
            logger.LogInformation("Starting database seeding...");

            // Ensure database is created
            await context.Database.MigrateAsync();

            await SeedAdminUserAsync();

            logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }


    /// <summary>
    /// Seeds the default admin user
    /// </summary>
    private async Task SeedAdminUserAsync()
    {
        logger.LogInformation("Seeding admin user...");

        // Get admin credentials from configuration
        var adminEmail = configuration["DefaultAdmin:Email"];
        var adminPassword = configuration["DefaultAdmin:Password"] ;

        // Check if admin user already exists
        var existingAdmin = await context.Users
            .FirstOrDefaultAsync(u => u.Email == adminEmail);

        if (existingAdmin != null)
        {
            logger.LogDebug("Admin user already exists: {Email}", adminEmail);
            return;
        }

        // Hash the password
        var (hashedPassword, salt) = passwordHasher.HashPassword(adminPassword);

        // Create admin user
        var adminUser = new User(adminEmail, hashedPassword, salt, UserRole.Admin);

        await context.Users.AddAsync(adminUser);
        await context.SaveChangesAsync();

        logger.LogInformation("Admin user created successfully: {Email}", adminEmail);
        logger.LogWarning("Default admin password is being used. Please change it immediately after first login!");
    }
}