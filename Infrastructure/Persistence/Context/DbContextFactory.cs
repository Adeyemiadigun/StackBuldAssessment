using System;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace StackBuldAssessment.Infrastructure.Persistence.Context;

/// <summary>
/// Design-time factory for creating AppDbContext instances during migrations
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<StoreDbContext>
{
    public StoreDbContext CreateDbContext(string[] args)
    {
        // Determine the base path for the API project
        var currentDirectory = Directory.GetCurrentDirectory();
        var apiProjectPath = Path.Combine(currentDirectory, "..", "Api");

        // If we're not in the Infrastructure.Persistence directory, try to find the API project
        if (!Directory.Exists(apiProjectPath))
        {
            // Try from solution root
            apiProjectPath = Path.Combine(currentDirectory, "src", "Api");
        }

        if (!Directory.Exists(apiProjectPath))
        {
            throw new InvalidOperationException(
                $"Cannot locate StackBuldAssessment.Api project. Current directory: {currentDirectory}");
        }

        // Build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiProjectPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        // Create DbContextOptions
        var optionsBuilder = new DbContextOptionsBuilder<StoreDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found in appsettings.json");
        }

        optionsBuilder.UseNpgsql(
            connectionString,
            b => b.MigrationsAssembly(typeof(StoreDbContext).Assembly.FullName));

        return new StoreDbContext(optionsBuilder.Options);
    }
}