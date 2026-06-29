using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingSystem.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Patient")]
        [Column("patient_id")]
        public int PatientId { get; set; }

        [Required]
        [Column("appointment_date")]
        public DateTime AppointmentDate { get; set; }

        [Required]
        [Column("appointment_time")]
        public TimeSpan AppointmentTime { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("doctor_name")]
        public string SelectedDoctor { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Confirmed";

        // Navigation Property
        public virtual Patient Patient { get; set; }
        public string DoctorName => SelectedDoctor;
    }
}
