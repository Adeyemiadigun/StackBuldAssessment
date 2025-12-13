using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdmissionMinaret.Infrastructure.Services;
using Application.Services;
using Infrastructure.Services;
using Infrastructure.Services.Payment.Paystack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            services.AddHttpClient<IPaymentGateway, PaystackPaymentGateway>(client =>
            {
                client.BaseAddress = new Uri("https://api.paystack.co/");
            });

            return services;

        }
    }
}