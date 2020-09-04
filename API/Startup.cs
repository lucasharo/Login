using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using API.Configurations;
using Entities.Settings;
using System.Net.Http;
using System;
using System.Net;
using Newtonsoft.Json.Serialization;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("Policy",
                builder =>
                {
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

            services.TryAddTransient<IHttpContextAccessor, HttpContextAccessor>();

            services.Configure<FacebookAuthSettings>(Configuration.GetSection(nameof(FacebookAuthSettings)));
            services.Configure<EmailSettings>(Configuration.GetSection(nameof(EmailSettings)));

            services.AddMvcCore();

            services.AddRazorPages();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new X509SecurityKey(new X509Certificate2(Configuration.GetSection("Certificado:Diretorio").Value, Configuration.GetSection("Certificado:Senha").Value), SecurityAlgorithms.RsaSha256Signature)
                };
            });

            services.AddDependencyInjectionSetup()
                .AddSwaggerSetup()
                .AddActionFilterSetup()
                .AddLogSetup(Configuration)
                .AddDatabaseSetup(Configuration)
                .AddAutoMapperSetup(Configuration);

            if (!string.IsNullOrEmpty(Configuration.GetSection("UrlProxy").Value))
            {
                HttpClient.DefaultProxy = new WebProxy
                {
                    Address = new Uri(Configuration.GetSection("UrlProxy").Value)
                };
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseCors("Policy");

            app.UseSwaggerSetup(Configuration);
            app.UseMiddlewareSetup();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}