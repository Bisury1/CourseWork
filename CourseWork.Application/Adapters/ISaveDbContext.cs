namespace CourseWork.Application.Adapters;

public interface ISaveDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}