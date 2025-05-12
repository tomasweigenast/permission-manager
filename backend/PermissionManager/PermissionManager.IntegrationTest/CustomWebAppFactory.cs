using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using PermissionManager.API;
using PermissionManager.Application.Interfaces;
using PermissionManager.Domain.Entities;
using PermissionManager.Persistence.Contexts;

namespace PermissionManager.IntegrationTest;

public class CustomWebAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove default DbContext
            services.Remove(services.Single(d =>
                d.ServiceType == typeof(IDbContextOptionsConfiguration<ApplicationDbContext>)));

            // Inject in-memory version
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
                options.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });

            // Delete real IFullTextSearchService and IProducerService implementations
            var ftsDesc = services.SingleOrDefault(
                d => d.ServiceType == typeof(IFullTextSearchService));
            if (ftsDesc != null) services.Remove(ftsDesc);
            var prodDesc = services.SingleOrDefault(
                d => d.ServiceType == typeof(IProducerService));
            if (prodDesc != null) services.Remove(prodDesc);

            // Replace with stubs
            services.AddSingleton<IFullTextSearchService, NoOpFullTextSearch>();
            services.AddSingleton<IProducerService, NoOpProducerService>();

            var sp = services.BuildServiceProvider();
            
            // Database seed
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureCreated();
            if (db.PermissionTypes.Any()) return;
            
            db.PermissionTypes.AddRange(
                new PermissionType { Description = "Read" },
                new PermissionType { Description = "Write" }
            );
            db.SaveChanges();
        });
    }
}