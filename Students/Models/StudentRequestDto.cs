using System.ComponentModel.DataAnnotations;
using Students.Models.Validation;
using Students.Enums;

namespace Students.Models
{
    public class StudentRequestDto
    {
        [Required, MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Address { get; set; } = string.Empty;

        [Required, PastDate]
        public DateTime DateOfBirth { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [UsPhone]
        public string? Phone { get; set; }

        // “K” or "1".."12"
        [Required, Grade]
        public string Grade { get; set; } = string.Empty;

        public EnrollmentStatus? EnrollmentStatus { get; set; }

    }
}
