using CourseWork.Application.Adapters;
using CourseWork.Domain.Entity;
using CourseWork.Infrastructure.AppDbContext.Configs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CourseWork.Infrastructure.AppDbContext;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<IdentityUser>(options), IFileRecordContext, ISaveDbContext
{
    public DbSet<FileRecord> FileRecords { get; init; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(FileRecordConfig).Assembly);
        base.OnModelCreating(builder);
    }
}
