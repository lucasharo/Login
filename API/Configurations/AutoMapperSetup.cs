using AutoMapper;
using Entities.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace API.Configurations
{
    public static class AutoMapperSetup
    {
        public static IServiceCollection AddAutoMapperSetup(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddAutoMapper(typeof(Startup));

            services.AddScoped(provider => new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Usuario, UsuarioLogin>().ReverseMap();
            }).CreateMapper());

            return services;
        }
    }
}