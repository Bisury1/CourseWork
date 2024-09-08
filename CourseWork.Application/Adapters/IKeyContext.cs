using CourseWork.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace CourseWork.Application.Adapters;

public interface IKeyContext
{
    DbSet<AesKey> AesKeys { get; init; }
}