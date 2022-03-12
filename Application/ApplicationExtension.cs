using System.Reflection;
using System.Text.Json;
using Application.Common.Response;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationExtension
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(Assembly.GetExecutingAssembly());

        services.AddMvc()
            .AddFluentValidation(x => x.AutomaticValidationEnabled = false);

        // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));

        return services;
    }

    public static IApplicationBuilder UseApplication(this IApplicationBuilder app)
    {
        app.UseStatusCodePages(async context => {
            switch (context.HttpContext.Response.StatusCode) {
                case 401 when context.HttpContext.Response.ContentType != "application/json":
                    context.HttpContext.Response.ContentType = "application/json";
                    await context.HttpContext.Response.WriteAsync(
                        JsonSerializer.Serialize(ResponseFormat.NotAuth<object>().Value
                        )
                    );
                    break;
                case 403 when context.HttpContext.Response.ContentType != "application/json":
                    context.HttpContext.Response.ContentType = "application/json";
                    await context.HttpContext.Response.WriteAsync(
                        JsonSerializer.Serialize(
                            ResponseFormat.PermissionDeniedMsg<object>("شما به این قسمت دسترسی ندارید.").Value
                        )
                    );
                    break;
                case 400:
                    context.HttpContext.Response.ContentType = "application/json";
                    await context.HttpContext.Response.WriteAsync(
                        JsonSerializer.Serialize(ResponseFormat.BadRequestMsg<object>("درخواست نامعتبر").Value
                        )
                    );
                    break;
                case 500:
                    context.HttpContext.Response.ContentType = "application/json";
                    await context.HttpContext.Response.WriteAsync(
                        JsonSerializer.Serialize(
                            ResponseFormat.InternalErrorMsg<object>("مشکلی در سرور رخ داده است.").Value
                        )
                    );
                    break;
            }
        });

        return app;
    }
}