using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Common;

public class DbContextExtension : SoftDeletes.Core.DbContext
{
    protected DbContextExtension()
    {
    }

    public DbContextExtension(DbContextOptions options) : base(options)
    {
    }

    public void DetachEntities(List<object> shouldDetach = null, List<object> except = null)
    {
        var states = new List<EntityState> {
            EntityState.Added,
            EntityState.Modified,
            EntityState.Deleted,
            EntityState.Unchanged,
        };
        var entitiesList = ChangeTracker.Entries()
            .Where(x => {
                var result = states.Contains(x.State);
                if (shouldDetach != null) {
                    result &= shouldDetach.Contains(x.Entity);
                }

                if (except != null) {
                    result &= !except.Contains(x.Entity);
                }

                return result;
            }).ToList();
        entitiesList.ForEach(x => x.State = EntityState.Detached);
    }

    protected override void SetNewEntitiesTimestamps()
    {
        base.SetNewEntitiesTimestamps();

        var persianToday = PersianDateTime.Now.Date;
        ChangeTracker.Entries().Where(x => x.State == EntityState.Added && x.Entity is IPersianDate)
            .Select(x => x.Entity as IPersianDate)
            .Where(x => x != null)
            .Select(x => x!)
            .ToList()
            .ForEach(x => {
                x.PersianYearCreated = persianToday.Year;
                x.PersianMonthCreated = persianToday.Month;
                x.PersianDayCreated = persianToday.Day;
            });
    }
}