using CourseWork.Domain.Enum;
using Microsoft.AspNetCore.Identity;

namespace CourseWork.Domain.Entity;

public class FileRecord
{
    public required Guid Id { get; set; }
    public required string FileName { get; set; }
    public long? FileSize { get; set; }
    public DateTime UploadedAt { get; set; }
    public Guid? ParentId { get; set; }
    public required IdentityUser Owner { get; set; }
    public required FileType FileType { get; set; } 
}