using Infrastructure;
using Parbad.Storage.EntityFrameworkCore.Context;

namespace Presentation.Programs;

public class DropDatabaseProgram : AbstractProgram
{
    public override void Run(string[] args)
    {
        var dbContext = ServiceProvider.GetService<AppDbContext>();
        var parbadContext = ServiceProvider.GetService<ParbadDataContext>();

        Console.WriteLine("Dropping database on {0}...", dbContext);
        Console.WriteLine("Dropping database on {0}...", parbadContext);

        dbContext.Database.EnsureDeleted();
        parbadContext.Database.EnsureDeleted();
    }

    public override string Name()
    {
        return "database:drop";
    }

    public override string Description()
    {
        return "Drop databases";
    }

    public override string Section()
    {
        return "Database";
    }
}