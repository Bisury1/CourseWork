using CourseWork.Application.Adapters;
using CourseWork.Infrastructure.AppDbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CourseWork.Infrastructure.DI;

public static class DependencyInjectionExtension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var connectionString = configuration["ConnectionString"];
        serviceCollection.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        serviceCollection.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
        serviceCollection.AddScoped<IFileRecordContext>(serv => serv.GetRequiredService<ApplicationDbContext>());
        serviceCollection.AddScoped<ISaveDbContext>(serv => serv.GetRequiredService<ApplicationDbContext>());

        return serviceCollection;
    }
}