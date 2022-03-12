using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Parbad.Storage.EntityFrameworkCore.Context;
using ReportSharp.DatabaseReporter.DbContext;

namespace Presentation.Programs;

public class RefreshDatabaseProgram : AbstractProgram
{
    public override void Run(string[] args)
    {
        try {
            var dbContext = ServiceProvider.GetService<AppDbContext>();
            var parbadContext = ServiceProvider.GetService<ParbadDataContext>();
            var reportSharpDbContext = ServiceProvider.GetService<ReportSharpDbContext>();

            DropDbContext(dbContext);
            DropDbContext(parbadContext);
            DropDbContext(reportSharpDbContext);

            MigrateDbContext(dbContext);
            MigrateDbContext(parbadContext);
            MigrateDbContext(reportSharpDbContext);
        }
        catch (Exception e) {
            Console.WriteLine(e);
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

    private void DropDbContext(DbContext dbContext)
    {
        Console.WriteLine("Dropping {0}...", dbContext);
        dbContext.Database.EnsureDeleted();
    }

    public override string Name()
    {
        return "database:refresh";
    }

    public override string Description()
    {
        return "Drop database and create it again. Note: This will remove all tables, records and data.";
    }

    public override string Section()
    {
        return "Database";
    }
}