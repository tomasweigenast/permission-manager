using Microsoft.EntityFrameworkCore;
using PermissionManager.Domain.Entities;

namespace PermissionManager.Persistence.Contexts;

/// <summary>
/// The default <see cref="DbContext"/> of the application.
/// </summary>
/// <param name="options"></param>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    /// <summary>
    /// A <see cref="DbSet{TEntity}"/> which represents the Permissions table.
    /// </summary>
    public DbSet<Permission> Permissions { get; set; }

    /// <summary>
    /// A <see cref="DbSet{TEntity}"/> which represents the PermissionTypes table.
    /// </summary>
    public DbSet<PermissionType> PermissionTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("Permisos");
            entity.HasKey(p => p.Id);

            entity.Property(p => p.EmployeeName)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("NombreEmpleado");
            
            entity.Property(p => p.EmployeeSurname)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("ApellidoEmpleado");

            entity.Property(p => p.CreatedAt)
                .IsRequired()
                .HasColumnName("FechaPermiso");

            entity.Property(p => p.PermissionTypeId)
                .IsRequired()
                .HasColumnName("TipoPermiso");
            
            entity.HasOne(p => p.PermissionType)
                .WithMany(pt => pt.Permissions)
                .HasForeignKey(pt => pt.PermissionTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PermissionType>(entity =>
        {
            entity.ToTable("TipoPermisos");
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("Descripcion");
        });
    }
    
    
}