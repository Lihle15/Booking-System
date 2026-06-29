using Microsoft.EntityFrameworkCore;
using BookingSystem.Models;

namespace BookingSystem.Data
{
    public class BookingContext : DbContext
    {
        public BookingContext(DbContextOptions<BookingContext> options) : base(options) { }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<DoctorModel> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<DoctorModel>().ToTable("doctors");
            modelBuilder.Entity<DoctorModel>().Property(d => d.Status).HasMaxLength(50).IsRequired();

        }
    }
}
