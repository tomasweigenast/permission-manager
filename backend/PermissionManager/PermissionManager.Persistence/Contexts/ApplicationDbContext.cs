using Microsoft.EntityFrameworkCore;
using PermissionManager.Domain.Entities;

namespace PermissionManager.Persistence.Contexts;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Permission> Permissions { get; set; }

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