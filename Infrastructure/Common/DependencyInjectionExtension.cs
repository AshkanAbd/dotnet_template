using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Common;

public class DependencyInjectionExtension
{
    protected IHost Host { get; set; }
    public IServiceProvider ServiceProvider { get; set; }

    protected object[] CreateConstructorParameters(ConstructorInfo constructor)
    {
        return constructor.GetParameters()
            .Select(parameter => ServiceProvider!.GetService(parameter.ParameterType))
            .ToArray();
    }


    protected ConstructorInfo GetConstructorInfo(Type controller)
    {
        if (controller.GetConstructors().Length < 1) {
            return null;
        }

        return controller.GetConstructors()[0];
    }

    protected void CreateServiceProvider()
    {
        using var scope = Host!.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

        ServiceProvider = scopeFactory.CreateScope().ServiceProvider;
    }

    protected void CreateHost<T>() where T : class
    {
        var hostBuilder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<T>());

        Host = hostBuilder.Build();
    }
}