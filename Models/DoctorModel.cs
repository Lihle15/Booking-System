using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingSystem.Models
{
    public class DoctorModel
    {
        [Key]
        [Column("doctor_id")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Specialty is required.")]
        [MaxLength(50)]
        public string Specialty { get; set; }

        [Required(ErrorMessage = "Availability is required.")]
        public string Availability { get; set; }

        [Required(ErrorMessage = "Contact details are required.")]
        [MaxLength(15)]
        [Column("contact_details")]
        public string ContactDetails { get; set; }

        public string Password { get; set; }

        public string Status { get; set; }  // This could be "Active" or "Inactive"

    }
}
