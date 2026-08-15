using Clientes.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clientes.DataAccess.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes", "dbo");

        builder.HasKey(c => c.ClienteId);

        builder.Property(c => c.ClienteId)
            .UseIdentityColumn();

        builder.Property(c => c.Nombre)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.ApellidoPaterno)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.ApellidoMaterno)
            .HasMaxLength(100);

        builder.Property(c => c.CorreoElectronico)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Telefono)
            .HasMaxLength(20);

        builder.Property(c => c.FechaNacimiento)
            .HasColumnType("date");

        builder.Property(c => c.Direccion)
            .HasMaxLength(250);

        builder.Property(c => c.Ciudad)
            .HasMaxLength(100);

        builder.Property(c => c.CodigoPostal)
            .HasMaxLength(10);

        builder.Property(c => c.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(c => c.FechaRegistro)
            .IsRequired();

        builder.Property(c => c.FechaModificacion);

        builder.HasIndex(c => c.CorreoElectronico)
            .IsUnique()
            .HasDatabaseName("UQ_Clientes_Correo");

        builder.HasIndex(c => new { c.ApellidoPaterno, c.Nombre })
            .HasDatabaseName("IX_Clientes_Nombre");
    }
}
