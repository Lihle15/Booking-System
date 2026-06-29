using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace BookingSystem.Models
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [RegularExpression(@"^[A-Z].*$", ErrorMessage = "Username must start with an uppercase letter.")]
        public string Username { get; set; }

        [Required]
        [MaxLength(255)]
        [PasswordValidation]
        public string Password { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string Surname { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required]
        [MaxLength(15)]
        [Phone]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be 10 digits long.")]
        [Column("phone_no")]
        public string PhoneNo { get; set; }

        [Required]
        [Column("date_of_birth")]
        public DateTime DateOfBirth { get; set; }
    }

    public class PasswordValidation : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var password = value as string;

            if (password == null || password.Length < 8)
            {
                return new ValidationResult("Password must be at least 8 characters long.");
            }

            if (!Regex.IsMatch(password, @"[!@#$%^&*(),.?""{}|<>]")) // At least 1 special character
            {
                return new ValidationResult("Password must contain at least 1 special character.");
            }

            if (Regex.Matches(password, @"\d").Count < 2) // At least 2 numbers
            {
                return new ValidationResult("Password must contain at least 2 numbers.");
            }

            return ValidationResult.Success;
        }
    }
}
