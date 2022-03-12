using Infrastructure.Authorization;
using Infrastructure.Fcm;
using Infrastructure.SMS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Infrastructure;

public static class InfrastructureExtension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options => {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .ConfigureWarnings(builder =>
                    builder.Throw(RelationalEventId.MultipleCollectionIncludeWarning)
                        .Ignore(RelationalEventId.BoolWithDefaultWarning)
                );
            if (configuration["ComponentConfig:Environment"].Equals("Development")) {
                options.EnableSensitiveDataLogging();
            }
        });

        services.AddScoped<ISmsService, SmsService>();

        services.AddTransient<IFcmService, FcmService>();

        services.AddScoped<IAuthorizationService, AuthorizationService>();

        services.Configure<Config>(configuration.GetSection("ComponentConfig"));

        ConfigRedis(services, configuration);

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        return services;
    }

    private static void ConfigRedis(IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options => {
            options.Configuration =
                $"{configuration["ComponentConfig:Redis:Host"]}:{configuration["ComponentConfig:Redis:Port"]}";
            options.InstanceName = "";
            options.ConfigurationOptions = new ConfigurationOptions {
                Password = configuration["ComponentConfig:Redis:Password"],
                EndPoints = {
                    {
                        configuration["ComponentConfig:Redis:Host"],
                        int.Parse(configuration["ComponentConfig:Redis:Port"])
                    }
                }
            };
        });
    }
}