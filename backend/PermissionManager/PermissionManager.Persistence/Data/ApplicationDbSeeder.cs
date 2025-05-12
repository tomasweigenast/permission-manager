using Microsoft.EntityFrameworkCore;
using PermissionManager.Domain.Entities;
using PermissionManager.Persistence.Contexts;

namespace PermissionManager.Persistence.Data;

/// <summary>
/// A helper class which is used to seed the database.
/// </summary>
public static class ApplicationDbSeeder
{
    private static readonly string[] Permissions = ["Acceso a planta de producción", "Acceso a administración", "Acceso a sistemas"];
    
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (!await context.PermissionTypes.AnyAsync())
        {
            context.PermissionTypes.AddRange(Permissions.Select(permission => new PermissionType
            {
                Description = permission
            }));
            await context.SaveChangesAsync();
        }
    }
}