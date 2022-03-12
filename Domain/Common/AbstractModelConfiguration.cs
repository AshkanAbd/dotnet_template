using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoftDeletes.ModelTools;

namespace Domain.Common;

public abstract class AbstractModelConfiguration<T> : IEntityTypeConfiguration<T> where T : class
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        if (typeof(T).IsAssignableTo(typeof(ISoftDelete))) {
            builder.HasQueryFilter(x => (x as ISoftDelete).DeletedAt == null);
        }
    }
}