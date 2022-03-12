using Infrastructure.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class AppDbContext : DbContextExtension
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor contextAccessor) :
        base(options)
    {
        ContextAccessor = contextAccessor;
    }

    public IHttpContextAccessor ContextAccessor { get; set; }
}