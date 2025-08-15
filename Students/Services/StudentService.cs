using Students.Models;
using Students.Repositories;
using Students.Mappings;
using Students.Exceptions;

namespace Students.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _repo;
        private readonly ILogger<StudentService> _log;

        public StudentService(IStudentRepository repo, ILogger<StudentService> log)
        {
            _repo = repo;
            _log = log;
        }

        public IEnumerable<StudentResponseDto> GetAll()
        {
            _log.LogInformation("GET GetAll");
           return _repo.GetAll().Select(s => s.ToResponseDto());
        }

        public StudentResponseDto? GetById(int id)
        {
            _log.LogInformation("GET /students/{Id}", id);
            return _repo.GetById(id)?.ToResponseDto();
        }

        public StudentResponseDto Create(StudentRequestDto dto)
        {
            _log.LogInformation("POST Create/{student}", dto);
            ThrowErrorIfDuplicateEmail(dto);

            var created = _repo.Insert(dto.ToEntity()); // repo assigns Id

            return created.ToResponseDto();
        }

        public bool Update(int id, StudentRequestDto dto)
        {
            _log.LogInformation("PUT Update/{student}", dto);

            // Look up the current record
            var current = _repo.GetAll().FirstOrDefault(s => s.Id == id);
            if (current is null) return false; // or throw NotFound if that's your pattern

            var newEmail = Normalize(dto.Email);
            var oldEmail = Normalize(current.Email);

            // Only check for dupes if the email changed, and exclude the current id
            if (!string.Equals(newEmail, oldEmail, StringComparison.Ordinal))
            {
                ThrowErrorIfDuplicateEmail(dto, excludeId: id);
            }

            return _repo.Update(id, dto.ToEntity(id)); // route id wins
        }

        private void ThrowErrorIfDuplicateEmail(StudentRequestDto dto, int? excludeId = null)
        {
            var email = Normalize(dto.Email);
            if (email is null) return;

            var clash = _repo.GetAll()
                .FirstOrDefault(s => Normalize(s.Email) == email &&
                                    (!excludeId.HasValue || s.Id != excludeId.Value));

            if (clash is not null)
                throw new DuplicateEmailException(dto.Email!, clash.Id);
        }

        public bool Delete(int id)
        {
            _log.LogInformation("DELETE {student ID}", id);
            return _repo.Delete(id);
        }

        private static string? Normalize(string? e)
            => string.IsNullOrWhiteSpace(e) ? null : e.Trim().ToLowerInvariant();

    }
}
