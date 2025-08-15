using System.Collections.Generic;
using Students.Models;

namespace Students.Services
{
    public interface IStudentService
    {
        IEnumerable<StudentResponseDto> GetAll();
        StudentResponseDto? GetById(int id);
        StudentResponseDto Create(StudentRequestDto dto);
        bool Update(int id, StudentRequestDto dto);
        bool Delete(int id);
    }
}
