using System.ComponentModel.DataAnnotations;
using Students.Models.Validation;
using Students.Enums;

namespace Students.Models
{
    public class Student
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string Grade { get; set; } = string.Empty;
        public EnrollmentStatus EnrollmentStatus { get; set; } = EnrollmentStatus.Active;

   }
}
