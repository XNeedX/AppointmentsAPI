using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AppointmentsAPI.Domain.Models;

namespace AppointmentsAPI.Infrastructure.Configurations;

public class ReceptionistConfiguration : IEntityTypeConfiguration<Receptionist>
{
    public void Configure(EntityTypeBuilder<Receptionist> builder)
    {
        builder.ToTable("Receptionists");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .ValueGeneratedNever();

        builder.Property(r => r.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.MiddleName)
            .HasMaxLength(100);

        builder.Property(r => r.Status)
            .IsRequired();
    }
}