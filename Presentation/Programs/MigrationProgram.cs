using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Parbad.Storage.EntityFrameworkCore.Context;
using ReportSharp.DatabaseReporter.DbContext;

namespace Presentation.Programs;

public class MigrationProgram : AbstractProgram
{
    public override void Run(string[] args)
    {
        try {
            var dbContext = ServiceProvider.GetService<AppDbContext>();
            var parbadContext = ServiceProvider.GetService<ParbadDataContext>();
            var reportSharpDbContext = ServiceProvider.GetService<ReportSharpDbContext>();

            MigrateDbContext(dbContext);
            MigrateDbContext(parbadContext);
            MigrateDbContext(reportSharpDbContext);
        }
        catch (Exception e) {
            Console.WriteLine(e);
            Console.WriteLine("Error on migration, maybe you need to drop database.");
        }
    }

    private void MigrateDbContext(DbContext dbContext)
    {
        if (dbContext.Database.GetPendingMigrations().Any()) {
            dbContext.Database.GetPendingMigrations()
                .ToList()
                .ForEach(x => Console.WriteLine("Migrating {0} on {1}...", x, dbContext));
            dbContext.Database.Migrate();
            Console.WriteLine("{0} migrated", dbContext);
        }
        else {
            Console.WriteLine("Nothing to migrate in {0}", dbContext);
        }
    }

    public override string Name()
    {
        return "migrate";
    }

    public override string Description()
    {
        return "Migrate database new changes";
    }

    public override string Section()
    {
        return "Migration";
    }
}