using System.Reflection;
using System.Text;
using Application;
using Application.Common.Response;
using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Parbad.Builder;
using Parbad.Gateway.ZarinPal;
using Parbad.Storage.EntityFrameworkCore.Builder;
using Presentation.Common.DiscordReporter.ExceptionHolderService;
using Presentation.Filters;
using ReportSharp.Api.Extensions;
using ReportSharp.Api.Services.ApiAuthorizationService;
using ReportSharp.DatabaseReporter.Builder.ReporterOptionsBuilder.DatabaseOptionsBuilder;
using ReportSharp.DatabaseReporter.DbContext;
using ReportSharp.DiscordReporter.Builder.ReporterOptionsBuilder.DiscordOptionsBuilder;
using ReportSharp.Extensions;
using ReportSharp.Middlewares;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Presentation;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddInfrastructure(Configuration);
        services.AddApplication();

        ConfigLogger(services);

        ConfigSwaggerService(services);

        ConfigAuthService(services);

        ConfigHangfireService(services);

        ConfigPayment(services);

        services.AddCors(options => {
            options.AddPolicy(CorsConstants.AccessControlAllowOrigin, builder =>
                builder.WithOrigins("*")
                    .WithHeaders("*")
                    .WithMethods("*")
                    .WithExposedHeaders("Content-Disposition")
            );
        });

        ConfigControllerService(services);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (Configuration["ComponentConfig:Environment"].Equals("Development")) {
            app.UseDeveloperExceptionPage();
            app.UseHangfireDashboard();

            app.UseDirectoryBrowser();

            app.UseSwagger();

            app.UseSwaggerUI(options => {
                options.SwaggerEndpoint($"/swagger/{Policies.UserSwagger}/swagger.json", Policies.UserSwagger);
                options.SwaggerEndpoint($"/swagger/{Policies.AdminSwagger}/swagger.json", Policies.AdminSwagger);
                options.EnableFilter("");
                options.DocExpansion(DocExpansion.None);
                options.ShowExtensions();
                options.DefaultModelRendering(ModelRendering.Example);
                options.DisplayOperationId();
                options.DisplayRequestDuration();
                options.EnableDeepLinking();
                options.EnablePersistAuthorization();
                options.ShowCommonExtensions();
                options.EnableTryItOutByDefault();
            });
        }
        else {
            app.UseExceptionHandler("/error");
        }

        app.UseReportSharp(configure => {
            configure
                .UseReportSharpMiddleware<ReportSharpMiddleware>()
                .UseApis();
        });

        app.UseApplication();

        app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("Content-Disposition")
        );

        app.UseStaticFiles(new StaticFileOptions {
            HttpsCompression = HttpsCompressionMode.Compress,
            OnPrepareResponse = context => {
                var headers = context.Context.Response.GetTypedHeaders();
                headers.CacheControl = new CacheControlHeaderValue {
                    Public = true,
                    MaxAge = TimeSpan.FromDays(180)
                };
            }
        });

        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }

    private void ConfigSwaggerService(IServiceCollection services)
    {
        services.AddSwaggerGen(options => {
            options.SwaggerDoc(
                Policies.UserSwagger,
                new OpenApiInfo {
                    Title = "", Version = Policies.UserSwagger
                }
            );
            options.SwaggerDoc(
                Policies.AdminSwagger,
                new OpenApiInfo {
                    Title = "", Version = Policies.AdminSwagger
                }
            );
            options.AddSecurityDefinition("Token", new OpenApiSecurityScheme {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme.",
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Token",
                        },
                    },
                    new string[] { }
                }
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            options.IncludeXmlComments(xmlPath);
        });
    }

    private void ConfigControllerService(IServiceCollection services)
    {
        services.AddControllersWithViews(options => {
            options.Filters.Add<UserAuthorizeFilter>();
            options.RespectBrowserAcceptHeader = true;
        }).ConfigureApiBehaviorOptions(options => {
            options.SuppressConsumesConstraintForFormFileParameters = true;
            options.InvalidModelStateResponseFactory =
                _ => ResponseFormat.BadRequestMsg<object>("درخواست نامعتبر");
        }).AddNewtonsoftJson(options =>
            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        );
    }

    private void ConfigAuthService(IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["ComponentConfig:Jwt:Issuer"],
                    ValidAudience = Configuration["ComponentConfig:Jwt:Audience"],
                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Configuration["ComponentConfig:Jwt:SecretKey"])),
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization(config =>
            config.AddPolicy(Policies.User, Policies.UserPolicy())
        );
        services.AddAuthorization(config =>
            config.AddPolicy(Policies.Admin, Policies.AdminPolicy())
        );
    }

    private void ConfigHangfireService(IServiceCollection services)
    {
        services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(
                Configuration.GetConnectionString("DefaultConnection"),
                new PostgreSqlStorageOptions {
                    QueuePollInterval = TimeSpan.FromSeconds(1),
                }
            )
        );

        services.AddHangfireServer(options =>
            options.SchedulePollingInterval = TimeSpan.FromSeconds(1)
        );
    }

    private void ConfigPayment(IServiceCollection services)
    {
        services.AddParbad()
            .ConfigureGateways(gateways => {
                gateways.AddZarinPal()
                    .WithAccounts(accounts => {
                        accounts.AddInMemory(account => {
                            account.MerchantId = Configuration["ComponentConfig:Payment:Zarinpal:MerchantId"];
                            account.IsSandbox =
                                Convert.ToBoolean(Configuration["ComponentConfig:Payment:Zarinpal:Sandbox"]);
                        });
                    });
            }).ConfigureStorage(builder => {
                builder.UseEfCore(options => {
                    var assemblyName = typeof(Startup).Assembly.GetName().Name;
                    options.ConfigureDbContext = db =>
                        db.UseNpgsql(
                            Configuration.GetConnectionString("DefaultConnection"),
                            sql => sql.MigrationsAssembly(assemblyName)
                        );

                    // If you prefer to have a separate MigrationHistory table for Parbad, you can change the above line to this:
                    options.ConfigureDbContext = db =>
                        db.UseNpgsql(
                            Configuration.GetConnectionString("DefaultConnection"),
                            sql => {
                                sql.MigrationsAssembly(assemblyName);
                                sql.MigrationsHistoryTable("PaymentHistory");
                            }
                        );

                    options.DefaultSchema = "public";

                    options.PaymentTableOptions.Name = "PaymentTable";
                    options.PaymentTableOptions.Schema = "public";

                    options.TransactionTableOptions.Name = "TransactionTable";
                    options.TransactionTableOptions.Schema = "public";
                });
            });
    }

    private void ConfigLogger(IServiceCollection services)
    {
        services.AddSingleton<IExceptionHolderService, ExceptionHolderService>();
        services.AddReportSharp(options => {
            var assemblyName = GetType().Assembly.GetName().Name;
            options.ConfigReportSharp(routerOptions =>
                routerOptions.SetApiPrefix(Configuration["LoggerConfig:ApiPrefix"])
                    .SetUsername(Configuration["LoggerConfig:Username"])
                    .SetSecretKey(Configuration["LoggerConfig:SecretKey"])
                    .SetWatchdogPrefix(Configuration["LoggerConfig:WatchdogPrefix"])
            ).AddExceptionReporter(() => new DatabaseReportOptionsBuilder<ReportSharpDbContext>()
                .SetOptionsBuilder(dbOptions =>
                    dbOptions.UseNpgsql(
                        Configuration.GetConnectionString("DefaultConnection"),
                        sql => sql.MigrationsAssembly(assemblyName)
                    )
                )
            );
            if (!Configuration["ComponentConfig:Environment"].Equals("Development")) {
                options.AddExceptionReporter(() => new DiscordReporterOptionsBuilder()
                    .SetToken(Configuration["LoggerConfig:DiscordToken"])
                    .AddChannelId(0)
                ).AddDataReporter(() => new DiscordReporterOptionsBuilder()
                    .SetToken(Configuration["LoggerConfig:DiscordToken"])
                    .AddChannelId(0)
                );
            }

            options.AddApis(apiOptions => apiOptions.UseAuthorization<DefaultApiAuthorizationService>());
        });
    }
}