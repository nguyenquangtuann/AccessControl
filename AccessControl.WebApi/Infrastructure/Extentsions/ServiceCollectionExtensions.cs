using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using AccessControl.Data;
using AccessControl.Model.Models;
using System.Text;

namespace AccessControl.WebApi.Infrastructure.Extentsions
{
    public static class ServiceCollectionExtensions
    {
        public static AppSettings GetApplicationSettings(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var applicationSettingsConfiguration = configuration.GetSection("ApplicationSettings");
            services.Configure<AppSettings>(applicationSettingsConfiguration);
            return applicationSettingsConfiguration.Get<AppSettings>();
        }

        public static IServiceCollection AddDatabase(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
               .AddDbContext<ACSDBContext>(options => options
               .UseLazyLoadingProxies()
                   .UseSqlServer(configuration.GetDefaultConnectionString(), x => x.MigrationsAssembly("AccessControl.Data.Migrations")));


            return services;
        }

        public static void MigrationDb(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<ACSDBContext>();
                context.Database.Migrate();
            }
        }

        public static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services
                .AddIdentity<AppUser, AppRole>(options =>
                {
                    options.Password.RequiredLength = 6;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<ACSDBContext>();

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            AppSettings appSettings)
        {
#pragma warning disable CS8604 // Possible null reference argument.
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
#pragma warning restore CS8604 // Possible null reference argument.

            services
                .AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services)
            => services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "User management",
                    Description = "Api AccessControl",
                    Contact = new OpenApiContact
                    {
                        Name = "Nguyễn Quang Tuấn",
                        Email = "tuanq7719@gmail.com",
                        Url = new Uri("https://profile-tuannq.netlify.app/")
                    }
                });

                s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                s.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                       Scheme="Bearer",
                       Name="Bearer"
                    },
                    new List<string>()
                }
            });
            });
    }
}