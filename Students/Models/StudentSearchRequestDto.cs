using System.ComponentModel.DataAnnotations;
using Students.Enums;
using Students.Models.Validation;

public class StudentSearchRequest
{
    [Grade]
    public string? Grade { get; set; }

    public EnrollmentStatus? EnrollmentStatus { get; set; }

    [StringLength(100, ErrorMessage = "Name filter too long.")]
    public string? NameContains { get; set; }

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 200)]
    public int PageSize { get; set; } = 50;

    public string? SortBy { get; set; } = "Id";   // Id|FirstName|LastName|Grade
    public bool Desc { get; set; } = false;
}
