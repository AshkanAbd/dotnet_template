using Ef.Seeder;
using Infrastructure;

namespace Presentation.Programs;

public class SeedProgram : AbstractProgram
{
    public override void Run(string[] args)
    {
        var databaseSeeder = new DatabaseSeeder(ServiceProvider, ServiceProvider.GetService<AppDbContext>());

        var isProduction = Configuration["ComponentConfig:Environment"].ToLower() == "production";
        databaseSeeder.IsProductionEnvironment(isProduction)
            .EnsureSeeded(true);
    }

    public override string Name()
    {
        return "seed";
    }

    public override string Description()
    {
        return "Seed database";
    }

    public override string Section()
    {
        return "Seed";
    }
}