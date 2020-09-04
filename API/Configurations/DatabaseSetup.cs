using System;
using Infra.Interfaces;
using Infra.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Configurations
{
    public static class DatabaseSetup
    {
        public static IServiceCollection AddDatabaseSetup(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<IUnitOfWork, UnitOfWork>(x =>
            {
                var cnn = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

                cnn.Open();

                return new UnitOfWork(cnn, cnn.BeginTransaction());
            });

            return services;
        }
    }
}