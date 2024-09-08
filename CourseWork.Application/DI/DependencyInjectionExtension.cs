using CourseWork.Application.Adapters;
using CourseWork.Application.Services.FileService;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CourseWork.Application.DI;

public static class DependencyInjectionExtension
{
    public static IServiceCollection AddApplication(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IFileService, FileService>();
        return serviceCollection;
    }
}