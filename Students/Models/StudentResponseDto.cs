using Students.Enums;

namespace Students.Models
{
    public class StudentResponseDto
    {
                public StudentResponseDto() { }

        public StudentResponseDto(int id, string firstName, string lastName, string? email, string grade, EnrollmentStatus enrollmentStatus)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Grade = grade;
            EnrollmentStatus = enrollmentStatus;
        }

        public int Id { get; set; }
        
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string Grade { get; set; } = string.Empty;

        public EnrollmentStatus EnrollmentStatus { get; set; }
    };
}