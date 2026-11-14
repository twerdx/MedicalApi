using MedicalApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalApi.Data;

public class MedicalDbContext : DbContext
{
    public MedicalDbContext(DbContextOptions<MedicalDbContext> options) : base(options)
    {
    }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<Spravka> Spravki { get; set; }
    public DbSet<SpravkaType> SpravkaTypes { get; set; }
    public DbSet<Appointment> Appointments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Restrict: нельзя удалить пациента / вид справки, пока на них ссылаются справки

        modelBuilder.Entity<Spravka>()
            .HasOne<Patient>()
            .WithMany()
            .HasForeignKey(s => s.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Spravka>()
            .HasOne<SpravkaType>()
            .WithMany()
            .HasForeignKey(s => s.SpravkaTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointment>()
            .HasOne<Patient>()
            .WithMany()
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}