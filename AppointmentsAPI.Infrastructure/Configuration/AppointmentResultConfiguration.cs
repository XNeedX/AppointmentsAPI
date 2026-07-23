using AppointmentsAPI.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppointmentsAPI.Infrastructure.Configuration;

public class AppointmentResultConfiguration : IEntityTypeConfiguration<AppointmentResult>
{
    public void Configure(EntityTypeBuilder<AppointmentResult> builder)
    {
        builder.ToTable("AppointmentResults");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Complaints).IsRequired();
        builder.Property(r => r.Conclusion).IsRequired();
        builder.Property(r => r.Recommendations).IsRequired();

        builder.HasOne(r => r.Appointment)
            .WithOne(a => a.Result)
            .HasForeignKey<AppointmentResult>(r => r.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
