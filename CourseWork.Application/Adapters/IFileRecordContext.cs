using CourseWork.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace CourseWork.Application.Adapters;

public interface IFileRecordContext
{
    DbSet<FileRecord> FileRecords { get; init; }
}