using System;
using System.Collections.Generic;
using System.Linq;
using Students.Models;

namespace Students.Mappings
{
    public static class StudentMappings
    {
        public static Student ToEntity(this StudentRequestDto dto, int id = 0)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new Student
            {
                Id = id, // route id wins for PUT; leave 0 for POST
                FirstName  = dto.FirstName?.Trim() ?? string.Empty,
                LastName   = dto.LastName?.Trim() ?? string.Empty,
                Address    = dto.Address?.Trim() ?? string.Empty,
                DateOfBirth= dto.DateOfBirth,
                Email      = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email,
                Phone      = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone,
                Grade      = dto.Grade?.Trim() ?? string.Empty
            };
        }

        public static void Apply(this Student target, StudentRequestDto dto)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            target.FirstName   = dto.FirstName?.Trim() ?? string.Empty;
            target.LastName    = dto.LastName?.Trim() ?? string.Empty;
            target.Address     = dto.Address?.Trim() ?? string.Empty;
            target.DateOfBirth = dto.DateOfBirth;
            target.Email       = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email;
            target.Phone       = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone;
            target.Grade       = dto.Grade?.Trim() ?? string.Empty;
        }

        public static StudentResponseDto ToResponseDto(this Student s)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));

            return new StudentResponseDto
            {
                Id          = s.Id,
                FirstName   = s.FirstName,
                LastName    = s.LastName,
                Address     = s.Address,
                DateOfBirth = s.DateOfBirth,
                Email       = s.Email,
                Phone       = s.Phone,
                Grade       = s.Grade
            };
        }

        public static IEnumerable<StudentResponseDto> ToResponseDtos(this IEnumerable<Student> students)
            => students.Select(s => s.ToResponseDto());
    }
}
