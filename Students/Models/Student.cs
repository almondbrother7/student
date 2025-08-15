using System.ComponentModel.DataAnnotations;
using Students.Models.Validation;

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
   }
}
