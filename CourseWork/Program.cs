using System.Reflection;
using CourseWork;
using CourseWork.Application.DI;
using CourseWork.Domain.Enum;
using CourseWork.Infrastructure.AppDbContext;
using CourseWork.Infrastructure.DI;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthorization();

builder.Services.AddInfrastructure(builder.Configuration).AddApplication();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => options.LoginPath = "/v1/users/auth");

builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Books Reviews API",
        Description = "An ASP.NET Core Web API for managing items",
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddControllers();
builder.Services.AddLogging(option => option.AddSimpleConsole());

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = await roleManager.Roles.ToListAsync();
    if (roles.FirstOrDefault(r => r.Name == nameof(UserRoles.Admin)) is null)
        await roleManager.CreateAsync(new IdentityRole { Name = nameof(UserRoles.Admin), NormalizedName = "ADMIN" });
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var user = Storage.User;
    await userManager.CreateAsync(user, Storage.UserPassword);
    user = await userManager.FindByNameAsync("a.nebaev");
    await userManager.AddToRoleAsync(user, nameof(UserRoles.Admin));
}

app.UseSwagger(options =>
{
    options.SerializeAsV2 = true;
});
app.UseSwaggerUI();

app.UseExceptionHandler("/Home/Error");
// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
app.UseHsts();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();