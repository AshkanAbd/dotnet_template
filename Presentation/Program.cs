using Presentation.Programs;

namespace Presentation;

public static class Program
{
    private static IDictionary<string, AbstractProgram> ArgsStates;

    private static void RegisterStates()
    {
        ArgsStates = AbstractProgram.RegisterPrograms();
    }

    public static void Main(string[] args)
    {
        RegisterStates();

        var host = CreateHostBuilder(args).Build();

        using var scope = host.Services.CreateScope();
        if (args.Length >= 1 && ArgsStates.ContainsKey(args[0].ToLower())) {
            ArgsStates[args[0].ToLower()].ServiceProvider = scope.ServiceProvider;
            ArgsStates[args[0].ToLower()].InitServices();
            ArgsStates[args[0].ToLower()].Run(args);
            return;
        }

        host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
                webBuilder.UseStartup<Startup>()
            );
}