using CourseWork.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseWork.Infrastructure.AppDbContext.Configs;

public class FileRecordConfig : IEntityTypeConfiguration<FileRecord>
{
    public void Configure(EntityTypeBuilder<FileRecord> builder)
    {
        builder.HasKey(record => record.Id);
        builder.Property(record => record.FileName).HasMaxLength(250);
        builder.ToTable("file_record");
    }
}