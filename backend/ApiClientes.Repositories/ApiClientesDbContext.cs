using ApiClientes.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiClientes.Repositories;

/// <summary>
/// Contexto de Entity Framework Core para la base de datos DBClientes.
/// </summary>
public class ApiClientesDbContext : DbContext
{
    public ApiClientesDbContext(DbContextOptions<ApiClientesDbContext> options) : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("Clientes");
            entity.HasKey(c => c.IdCliente);

            entity.Property(c => c.IdCliente)
                .ValueGeneratedOnAdd();

            entity.Property(c => c.Identificacion)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(c => c.Identificacion)
                .IsUnique();

            entity.Property(c => c.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(c => c.Apellido)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(c => c.Email)
                .HasMaxLength(150);

            entity.Property(c => c.FechaCreacion)
                .HasDefaultValueSql("getdate()");

            entity.Property(c => c.FechaActualizacion)
                .HasDefaultValueSql("getdate()");
        });
    }
}
