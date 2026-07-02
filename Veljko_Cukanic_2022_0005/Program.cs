using AutoService.Application.Auth;
using AutoService.Application.Mediator;
using AutoService.Application.Repositories;
using AutoService.Application.SystemOperations;
using AutoService.Application.Termini.Commands;
using AutoService.Infrastructure.Data;
using AutoService.Infrastructure.Repositories;
using AutoService.Web.Auth;
using AutoService.Web.Mediator;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AutoServiceDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ITerminRepository, TerminRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<ZakaziTerminOperation>();
builder.Services.AddScoped<OtkaziTerminOperation>();
builder.Services.AddScoped<ZavrsiTerminOperation>();

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddSingleton<ApplicationSession>();

builder.Services.AddScoped<IMediator, SimpleMediator>();
builder.Services.AddMediatorHandlers(
    typeof(ZakaziTerminCommand).Assembly,
    typeof(Program).Assembly);

builder.Services
    .AddAuthentication(JwtAuthenticationHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, JwtAuthenticationHandler>(
        JwtAuthenticationHandler.SchemeName,
        options => { });

builder.Services.AddAuthorization();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}")
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<AutoServiceDbContext>();

    await dbContext.Database.MigrateAsync();

    var passwordHasher = scope.ServiceProvider
        .GetRequiredService<IPasswordHasher>();

    await DbInitializer.InitializeAsync(dbContext, passwordHasher);
}

app.Run();

