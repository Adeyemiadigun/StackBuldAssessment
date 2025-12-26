using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackBuldAssessment.Infrastructure.Persistence.Seeders;
using StackBuldAssessment.Infrastructure.Services;
using Application.Common.Interfaces;
using Application.Services;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Infrastructure.Services.Payment.Paystack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Persistence.Context;
using Application.Repositories;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
        {

            // Register authentication and user services
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<ICurrentUser, CurrentUserService>();
            services.Configure<PaystackOptions>(
             configuration.GetSection("Paystack"));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddHttpClient<IPaymentGateway, PaystackPaymentGateway>(client =>
            {
                client.BaseAddress = new Uri("https://api.paystack.co/");
            });
            // Add DbContext
            services.AddDbContext<StoreDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(StoreDbContext).Assembly.FullName)));

            // Register generic repository
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IPaymentTransactionRepository, PaymentTransactionRepository>();

            // Register UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register Database Seeder
            services.AddScoped<DatabaseSeeder>();
            return services;

        }
    }
}